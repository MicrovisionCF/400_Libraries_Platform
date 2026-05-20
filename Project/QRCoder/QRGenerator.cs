using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microvision.Types;

namespace Microvision.QRCoder
{
    internal class QRGenerator : Citizen
    {
        // ***************************************************************************************************
        // 13.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private readonly List<char> _numChars;
        private readonly Dictionary<char, int> _alphaNumEncValues;
        private readonly List<xQRAntilog> _galoisField;
        private readonly QRConfigsInfos _configsInfos;
        private readonly QRMasker _masker;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRGenerator()
        {
            _numChars = QRTablesShop.CreateNumChars();
            _alphaNumEncValues = QRTablesShop.CreateAlphaNumEncValues();
            _galoisField = QRTablesShop.CreateAntilogTable();
            _configsInfos = QRTablesShop.CreateCapacities();
            _masker = new QRMasker();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public QRData CreateQrCode(string text, QRStrength strength, int forcedVersion)
        {
            // Flags conservés à false en dur, en attendant un besoin futur potentiel
            bool forceUtf8 = false;
            bool utf8BOM = false;

            QREncodingMode encoding = oDetectEncoding(text, forceUtf8);
            string codedText = oTextToBinary(text, encoding, utf8BOM, forceUtf8);
            int dataLength = oGetDataLength(encoding, text, codedText, forceUtf8);

            QRVersion version;
            if (forcedVersion != -1)
            {
                version = (QRVersion)forcedVersion;
            }
            else
            {
                version = _configsInfos.ChooseVersion(dataLength, encoding, strength);
                zCheckVersionIsValid(version, dataLength);
            }

            xQRConfigInfos configInfos = _configsInfos[version][strength];
            zCheckCapacityIsValid(configInfos, encoding, dataLength);

            string bitString = oCreateBitString(codedText, encoding, dataLength, configInfos);
            List<xQRCodewordBlock> codeWords = oCreateCodeWords(bitString, configInfos);
            string interleavedData = oCreateInterleavedData(codeWords, configInfos);

            return oCreateQrCode(interleavedData, configInfos);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected QRPolynom oCalculateGeneratorPolynom(int wordsCount)
        {
            QRPolynom generatorPolynom = new QRPolynom();
            generatorPolynom.PolyItems.AddRange(new[] { new QRPolynom.Item(0, 1), new QRPolynom.Item(0, 0) });

            for (int i = 1; i < wordsCount; i++)
            {
                QRPolynom multiplierPolynom = new QRPolynom();
                multiplierPolynom.PolyItems.AddRange(new[] { new QRPolynom.Item(0, 1), new QRPolynom.Item(i, 0) });
                generatorPolynom = oMultiplyAlphaPolynoms(generatorPolynom, multiplierPolynom);
            }

            return generatorPolynom;
        }

        protected List<string> oCalculateWords(string bitString, xQRConfigInfos strengthInfo)
        {
            QRPolynom messagePolynom = zCalculateMessagePolynom(bitString);
            QRPolynom generatorPolynom = oCalculateGeneratorPolynom(strengthInfo.wordsPerBlock);

            for (int i = 0; i < messagePolynom.PolyItems.Count; i++)
                messagePolynom.PolyItems[i] = new QRPolynom.Item(messagePolynom.PolyItems[i].coefficient, messagePolynom.PolyItems[i].exponent + strengthInfo.wordsPerBlock);

            for (int i = 0; i < generatorPolynom.PolyItems.Count; i++)
                generatorPolynom.PolyItems[i] = new QRPolynom.Item(generatorPolynom.PolyItems[i].coefficient, generatorPolynom.PolyItems[i].exponent + (messagePolynom.PolyItems.Count - 1));

            QRPolynom leadTermSource = messagePolynom;

            int exp = 0;
            while (leadTermSource.PolyItems.Count > 0 && leadTermSource.PolyItems[leadTermSource.PolyItems.Count - 1].exponent > 0)
            {
                if (leadTermSource.PolyItems[0].coefficient == 0)
                {
                    leadTermSource.PolyItems.RemoveAt(0);
                    leadTermSource.PolyItems.Add(new QRPolynom.Item(0, leadTermSource.PolyItems[leadTermSource.PolyItems.Count - 1].exponent - 1));
                }
                else
                {
                    QRPolynom resPoly = zMultiplyGeneratorPolynomByLeadterm(generatorPolynom, oConvertToAlphaNotation(leadTermSource).PolyItems[0], exp);
                    resPoly = oConvertToDecNotation(resPoly);
                    resPoly = zXORPolynoms(leadTermSource, resPoly);
                    leadTermSource = resPoly;
                }

                exp++;
            }

            return leadTermSource.PolyItems.Select(x => zzDecToBin(x.coefficient, 8)).ToList();
        }

        protected QRPolynom oConvertToAlphaNotation(QRPolynom poly)
        {
            QRPolynom newPoly = new QRPolynom();

            foreach (QRPolynom.Item polyItem in poly.PolyItems)
                newPoly.PolyItems.Add(new QRPolynom.Item(polyItem.coefficient != 0 ? zGetAlphaExpFromIntVal(_galoisField, polyItem.coefficient) : 0, polyItem.exponent));

            return newPoly;
        }

        protected QRPolynom oConvertToDecNotation(QRPolynom poly)
        {
            QRPolynom newPoly = new QRPolynom();

            foreach (QRPolynom.Item polyItem in poly.PolyItems)
                newPoly.PolyItems.Add(new QRPolynom.Item(zGetIntValFromAlphaExp(_galoisField, polyItem.coefficient), polyItem.exponent));

            return newPoly;
        }

        protected string oCreateBitString(string codedText, QREncodingMode encoding, int dataInputLength, xQRConfigInfos info)
        {
            int dataLength = info.GetTotalDataCodewords() * 8;
            string modeIndicator = zzDecToBin((int)encoding, 4);
            string countIndicator = zzDecToBin(dataInputLength, zGetCountIndicatorLength(info.version, encoding));
            string bitString = modeIndicator + countIndicator;
            bitString += codedText;
            bitString = zAdjustBitStringLength(bitString, dataLength);

            return bitString;
        }

        protected List<xQRCodewordBlock> oCreateCodeWords(string bitString, xQRConfigInfos info)
        {
            List<xQRCodewordBlock> codeWords = new List<xQRCodewordBlock>();

            for (int i = 0; i < info.blocksInGroup1; i++)
            {
                string bitStr = bitString.Substring(i * info.codewordsInGroup1 * 8, info.codewordsInGroup1 * 8);
                List<string> bitBlockList = zBinaryStringToBitBlockList(bitStr);
                List<int> bitBlockListDec = zBinaryStringListToDecList(bitBlockList);
                List<string> wordList = oCalculateWords(bitStr, info);
                List<int> wordListDec = zBinaryStringListToDecList(wordList);
                codeWords.Add(new xQRCodewordBlock(1, i + 1, bitStr, bitBlockList, wordList, bitBlockListDec, wordListDec));
            }

            bitString = bitString.Substring(info.blocksInGroup1 * info.codewordsInGroup1 * 8);

            for (int i = 0; i < info.blocksInGroup2; i++)
            {
                string bitStr = bitString.Substring(i * info.codewordsInGroup2 * 8, info.codewordsInGroup2 * 8);
                List<string> bitBlockList = zBinaryStringToBitBlockList(bitStr);
                List<int> bitBlockListDec = zBinaryStringListToDecList(bitBlockList);
                List<string> wordList = oCalculateWords(bitStr, info);
                List<int> wordListDec = zBinaryStringListToDecList(wordList);
                codeWords.Add(new xQRCodewordBlock(2, i + 1, bitStr, bitBlockList, wordList, bitBlockListDec, wordListDec));
            }

            return codeWords;
        }

        protected string oCreateInterleavedData(List<xQRCodewordBlock> codeWords, xQRConfigInfos info)
        {
            StringBuilder interleavedWordsSb = new StringBuilder();

            for (int i = 0; i < Math.Max(info.codewordsInGroup1, info.codewordsInGroup2); i++)
                foreach (xQRCodewordBlock codeBlock in codeWords)
                    if (codeBlock.codeWords.Count > i)
                        interleavedWordsSb.Append(codeBlock.codeWords[i]);

            for (int i = 0; i < info.wordsPerBlock; i++)
                foreach (xQRCodewordBlock codeBlock in codeWords)
                    if (codeBlock.words.Count > i)
                        interleavedWordsSb.Append(codeBlock.words[i]);

            interleavedWordsSb.Append(new string('0', info.reminderBits));

            return interleavedWordsSb.ToString();
        }

        protected QRData oCreateQrCode(string interleavedData, xQRConfigInfos info)
        {
            QRData qr = new QRData(info);

            QRModulesShop.PlaceVersion(qr);
            QRModulesShop.PlaceFinderPatterns(qr);
            QRModulesShop.PlaceSeparators(qr);
            QRModulesShop.PlaceAlignmentPatterns(qr);
            QRModulesShop.PlaceTimingPatterns(qr);
            QRModulesShop.PlaceDarkModule(qr);
            QRModulesShop.ReserveFormatAreas(qr);

            QRModulesShop.PlaceDataWords(qr, interleavedData);

            qr = _masker.MaskCode(qr);
            qr.AddQuietZone();

            return qr;
        }

        protected QREncodingMode oDetectEncoding(string plainText, bool forceUtf8)
        {
            // Détermine l'encodage en fonction des caractères à écrire dans le QRCode

            List<char> alphaNums = _alphaNumEncValues.Keys.ToList();

            QREncodingMode result;
            if (forceUtf8)
                result = QREncodingMode.Byte;
            else if (plainText.ToList().TrueForAll(c => _numChars.Contains(c)))
                result = QREncodingMode.Numeric;
            else if (plainText.ToList().TrueForAll(c => alphaNums.Contains(c)))
                result = QREncodingMode.Alphanumeric;
            else
                result = QREncodingMode.Byte;

            return result;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _masker.Dispose();

            base.oDispose(isExplicit);
        }

        protected int oGetDataLength(QREncodingMode encoding, string plainText, string codeText, bool forceUtf8)
        {
            return (forceUtf8 || zIsUtf8(encoding, plainText) ? codeText.Length / 8 : plainText.Length);
        }

        protected QRPolynom oMultiplyAlphaPolynoms(QRPolynom polynomBase, QRPolynom polynomMultiplier)
        {
            QRPolynom resultPolynom = new QRPolynom();

            foreach (QRPolynom.Item polItemBase in polynomMultiplier.PolyItems)
                foreach (QRPolynom.Item polItemMulti in polynomBase.PolyItems)
                {
                    QRPolynom.Item polItemRes = new QRPolynom.Item(zShrinkAlphaExp(polItemBase.coefficient + polItemMulti.coefficient), polItemBase.exponent + polItemMulti.exponent);
                    resultPolynom.PolyItems.Add(polItemRes);
                }

            List<int> exponentsToGlue = resultPolynom.PolyItems.GroupBy(x => x.exponent).Where(x => x.Count() > 1).Select(x => x.First().exponent).ToList();
            List<QRPolynom.Item> gluedPolynoms = new List<QRPolynom.Item>();

            foreach (int exponent in exponentsToGlue)
            {
                int coeff = resultPolynom.PolyItems.Where(x => x.exponent == exponent).Aggregate(0, (i, polynomOld) => i ^ zGetIntValFromAlphaExp(_galoisField, polynomOld.coefficient));
                QRPolynom.Item polynomFixed = new QRPolynom.Item(zGetAlphaExpFromIntVal(_galoisField, coeff), exponent);
                gluedPolynoms.Add(polynomFixed);
            }

            resultPolynom.PolyItems.RemoveAll(x => exponentsToGlue.Contains(x.exponent));
            resultPolynom.PolyItems.AddRange(gluedPolynoms);
            resultPolynom.PolyItems.SortRegarding(resultPolynom.PolyItems.ConvertAll(o => -o.exponent));

            return resultPolynom;
        }

        protected string oTextToBinary(string plainText, QREncodingMode encMode, bool utf8BOM, bool forceUtf8)
        {
            string output = encMode switch
            {
                QREncodingMode.Numeric => zPlainTextToBinaryNumeric(plainText),
                QREncodingMode.Alphanumeric => zPlainTextToBinaryAlphanumeric(plainText, _alphaNumEncValues),
                QREncodingMode.Byte => zPlainTextToBinaryByte(plainText, utf8BOM, forceUtf8),
                _ => "",
            };

            return output;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zAdjustBitStringLength(string bitString, int dataLength)
        {
            int lengthDiff = dataLength - bitString.Length;

            if (lengthDiff > 0) bitString += new string('0', Math.Min(lengthDiff, 4));

            if (bitString.Length % 8 != 0) bitString += new string('0', 8 - bitString.Length % 8);

            while (bitString.Length < dataLength)
                bitString += "1110110000010001";

            if (bitString.Length > dataLength) bitString = bitString.Substring(0, dataLength);

            return bitString;
        }

        private static List<int> zBinaryStringListToDecList(List<string> binaryStringList)
        {
            return binaryStringList.Select(s => zBinToDec(s)).ToList();
        }

        private static List<string> zBinaryStringToBitBlockList(string bitString)
        {
            List<string> output = new List<string>();

            while (bitString.Length >= 8)
            {
                output.Add(bitString.Substring(0, 8));
                bitString = bitString.Substring(8);
            }

            return output;
        }

        private static int zBinToDec(string binStr)
        {
            return Convert.ToInt32(binStr, 2);
        }

        private static QRPolynom zCalculateMessagePolynom(string bitString)
        {
            QRPolynom messagePol = new QRPolynom();
            int i = bitString.Length / 8 - 1;

            while (i >= 0)
            {
                messagePol.PolyItems.Add(new QRPolynom.Item(zBinToDec(bitString.Substring(0, 8)), i));
                bitString = bitString.Remove(0, 8);
                i--;
            }

            return messagePol;
        }

        private static void zCheckCapacityIsValid(xQRConfigInfos strengthDetails, QREncodingMode encoding, int dataLength)
        {
            if (strengthDetails.capacity[encoding] < dataLength)
                throw new ArgumentException("The forced version " + strengthDetails.version.ToNameString() + " of QRCode can not encode " + dataLength.ToString() + " characters. The maximum is " + strengthDetails.capacity[encoding].ToString() + ".");
        }

        private static void zCheckVersionIsValid(QRVersion version, int dataLength)
        {
            if ((int)version == -1)
                throw new ArgumentException("Too many characters (" + dataLength.ToString() + ") to create the QRCode.");
        }

        private static int zGetAlphaExpFromIntVal(List<xQRAntilog> galoisField, int intVal)
        {
            return galoisField.First(alog => alog.integerValue == intVal).exponentAlpha;
        }

        private static int zGetCountIndicatorLength(QRVersion version, QREncodingMode encMode)
        {
            int output;

            // Doc : Page "Data Encoding", Step 4

            if ((int)version < 10)
            {
                if (encMode.Equals(QREncodingMode.Numeric))
                    output = 10;
                else if (encMode.Equals(QREncodingMode.Alphanumeric))
                    output = 9;
                else
                    output = 8;
            }
            else if ((int)version < 27)
            {
                if (encMode.Equals(QREncodingMode.Numeric))
                    output = 12;
                else if (encMode.Equals(QREncodingMode.Alphanumeric))
                    output = 11;
                else if (encMode.Equals(QREncodingMode.Byte))
                    output = 16;
                else
                    output = 10;
            }
            else if (encMode.Equals(QREncodingMode.Numeric))
                output = 14;
            else if (encMode.Equals(QREncodingMode.Alphanumeric))
                output = 13;
            else if (encMode.Equals(QREncodingMode.Byte))
                output = 16;
            else
                output = 12;

            return output;
        }

        private static int zGetIntValFromAlphaExp(List<xQRAntilog> galoisField, int exp)
        {
            return galoisField.First(alog => alog.exponentAlpha == exp).integerValue;
        }

        private static bool zIsUtf8(QREncodingMode encoding, string plainText)
        {
            return encoding == QREncodingMode.Byte && !zIsValidISO(plainText);
        }

        private static bool zIsValidISO(string input)
        {
            byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
            string result = Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, bytes.Length);

            return string.Equals(input, result);
        }

        private static QRPolynom zMultiplyGeneratorPolynomByLeadterm(QRPolynom genPolynom, QRPolynom.Item leadTerm, int lowerExponentBy)
        {
            QRPolynom resultPolynom = new QRPolynom();

            foreach (QRPolynom.Item polItemBase in genPolynom.PolyItems)
            {
                QRPolynom.Item polItemRes = new QRPolynom.Item((polItemBase.coefficient + leadTerm.coefficient) % 255, polItemBase.exponent - lowerExponentBy);
                resultPolynom.PolyItems.Add(polItemRes);
            }

            return resultPolynom;
        }

        private static string zPlainTextToBinaryAlphanumeric(string plainText, Dictionary<char, int> alphaNumEnc)
        {
            string codeText = "";

            while (plainText.Length >= 2)
            {
                string token = plainText.Substring(0, 2);
                int dec = alphaNumEnc[token[0]] * 45 + alphaNumEnc[token[1]];
                codeText += zzDecToBin(dec, 11);
                plainText = plainText.Substring(2);
            }

            if (plainText.Length > 0)
                codeText += zzDecToBin(alphaNumEnc[plainText[0]], 6);

            return codeText;
        }

        private static string zPlainTextToBinaryByte(string plainText, bool utf8Bom, bool forceUtf8)
        {
            string codeText = "";

            byte[] codeBytes;
            if (zIsValidISO(plainText) && !forceUtf8)
                codeBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(plainText);
            else
                codeBytes = utf8Bom ? Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(plainText)).ToArray() : Encoding.UTF8.GetBytes(plainText);

            foreach (byte b in codeBytes)
                codeText += zzDecToBin(b, 8);

            return codeText;
        }

        private static string zPlainTextToBinaryNumeric(string plainText)
        {
            string codeText = "";

            while (plainText.Length >= 3)
            {
                int dec = Convert.ToInt32(plainText.Substring(0, 3));
                codeText += zzDecToBin(dec, 10);
                plainText = plainText.Substring(3);
            }

            if (plainText.Length == 2)
            {
                int dec = Convert.ToInt32(plainText.Substring(0, plainText.Length));
                codeText += zzDecToBin(dec, 7);
            }
            else if (plainText.Length == 1)
            {
                int dec = Convert.ToInt32(plainText.Substring(0, plainText.Length));
                codeText += zzDecToBin(dec, 4);
            }

            return codeText;
        }

        private static int zShrinkAlphaExp(int alphaExp)
        {
            return alphaExp % 256 + (alphaExp / 256d).ToFloorInt();
        }

        private static QRPolynom zXORPolynoms(QRPolynom messagePolynom, QRPolynom resPolynom)
        {
            QRPolynom resultPolynom = new QRPolynom();

            QRPolynom longPoly, shortPoly;
            if (messagePolynom.PolyItems.Count >= resPolynom.PolyItems.Count)
            {
                longPoly = messagePolynom;
                shortPoly = resPolynom;
            }
            else
            {
                longPoly = resPolynom;
                shortPoly = messagePolynom;
            }

            for (int i = 0; i < longPoly.PolyItems.Count; i++)
            {
                QRPolynom.Item polItemRes = new QRPolynom.Item(longPoly.PolyItems[i].coefficient ^ (shortPoly.PolyItems.Count > i ? shortPoly.PolyItems[i].coefficient : 0), messagePolynom.PolyItems[0].exponent - i);
                resultPolynom.PolyItems.Add(polItemRes);
            }

            resultPolynom.PolyItems.RemoveAt(0);

            return resultPolynom;
        }

        private static string zzDecToBin(int decNum, int padLeftUpTo)
        {
            return Convert.ToString(decNum, 2).PadLeft(padLeftUpTo, '0');
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}