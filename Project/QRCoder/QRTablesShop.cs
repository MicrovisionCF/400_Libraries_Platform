using System;
using System.Collections.Generic;
using System.Linq;

using Microvision.Geometry;

namespace Microvision.QRCoder
{
    internal static class QRTablesShop
    {
        // ***************************************************************************************************
        // 13.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private struct xQRStrengthInfo
        {
            public QRVersion version;
            public QRStrength strength;
            public int wordsPerBlock;
            public int blocksInGroup1;
            public int codewordsInGroup1;
            public int blocksInGroup2;
            public int codewordsInGroup2;

            public xQRStrengthInfo(QRVersion version, QRStrength strength, int wordsPerBlock, int blocksInGroup1, int codewordsInGroup1, int blocksInGroup2, int codewordsInGroup2)
            {
                this.version = version;
                this.strength = strength;
                this.wordsPerBlock = wordsPerBlock;
                this.blocksInGroup1 = blocksInGroup1;
                this.codewordsInGroup1 = codewordsInGroup1;
                this.blocksInGroup2 = blocksInGroup2;
                this.codewordsInGroup2 = codewordsInGroup2;
            }
        }


        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static Dictionary<char, int> CreateAlphaNumEncValues()
        {
            // Détermine l'association caractère / valeur qui le représente
            // Doc : Alphanumeric table

            Dictionary<char, int> values = [];
            zCreateAlphaNumEncChars().ForEach(c => values.Add(c, values.Count));

            return values;
        }

        public static List<xQRAntilog> CreateAntilogTable()
        {
            // Dans la doc : Page "Error correction coding", Step 5

            List<xQRAntilog> galoisField = [];
            for (int i = 0; i < 256; i++)
            {
                int item;

                if (i > 7)
                    item = galoisField[i - 1].integerValue * 2;
                else
                    item = (int)Math.Pow(2, i);

                if (item > 255) item ^= 285;

                galoisField.Add(new xQRAntilog(i, item));
            }

            return galoisField;
        }

        public static QRConfigsInfos CreateCapacities()
        {
            QRConfigsInfos capacities = [];

            Dictionary<QREncodingMode, Dictionary<QRStrength, List<int>>> capacityBaseValues = zCreateCapacityBaseValues();
            List<xQRStrengthInfo> strengthInfos = zCreateStrengthsCapacities();
            Dictionary<QRVersion, int> reminderBits = zCreateReminderBits();
            Dictionary<QRVersion, PointIs> alignmentPos = zCreateAlignementPatterns();

            foreach (QRVersion version in Enum.GetValues<QRVersion>())
            {
                capacities.Add(version, []);

                for (int strength = 0; strength <= 4 - 1; strength++)
                {
                    xQRConfigInfos details = new xQRConfigInfos();
                    details.strength = (QRStrength)strength;
                    details.version = version;
                    details.reminderBits = reminderBits[version];
                    details.alignementPositions = alignmentPos[version];

                    details.capacity = [];
                    foreach (QREncodingMode encoding in Enum.GetValues<QREncodingMode>())
                        details.capacity.Add(encoding, capacityBaseValues[encoding][(QRStrength)strength][(int)version - 1]);

                    xQRStrengthInfo info = strengthInfos.First(o => o.version == version && (int)o.strength == strength);
                    details.blocksInGroup1 = info.blocksInGroup1;
                    details.blocksInGroup2 = info.blocksInGroup2;
                    details.codewordsInGroup1 = info.codewordsInGroup1;
                    details.codewordsInGroup2 = info.codewordsInGroup2;
                    details.wordsPerBlock = info.wordsPerBlock;

                    capacities[version].Add((QRStrength)strength, details);
                }
            }

            return capacities;
        }

        public static List<PointIs> CreateFormatPositions(int width)
        {
            // Détermine la position de l'écriture du format dans le QRCode. Le format est écrit 2 fois d'où les 2 listes
            // Doc : Page "Format and Version Information", Chapitre "Put the Format String into the QR Code"

            List<PointIs> pos = [
            [
                (8, 0),
                (8, 1),
                (8, 2),
                (8, 3),
                (8, 4),
                (8, 5),
                (8, 7),
                (8, 8),
                (7, 8),
                (5, 8),
                (4, 8),
                (3, 8),
                (2, 8),
                (1, 8),
                (0, 8)
            ],
            [
                (width - 1, 8),
                (width - 2, 8),
                (width - 3, 8),
                (width - 4, 8),
                (width - 5, 8),
                (width - 6, 8),
                (width - 7, 8),
                (width - 8, 8),
                (8, width - 7),
                (8, width - 6),
                (8, width - 5),
                (8, width - 4),
                (8, width - 3),
                (8, width - 2),
                (8, width - 1)
            ]];

            return pos;
        }

