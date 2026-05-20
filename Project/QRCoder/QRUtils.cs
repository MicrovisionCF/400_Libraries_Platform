using System.Collections.Generic;

using Microvision.Geometry;

namespace Microvision.QRCoder
{
    internal struct xQRCodewordBlock
    {
        public int groupNumber;
        public int blockNumber;
        public string bitString;
        public List<string> codeWords;
        public List<int> codeWordsInt;
        public List<string> words;
        public List<int> wordsInt;

        public xQRCodewordBlock(int groupNumber, int blockNumber, string bitString, List<string> codeWords, List<string> words, List<int> codeWordsInt, List<int> wordsInt)
        {
            this.groupNumber = groupNumber;
            this.blockNumber = blockNumber;
            this.bitString = bitString;
            this.codeWords = codeWords;
            this.words = words;
            this.codeWordsInt = codeWordsInt;
            this.wordsInt = wordsInt;
        }
    }

    internal struct xQRConfigInfos
    {
        public QRVersion version;
        public QRStrength strength;
        public int reminderBits;
        public PointIs alignementPositions;
        public int wordsPerBlock;
        public int blocksInGroup1;
        public int codewordsInGroup1;
        public int blocksInGroup2;
        public int codewordsInGroup2;
        public Dictionary<QREncodingMode, int> capacity;

        public int GetTotalDataCodewords()
        {
            return blocksInGroup1 * codewordsInGroup1 + blocksInGroup2 * codewordsInGroup2;
        }
    }

    internal struct xQRAntilog
    {
        public int exponentAlpha;
        public int integerValue;

        public xQRAntilog(int exponentAlpha, int integerValue)
        {
            this.exponentAlpha = exponentAlpha;
            this.integerValue = integerValue;
        }
    }

    public enum QRStrength
    {
        Low, // Error correction level L : Accepte 7% de perte
        Middle, // Error correction level M : Accepte 15% de perte
        Quality, // Error correction level Q : Accepte 25% de perte
        HighQuality // Error correction level H : Accepte 30% de perte
    }

    public enum QRVersion
    {
        // Juste pour que ce soit plus clair quand c'est une version et pas juste un entier quelconque, en particulier dans les tables
        v1 = 1,
        v2,
        v3,
        v4,
        v5,
        v6,
        v7,
        v8,
        v9,
        v10,
        v11,
        v12,
        v13,
        v14,
        v15,
        v16,
        v17,
        v18,
        v19,
        v20,
        v21,
        v22,
        v23,
        v24,
        v25,
        v26,
        v27,
        v28,
        v29,
        v30,
        v31,
        v32,
        v33,
        v34,
        v35,
        v36,
        v37,
        v38,
        v39,
        v40
    }

    internal enum QREncodingMode
    {
        Numeric = 1,
        Alphanumeric = 2,
        Byte = 4,
        Kanji = 8
    }

    internal class QRConfigsInfos : Dictionary<QRVersion, Dictionary<QRStrength, xQRConfigInfos>>
    {
        // ***************************************************************************************************
        // 13.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 09.07.25 : Correction possibilité d'utiliser la version taille max
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public QRVersion ChooseVersion(int length, QREncodingMode encMode, QRStrength strength)
        {
            bool found = false;
            QRVersion version = (QRVersion)1;

            while ((int)version <= Count && !found)
            {
                if (this[version][strength].capacity[encMode] >= length)
                    found = true;
                else
                    version = version + 1;
            }

            if (!found) version = (QRVersion)(-1);

            return version;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}