using System;
using System.Collections.Generic;
using System.Text;

using Microvision.Types;

namespace Microvision.QRCoder
{
    internal class QRMasker : Citizen
    {
        // ***************************************************************************************************
        // 15.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private delegate bool MaskDelegate(int x, int y);

        // ***************************************************************************************************

        private List<MaskDelegate> _masks;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRMasker()
        {
            _masks = new List<MaskDelegate>();
            _masks.Add(zMask1);
            _masks.Add(zMask2);
            _masks.Add(zMask3);
            _masks.Add(zMask4);
            _masks.Add(zMask5);
            _masks.Add(zMask6);
            _masks.Add(zMask7);
            _masks.Add(zMask8);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public QRData MaskCode(QRData qrCode)
        {
            int bestPenalty = int.MaxValue;
            QRData bestResult = null;

            for (int iMask = 0; iMask < _masks.Count; iMask++)
            {
                string format = zGetFormatString(qrCode.Strength, iMask);
                QRModulesShop.PlaceFormat(qrCode, format);

                QRData qrMask = zCreateQrMask(_masks[iMask], qrCode.Infos);
                QRData newQrCode = zApplyMask(new QRData(qrCode), qrMask);
                qrMask.Dispose();

                int maskPenalty = QRPenaltyShop.CalcPenalty(newQrCode);
                if (maskPenalty < bestPenalty)
                {
                    bestPenalty = maskPenalty;
                    bestResult?.Dispose();
                    bestResult = newQrCode;
                }
                else
                {
                    newQrCode.Dispose();
                }
            }

            return bestResult;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _masks.Clear();

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static QRData zApplyMask(QRData original, QRData mask)
        {
            for (int x = 0; x < original.Width; x++)
                for (int y = 0; y < original.Width; y++)
                    if (!original.IsLocked(x, y))
                        original.SetPixelToMask(y, x, original.GetPixel(y, x) ^ mask.GetPixel(x, y));

            return original;
        }

        private static QRData zCreateQrMask(MaskDelegate mask, xQRConfigInfos info)
        {
            QRData qrMask = new QRData(info);
            
            for (int x = 0; x < qrMask.Width; x++)
                for (int y = 0; y < qrMask.Width; y++)
                    qrMask.SetPixel(x, y, mask(x, y));

            return qrMask;
        }

        private static string zDecToBin(int decNum)
        {
            return Convert.ToString(decNum, 2);
        }

        private static string zDecToBin(int decNum, int padLeftUpTo)
        {
            return zDecToBin(decNum).PadLeft(padLeftUpTo, '0');
        }

        private static string zGetFormatString(QRStrength strength, int maskVersion)
        {
            string generator = "10100110111";
            string mask = "101010000010010";

            string strengthString = strength switch
            {
                QRStrength.Low => "01",
                QRStrength.Middle => "00",
                QRStrength.Quality => "11",
                QRStrength.HighQuality => "10",
                _ => "01"
            };

            strengthString += zDecToBin(maskVersion, 3);
            string s = strengthString.PadRight(15, '0').TrimStart('0');

            while (s.Length > 10)
            {
                generator = generator.PadRight(s.Length, '0');
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < s.Length; i++)
                    sb.Append((Convert.ToInt32(s[i]) ^ Convert.ToInt32(generator[i])).ToString());

                s = sb.ToString().TrimStart('0');
            }

            s = s.PadLeft(10, '0');
            strengthString += s;

            StringBuilder sbFormat = new StringBuilder();
            for (int i = 0; i < strengthString.Length; i++)
                sbFormat.Append((Convert.ToInt32(strengthString[i]) ^ Convert.ToInt32(mask[i])).ToString());

            return sbFormat.ToString();
        }

        private bool zMask1(int x, int y)
        {
            return (x + y) % 2 == 0;
        }

        private bool zMask2(int x, int y)
        {
            return y % 2 == 0;
        }

        private bool zMask3(int x, int y)
        {
            return x % 3 == 0;
        }

        private bool zMask4(int x, int y)
        {
            return (x + y) % 3 == 0;
        }

        private bool zMask5(int x, int y)
        {
            return (Math.Floor(y / 2f) + Math.Floor(x / 3f)).ToRoundInt() % 2 == 0;
        }

        private bool zMask6(int x, int y)
        {
            return x * y % 2 + x * y % 3 == 0;
        }

        private bool zMask7(int x, int y)
        {
            return (x * y % 2 + x * y % 3) % 2 == 0;
        }

        private bool zMask8(int x, int y)
        {
            return ((x + y) % 2 + x * y % 3) % 2 == 0;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}