        public static List<char> CreateNumChars()
        {
            return [.. "0123456789"];
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Dictionary<QRVersion, PointIs> zCreateAlignementPatterns()
        {
            Dictionary<QRVersion, List<int>> positions = zCreateAlignmentCenters();
            Dictionary<QRVersion, PointIs> patterns = [];

            foreach (QRVersion version in Enum.GetValues<QRVersion>())
            {
                PointIs points = [];
                positions[version].ForEach(x => positions[version].ForEach(y => points.Add(new PointI(x - 2, y - 2))));
                patterns.Add(version, points);
            }

            return patterns;
        }

        private static Dictionary<QRVersion, List<int>> zCreateAlignmentCenters()
        {
            // Détermine l'emplacement des patterns d'alignement suivant la taille du QRCode
            // La liste retournée sont les coordonnées à multiplexer pour obtenir les positions
            // Exemple : [6, 18] représente les points (6:6) (6:18) (18:6) (16:16)

            // Doc : Page "Module placement matrix" Step 3

            Dictionary<QRVersion, List<int>> positions;
            positions = new Dictionary<QRVersion, List<int>>
            {
                { (QRVersion)1, new List<int>() },
                { (QRVersion)2, new List<int> { 6, 18 } },
                { (QRVersion)3, new List<int> { 6, 22 } },
                { (QRVersion)4, new List<int> { 6, 26 } },
                { (QRVersion)5, new List<int> { 6, 30 } },
                { (QRVersion)6, new List<int> { 6, 34 } },
                { (QRVersion)7, new List<int> { 6, 22, 38 } },
                { (QRVersion)8, new List<int> { 6, 24, 42 } },
                { (QRVersion)9, new List<int> { 6, 26, 46 } },
                { (QRVersion)10, new List<int> { 6, 28, 50 } },
                { (QRVersion)11, new List<int> { 6, 30, 54 } },
                { (QRVersion)12, new List<int> { 6, 32, 58 } },
                { (QRVersion)13, new List<int> { 6, 34, 62 } },
                { (QRVersion)14, new List<int> { 6, 26, 46, 66 } },
                { (QRVersion)15, new List<int> { 6, 26, 48, 70 } },
                { (QRVersion)16, new List<int> { 6, 26, 50, 74 } },
                { (QRVersion)17, new List<int> { 6, 30, 54, 78 } },
                { (QRVersion)18, new List<int> { 6, 30, 56, 82 } },
                { (QRVersion)19, new List<int> { 6, 30, 58, 86 } },
                { (QRVersion)20, new List<int> { 6, 34, 62, 90 } },
                { (QRVersion)21, new List<int> { 6, 28, 50, 72, 94 } },
                { (QRVersion)22, new List<int> { 6, 26, 50, 74, 98 } },
                { (QRVersion)23, new List<int> { 6, 30, 54, 78, 102 } },
                { (QRVersion)24, new List<int> { 6, 28, 54, 80, 106 } },
                { (QRVersion)25, new List<int> { 6, 32, 58, 84, 110 } },
                { (QRVersion)26, new List<int> { 6, 30, 58, 86, 114 } },
                { (QRVersion)27, new List<int> { 6, 34, 62, 90, 118 } },
                { (QRVersion)28, new List<int> { 6, 26, 50, 74, 98, 122 } },
                { (QRVersion)29, new List<int> { 6, 30, 54, 78, 102, 126 } },
                { (QRVersion)30, new List<int> { 6, 26, 52, 78, 104, 130 } },
                { (QRVersion)31, new List<int> { 6, 30, 56, 82, 108, 134 } },
                { (QRVersion)32, new List<int> { 6, 34, 60, 86, 112, 138 } },
                { (QRVersion)33, new List<int> { 6, 30, 58, 86, 114, 142 } },
                { (QRVersion)34, new List<int> { 6, 34, 62, 90, 118, 146 } },
                { (QRVersion)35, new List<int> { 6, 30, 54, 78, 102, 126, 150 } },
                { (QRVersion)36, new List<int> { 6, 24, 50, 76, 102, 128, 154 } },
                { (QRVersion)37, new List<int> { 6, 28, 54, 80, 106, 132, 158 } },
                { (QRVersion)38, new List<int> { 6, 32, 58, 84, 110, 136, 162 } },
                { (QRVersion)39, new List<int> { 6, 26, 54, 82, 110, 138, 166 } },
                { (QRVersion)40, new List<int> { 6, 30, 58, 86, 114, 142, 170 } }
            };

            return positions;
        }

        private static List<char> zCreateAlphaNumEncChars()
        {
            // Liste des caractères encodables en version alpha numérique
            // Doc : Page "Data Analysis", Chapitre "The QR Code Modes", Lien "alphanumeric table"

            return [.. "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:"];
        }

        private static Dictionary<QREncodingMode, Dictionary<QRStrength, List<int>>> zCreateCapacityBaseValues()
        {
            // Détermine la capacité selon le type de données, le niveau de robustesse et la version
            // La table ne semble pas pouvoir être calculée dynamiquement elle est donc crée de façon statique

            // Doc : Appendices / Character Capacities by Version, Mode, and Error Correction

            Dictionary<QREncodingMode, Dictionary<QRStrength, List<int>>> output = new Dictionary<QREncodingMode, Dictionary<QRStrength, List<int>>>
            {
                {
                    QREncodingMode.Numeric,
                    new Dictionary<QRStrength, List<int>>
                    {
                        { QRStrength.Low, [41, 77, 127, 187, 255, 322, 370, 461, 552, 652, 772, 883, 1022, 1101, 1250, 1408, 1548, 1725, 1903, 2061, 2232, 2409, 2620, 2812, 3057, 3283, 3517, 3669, 3909, 4158, 4417, 4686, 4965, 5253, 5529, 5836, 6153, 6479, 6743, 7089] },
                        { QRStrength.Middle, [34, 63, 101, 149, 202, 255, 293, 365, 432, 513, 604, 691, 796, 871, 991, 1082, 1212, 1346, 1500, 1600, 1708, 1872, 2059, 2188, 2395, 2544, 2701, 2857, 3035, 3289, 3486, 3693, 3909, 4134, 4343, 4588, 4775, 5039, 5313, 5596] },
                        { QRStrength.Quality, [27, 48, 77, 111, 144, 178, 207, 259, 312, 364, 427, 489, 580, 621, 703, 775, 876, 948, 1063, 1159, 1224, 1358, 1468, 1588, 1718, 1804, 1933, 2085, 2181, 2358, 2473, 2670, 2805, 2949, 3081, 3244, 3417, 3599, 3791, 3993] },
                        { QRStrength.HighQuality, [17, 34, 58, 82, 106, 139, 154, 202, 235, 288, 331, 374, 427, 468, 530, 602, 674, 746, 813, 919, 969, 1056, 1108, 1228, 1286, 1425, 1501, 1581, 1677, 1782, 1897, 2022, 2157, 2301, 2361, 2524, 2625, 2735, 2927, 3057] }
                    }
                },
                {
                    QREncodingMode.Alphanumeric,
                    new Dictionary<QRStrength, List<int>>
                    {
                        { QRStrength.Low, [25, 47, 77, 114, 154, 195, 224, 279, 335, 395, 468, 535, 619, 667, 758, 854, 938, 1046, 1153, 1249, 1352, 1460, 1588, 1704, 1853, 1990, 2132, 2223, 2369, 2520, 2677, 2840, 3009, 3183, 3351, 3537, 3729, 3927, 4087, 4296] },
                        { QRStrength.Middle, [20, 38, 61, 90, 122, 154, 178, 221, 262, 311, 366, 419, 483, 528, 600, 656, 734, 816, 909, 970, 1035, 1134, 1248, 1326, 1451, 1542, 1637, 1732, 1839, 1994, 2113, 2238, 2369, 2506, 2632, 2780, 2894, 3054, 3220, 3391]},
                        { QRStrength.Quality, [16, 29, 47, 67, 87, 108, 125, 157, 189, 221, 259, 296, 352, 376, 426, 470, 531, 574, 644, 702, 742, 823, 890, 963, 1041, 1094, 1172, 1263, 1322, 1429, 1499, 1618, 1700, 1787, 1867, 1966, 2071, 2181, 2298, 2420]},
                        { QRStrength.HighQuality, [10, 20, 35, 50, 64, 84, 93, 122, 143, 174, 200, 227, 259, 283, 321, 365, 408, 452, 493, 557, 587, 640, 672, 744, 779, 864, 910, 958, 1016, 1080, 1150, 1226, 1307, 1394, 1431, 1530, 1591, 1658, 1774, 1852]}
                    }
                },
                {
                    QREncodingMode.Byte,
                    new Dictionary<QRStrength, List<int>>
                    {
                        { QRStrength.Low, [17, 32, 53, 78, 106, 134, 154, 192, 230, 271, 321, 367, 425, 458, 520, 586, 644, 718, 792, 858, 929, 1003, 1091, 1171, 1273, 1367, 1465, 1528, 1628, 1732, 1840, 1952, 2068, 2188, 2303, 2431, 2563, 2699, 2809, 2953]},
                        {QRStrength.Middle, [14, 26, 42, 62, 84, 106, 122, 152, 180, 213, 251, 287, 331, 362, 412, 450, 504, 560, 624, 666, 711, 779, 857, 911, 997, 1059, 1125, 1190, 1264, 1370, 1452, 1538, 1628, 1722, 1809, 1911, 1989, 2099, 2213, 2331]},
                        {QRStrength.Quality, [11, 20, 32, 46, 60, 74, 86, 108, 130, 151, 177, 203, 241, 258, 292, 322, 364, 394, 442, 482, 509, 565, 611, 661, 715, 751, 805, 868, 908, 982, 1030, 1112, 1168, 1228, 1283, 1351, 1423, 1499, 1579, 1663]},
                        { QRStrength.HighQuality, [7, 14, 24, 34, 44, 58, 64, 84, 98, 119, 137, 155, 177, 194, 220, 250, 280, 310, 338, 382, 403, 439, 461, 511, 535, 593, 625, 658, 698, 742, 790, 842, 898, 958, 983, 1051, 1093, 1139, 1219, 1273]}
                    }
                },
                {
                    QREncodingMode.Kanji,
                    new Dictionary<QRStrength, List<int>>
                    {
                        {QRStrength.Low, [10, 20, 32, 48, 65, 82, 95, 118, 141, 167, 198, 226, 262, 282, 320, 361, 397, 442, 488, 528, 572, 618, 672, 721, 784, 842, 902, 940, 1002, 1066, 1132, 1201, 1273, 1347, 1417, 1496, 1577, 1661, 1729, 1817]},
                        {QRStrength.Middle, [8, 16, 26, 38, 52, 65, 75, 93, 111, 131, 155, 177, 204, 223, 254, 277, 310, 345, 384, 410, 438, 480, 528, 561, 614, 652, 692, 732, 778, 843, 894, 947, 1002, 1060, 1113, 1176, 1224, 1292, 1362, 1435]},
                        {QRStrength.Quality, [7, 12, 20, 28, 37, 45, 53, 66, 80, 93, 109, 125, 149, 159, 180, 198, 224, 243, 272, 297, 314, 348, 376, 407, 440, 462, 496, 534, 559, 604, 634, 684, 719, 756, 790, 832, 876, 923, 972, 1024]},
                        { QRStrength.HighQuality, [4, 8, 15, 21, 27, 36, 39, 52, 60, 74, 85, 96, 109, 120, 136, 154, 173, 191, 208, 235, 248, 270, 284, 315, 330, 365, 385, 405, 430, 457, 486, 518, 553, 590, 605, 647, 673, 701, 750, 784]}
                    }
                }
            };

            return output;
        }

        private static Dictionary<QRVersion, int> zCreateReminderBits()
        {
            // Détermine le nombre de bits à ajouter selon la taille pour compléter le message
            // Doc : Page "Structure final message", Step 4

            Dictionary<QRVersion, int> bits = new Dictionary<QRVersion, int>
            {
                { (QRVersion)1, 0 },
                { (QRVersion)2, 7 },
                { (QRVersion)3, 7 },
                { (QRVersion)4, 7 },
                { (QRVersion)5, 7 },
                { (QRVersion)6, 7 },
                { (QRVersion)7, 0 },
                { (QRVersion)8, 0 },
                { (QRVersion)9, 0 },
                { (QRVersion)10, 0 },
                { (QRVersion)11, 0 },
                { (QRVersion)12, 0 },
                { (QRVersion)13, 0 },
                { (QRVersion)14, 3 },
                { (QRVersion)15, 3 },
                { (QRVersion)16, 3 },
                { (QRVersion)17, 3 },
                { (QRVersion)18, 3 },
                { (QRVersion)19, 3 },
                { (QRVersion)20, 3 },
                { (QRVersion)21, 4 },
                { (QRVersion)22, 4 },
                { (QRVersion)23, 4 },
                { (QRVersion)24, 4 },
                { (QRVersion)25, 4 },
                { (QRVersion)26, 4 },
                { (QRVersion)27, 4 },
                { (QRVersion)28, 3 },
                { (QRVersion)29, 3 },
                { (QRVersion)30, 3 },
                { (QRVersion)31, 3 },
                { (QRVersion)32, 3 },
                { (QRVersion)33, 3 },
                { (QRVersion)34, 3 },
                { (QRVersion)35, 0 },
                { (QRVersion)36, 0 },
                { (QRVersion)37, 0 },
                { (QRVersion)38, 0 },
                { (QRVersion)39, 0 },
                { (QRVersion)40, 0 }
            };
            return bits;
        }

        private static List<xQRStrengthInfo> zCreateStrengthsCapacities()
        {
            // Détermine les capacité en fonction de la taille et de la correction
            // Doc : Appendices / Error Correction Code Words and Block Information

            List<xQRStrengthInfo> infos =
            [
                new xQRStrengthInfo((QRVersion)1, QRStrength.Low, 7, 1, 19, 0, 0),
                new xQRStrengthInfo((QRVersion)1, QRStrength.Middle, 10, 1, 16, 0, 0),
                new xQRStrengthInfo((QRVersion)1, QRStrength.Quality, 13, 1, 13, 0, 0),
                new xQRStrengthInfo((QRVersion)1, QRStrength.HighQuality, 17, 1, 9, 0, 0),
                
                new xQRStrengthInfo((QRVersion)2, QRStrength.Low, 10, 1, 34, 0, 0),
                new xQRStrengthInfo((QRVersion)2, QRStrength.Middle, 16, 1, 28, 0, 0),
                new xQRStrengthInfo((QRVersion)2, QRStrength.Quality, 22, 1, 22, 0, 0),
                new xQRStrengthInfo((QRVersion)2, QRStrength.HighQuality, 28, 1, 16, 0, 0),
                
                new xQRStrengthInfo((QRVersion)3, QRStrength.Low, 15, 1, 55, 0, 0),
                new xQRStrengthInfo((QRVersion)3, QRStrength.Middle, 26, 1, 44, 0, 0),
                new xQRStrengthInfo((QRVersion)3, QRStrength.Quality, 18, 2, 17, 0, 0),
                new xQRStrengthInfo((QRVersion)3, QRStrength.HighQuality, 22, 2, 13, 0, 0),
                
                new xQRStrengthInfo((QRVersion)4, QRStrength.Low, 20, 1, 80, 0, 0),
                new xQRStrengthInfo((QRVersion)4, QRStrength.Middle, 18, 2, 32, 0, 0),
                new xQRStrengthInfo((QRVersion)4, QRStrength.Quality, 26, 2, 24, 0, 0),
                new xQRStrengthInfo((QRVersion)4, QRStrength.HighQuality, 16, 4, 9, 0, 0),
                
                new xQRStrengthInfo((QRVersion)5, QRStrength.Low, 26, 1, 108, 0, 0),
                new xQRStrengthInfo((QRVersion)5, QRStrength.Middle, 24, 2, 43, 0, 0),
                new xQRStrengthInfo((QRVersion)5, QRStrength.Quality, 18, 2, 15, 2, 16),
                new xQRStrengthInfo((QRVersion)5, QRStrength.HighQuality, 22, 2, 11, 2, 12),
                
                new xQRStrengthInfo((QRVersion)6, QRStrength.Low, 18, 2, 68, 0, 0),
                new xQRStrengthInfo((QRVersion)6, QRStrength.Middle, 16, 4, 27, 0, 0),
                new xQRStrengthInfo((QRVersion)6, QRStrength.Quality, 24, 4, 19, 0, 0),
                new xQRStrengthInfo((QRVersion)6, QRStrength.HighQuality, 28, 4, 15, 0, 0),
                
                new xQRStrengthInfo((QRVersion)7, QRStrength.Low, 20, 2, 78, 0, 0),
                new xQRStrengthInfo((QRVersion)7, QRStrength.Middle, 18, 4, 31, 0, 0),
                new xQRStrengthInfo((QRVersion)7, QRStrength.Quality, 18, 2, 14, 4, 15),
                new xQRStrengthInfo((QRVersion)7, QRStrength.HighQuality, 26, 4, 13, 1, 14),
                
                new xQRStrengthInfo((QRVersion)8, QRStrength.Low, 24, 2, 97, 0, 0),
                new xQRStrengthInfo((QRVersion)8, QRStrength.Middle, 22, 2, 38, 2, 39),
                new xQRStrengthInfo((QRVersion)8, QRStrength.Quality, 22, 4, 18, 2, 19),
                new xQRStrengthInfo((QRVersion)8, QRStrength.HighQuality, 26, 4, 14, 2, 15),
                
                new xQRStrengthInfo((QRVersion)9, QRStrength.Low, 30, 2, 116, 0, 0),
                new xQRStrengthInfo((QRVersion)9, QRStrength.Middle, 22, 3, 36, 2, 37),
                new xQRStrengthInfo((QRVersion)9, QRStrength.Quality, 20, 4, 16, 4, 17),
                new xQRStrengthInfo((QRVersion)9, QRStrength.HighQuality, 24, 4, 12, 4, 13),
                
                new xQRStrengthInfo((QRVersion)10, QRStrength.Low, 18, 2, 68, 2, 69),
                new xQRStrengthInfo((QRVersion)10, QRStrength.Middle, 26, 4, 43, 1, 44),
                new xQRStrengthInfo((QRVersion)10, QRStrength.Quality, 24, 6, 19, 2, 20),
                new xQRStrengthInfo((QRVersion)10, QRStrength.HighQuality, 28, 6, 15, 2, 16),
                
                new xQRStrengthInfo((QRVersion)11, QRStrength.Low, 20, 4, 81, 0, 0),
                new xQRStrengthInfo((QRVersion)11, QRStrength.Middle, 30, 1, 50, 4, 51),
                new xQRStrengthInfo((QRVersion)11, QRStrength.Quality, 28, 4, 22, 4, 23),
                new xQRStrengthInfo((QRVersion)11, QRStrength.HighQuality, 24, 3, 12, 8, 13),
                
                new xQRStrengthInfo((QRVersion)12, QRStrength.Low, 24, 2, 92, 2, 93),
                new xQRStrengthInfo((QRVersion)12, QRStrength.Middle, 22, 6, 36, 2, 37),
                new xQRStrengthInfo((QRVersion)12, QRStrength.Quality, 26, 4, 20, 6, 21),
                new xQRStrengthInfo((QRVersion)12, QRStrength.HighQuality, 28, 7, 14, 4, 15),
                
                new xQRStrengthInfo((QRVersion)13, QRStrength.Low, 26, 4, 107, 0, 0),
                new xQRStrengthInfo((QRVersion)13, QRStrength.Middle, 22, 8, 37, 1, 38),
                new xQRStrengthInfo((QRVersion)13, QRStrength.Quality, 24, 8, 20, 4, 21),
                new xQRStrengthInfo((QRVersion)13, QRStrength.HighQuality, 22, 12, 11, 4, 12),
                
                new xQRStrengthInfo((QRVersion)14, QRStrength.Low, 30, 3, 115, 1, 116),
                new xQRStrengthInfo((QRVersion)14, QRStrength.Middle, 24, 4, 40, 5, 41),
                new xQRStrengthInfo((QRVersion)14, QRStrength.Quality, 20, 11, 16, 5, 17),
                new xQRStrengthInfo((QRVersion)14, QRStrength.HighQuality, 24, 11, 12, 5, 13),
                
                new xQRStrengthInfo((QRVersion)15, QRStrength.Low, 22, 5, 87, 1, 88),
                new xQRStrengthInfo((QRVersion)15, QRStrength.Middle, 24, 5, 41, 5, 42),
                new xQRStrengthInfo((QRVersion)15, QRStrength.Quality, 30, 5, 24, 7, 25),
                new xQRStrengthInfo((QRVersion)15, QRStrength.HighQuality, 24, 11, 12, 7, 13), 
                
                new xQRStrengthInfo((QRVersion)16, QRStrength.Low, 24, 5, 98, 1, 99),
                new xQRStrengthInfo((QRVersion)16, QRStrength.Middle, 28, 7, 45, 3, 46),
                new xQRStrengthInfo((QRVersion)16, QRStrength.Quality, 24, 15, 19, 2, 20),
                new xQRStrengthInfo((QRVersion)16, QRStrength.HighQuality, 30, 3, 15, 13, 16), 
                
                new xQRStrengthInfo((QRVersion)17, QRStrength.Low, 28, 1, 107, 5, 108),
                new xQRStrengthInfo((QRVersion)17, QRStrength.Middle, 28, 10, 46, 1, 47),
                new xQRStrengthInfo((QRVersion)17, QRStrength.Quality, 28, 1, 22, 15, 23),
                new xQRStrengthInfo((QRVersion)17, QRStrength.HighQuality, 28, 2, 14, 17, 15), 
                
                new xQRStrengthInfo((QRVersion)18, QRStrength.Low, 30, 5, 120, 1, 121),
                new xQRStrengthInfo((QRVersion)18, QRStrength.Middle, 26, 9, 43, 4, 44),
                new xQRStrengthInfo((QRVersion)18, QRStrength.Quality, 28, 17, 22, 1, 23),
                new xQRStrengthInfo((QRVersion)18, QRStrength.HighQuality, 28, 2, 14, 19, 15), 
                
                new xQRStrengthInfo((QRVersion)19, QRStrength.Low, 28, 3, 113, 4, 114),
                new xQRStrengthInfo((QRVersion)19, QRStrength.Middle, 26, 3, 44, 11, 45),
                new xQRStrengthInfo((QRVersion)19, QRStrength.Quality, 26, 17, 21, 4, 22),
                new xQRStrengthInfo((QRVersion)19, QRStrength.HighQuality, 26, 9, 13, 16, 14), 
                
                new xQRStrengthInfo((QRVersion)20, QRStrength.Low, 28, 3, 107, 5, 108),
                new xQRStrengthInfo((QRVersion)20, QRStrength.Middle, 26, 3, 41, 13, 42),
                new xQRStrengthInfo((QRVersion)20, QRStrength.Quality, 30, 15, 24, 5, 25),
                new xQRStrengthInfo((QRVersion)20, QRStrength.HighQuality, 28, 15, 15, 10, 16), 
                
                new xQRStrengthInfo((QRVersion)21, QRStrength.Low, 28, 4, 116, 4, 117),
                new xQRStrengthInfo((QRVersion)21, QRStrength.Middle, 26, 17, 42, 0, 0),
                new xQRStrengthInfo((QRVersion)21, QRStrength.Quality, 28, 17, 22, 6, 23),
                new xQRStrengthInfo((QRVersion)21, QRStrength.HighQuality, 30, 19, 16, 6, 17), 
                
                new xQRStrengthInfo((QRVersion)22, QRStrength.Low, 28, 2, 111, 7, 112),
                new xQRStrengthInfo((QRVersion)22, QRStrength.Middle, 28, 17, 46, 0, 0),
                new xQRStrengthInfo((QRVersion)22, QRStrength.Quality, 30, 7, 24, 16, 25),
                new xQRStrengthInfo((QRVersion)22, QRStrength.HighQuality, 24, 34, 13, 0, 0), 
                
                new xQRStrengthInfo((QRVersion)23, QRStrength.Low, 30, 4, 121, 5, 122),
                new xQRStrengthInfo((QRVersion)23, QRStrength.Middle, 28, 4, 47, 14, 48),
                new xQRStrengthInfo((QRVersion)23, QRStrength.Quality, 30, 11, 24, 14, 25),
                new xQRStrengthInfo((QRVersion)23, QRStrength.HighQuality, 30, 16, 15, 14, 16), 
                
                new xQRStrengthInfo((QRVersion)24, QRStrength.Low, 30, 6, 117, 4, 118),
                new xQRStrengthInfo((QRVersion)24, QRStrength.Middle, 28, 6, 45, 14, 46),
                new xQRStrengthInfo((QRVersion)24, QRStrength.Quality, 30, 11, 24, 16, 25),
                new xQRStrengthInfo((QRVersion)24, QRStrength.HighQuality, 30, 30, 16, 2, 17), 
                
                new xQRStrengthInfo((QRVersion)25, QRStrength.Low, 26, 8, 106, 4, 107),
                new xQRStrengthInfo((QRVersion)25, QRStrength.Middle, 28, 8, 47, 13, 48),
                new xQRStrengthInfo((QRVersion)25, QRStrength.Quality, 30, 7, 24, 22, 25),
                new xQRStrengthInfo((QRVersion)25, QRStrength.HighQuality, 30, 22, 15, 13, 16), 
                
                new xQRStrengthInfo((QRVersion)26, QRStrength.Low, 28, 10, 114, 2, 115),
                new xQRStrengthInfo((QRVersion)26, QRStrength.Middle, 28, 19, 46, 4, 47),
                new xQRStrengthInfo((QRVersion)26, QRStrength.Quality, 28, 28, 22, 6, 23),
                new xQRStrengthInfo((QRVersion)26, QRStrength.HighQuality, 30, 33, 16, 4, 17), 
                
                new xQRStrengthInfo((QRVersion)27, QRStrength.Low, 30, 8, 122, 4, 123),
                new xQRStrengthInfo((QRVersion)27, QRStrength.Middle, 28, 22, 45, 3, 46),
                new xQRStrengthInfo((QRVersion)27, QRStrength.Quality, 30, 8, 23, 26, 24),
                new xQRStrengthInfo((QRVersion)27, QRStrength.HighQuality, 30, 12, 15, 28, 16), 
                
                new xQRStrengthInfo((QRVersion)28, QRStrength.Low, 30, 3, 117, 10, 118),
                new xQRStrengthInfo((QRVersion)28, QRStrength.Middle, 28, 3, 45, 23, 46),
                new xQRStrengthInfo((QRVersion)28, QRStrength.Quality, 30, 4, 24, 31, 25),
                new xQRStrengthInfo((QRVersion)28, QRStrength.HighQuality, 30, 11, 15, 31, 16), 
                
                new xQRStrengthInfo((QRVersion)29, QRStrength.Low, 30, 7, 116, 7, 117),
                new xQRStrengthInfo((QRVersion)29, QRStrength.Middle, 28, 21, 45, 7, 46),
                new xQRStrengthInfo((QRVersion)29, QRStrength.Quality, 30, 1, 23, 37, 24),
                new xQRStrengthInfo((QRVersion)29, QRStrength.HighQuality, 30, 19, 15, 26, 16), 
                
                new xQRStrengthInfo((QRVersion)30, QRStrength.Low, 30, 5, 115, 10, 116),
                new xQRStrengthInfo((QRVersion)30, QRStrength.Middle, 28, 19, 47, 10, 48),
                new xQRStrengthInfo((QRVersion)30, QRStrength.Quality, 30, 15, 24, 25, 25),
                new xQRStrengthInfo((QRVersion)30, QRStrength.HighQuality, 30, 23, 15, 25, 16), 
                
                new xQRStrengthInfo((QRVersion)31, QRStrength.Low, 30, 13, 115, 3, 116),
                new xQRStrengthInfo((QRVersion)31, QRStrength.Middle, 28, 2, 46, 29, 47),
                new xQRStrengthInfo((QRVersion)31, QRStrength.Quality, 30, 42, 24, 1, 25),
                new xQRStrengthInfo((QRVersion)31, QRStrength.HighQuality, 30, 23, 15, 28, 16), 
                
                new xQRStrengthInfo((QRVersion)32, QRStrength.Low, 30, 17, 115, 0, 0),
                new xQRStrengthInfo((QRVersion)32, QRStrength.Middle, 28, 10, 46, 23, 47),
                new xQRStrengthInfo((QRVersion)32, QRStrength.Quality, 30, 10, 24, 35, 25),
                new xQRStrengthInfo((QRVersion)32, QRStrength.HighQuality, 30, 19, 15, 35, 16), 
                
                new xQRStrengthInfo((QRVersion)33, QRStrength.Low, 30, 17, 115, 1, 116),
                new xQRStrengthInfo((QRVersion)33, QRStrength.Middle, 28, 14, 46, 21, 47),
                new xQRStrengthInfo((QRVersion)33, QRStrength.Quality, 30, 29, 24, 19, 25),
                new xQRStrengthInfo((QRVersion)33, QRStrength.HighQuality, 30, 11, 15, 46, 16), 
                
                new xQRStrengthInfo((QRVersion)34, QRStrength.Low, 30, 13, 115, 6, 116),
                new xQRStrengthInfo((QRVersion)34, QRStrength.Middle, 28, 14, 46, 23, 47),
                new xQRStrengthInfo((QRVersion)34, QRStrength.Quality, 30, 44, 24, 7, 25),
                new xQRStrengthInfo((QRVersion)34, QRStrength.HighQuality, 30, 59, 16, 1, 17), 
                
                new xQRStrengthInfo((QRVersion)35, QRStrength.Low, 30, 12, 121, 7, 122),
                new xQRStrengthInfo((QRVersion)35, QRStrength.Middle, 28, 12, 47, 26, 48),
                new xQRStrengthInfo((QRVersion)35, QRStrength.Quality, 30, 39, 24, 14, 25),
                new xQRStrengthInfo((QRVersion)35, QRStrength.HighQuality, 30, 22, 15, 41, 16), 
                
                new xQRStrengthInfo((QRVersion)36, QRStrength.Low, 30, 6, 121, 14, 122),
                new xQRStrengthInfo((QRVersion)36, QRStrength.Middle, 28, 6, 47, 34, 48),
                new xQRStrengthInfo((QRVersion)36, QRStrength.Quality, 30, 46, 24, 10, 25),
                new xQRStrengthInfo((QRVersion)36, QRStrength.HighQuality, 30, 2, 15, 64, 16), 
                
                new xQRStrengthInfo((QRVersion)37, QRStrength.Low, 30, 17, 122, 4, 123),
                new xQRStrengthInfo((QRVersion)37, QRStrength.Middle, 28, 29, 46, 14, 47),
                new xQRStrengthInfo((QRVersion)37, QRStrength.Quality, 30, 49, 24, 10, 25),
                new xQRStrengthInfo((QRVersion)37, QRStrength.HighQuality, 30, 24, 15, 46, 16), 
                
                new xQRStrengthInfo((QRVersion)38, QRStrength.Low, 30, 4, 122, 18, 123),
                new xQRStrengthInfo((QRVersion)38, QRStrength.Middle, 28, 13, 46, 32, 47),
                new xQRStrengthInfo((QRVersion)38, QRStrength.Quality, 30, 48, 24, 14, 25),
                new xQRStrengthInfo((QRVersion)38, QRStrength.HighQuality, 30, 42, 15, 32, 16), 
                
                new xQRStrengthInfo((QRVersion)39, QRStrength.Low, 30, 20, 117, 4, 118),
                new xQRStrengthInfo((QRVersion)39, QRStrength.Middle, 28, 40, 47, 7, 48),
                new xQRStrengthInfo((QRVersion)39, QRStrength.Quality, 30, 43, 24, 22, 25),
                new xQRStrengthInfo((QRVersion)39, QRStrength.HighQuality, 30, 10, 15, 67, 16), 
                
                new xQRStrengthInfo((QRVersion)40, QRStrength.Low, 30, 19, 118, 6, 119),
                new xQRStrengthInfo((QRVersion)40, QRStrength.Middle, 28, 18, 47, 31, 48),
                new xQRStrengthInfo((QRVersion)40, QRStrength.Quality, 30, 34, 24, 34, 25),
                new xQRStrengthInfo((QRVersion)40, QRStrength.HighQuality, 30, 20, 15, 61, 16),
            ];

            return infos;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}