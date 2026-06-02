using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microvision.Geometry;

namespace Microvision.QRCoder
{
    internal static class QRModulesShop
    {
        // ***************************************************************************************************
        // 16.02.18 : Création
        // 21.11.19 : (libs 2.2) NotInheritable
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static void PlaceAlignmentPatterns(QRData qrCode)
        {
            // Doc : Page "Module Placement in Matrix", Step 3

            foreach (PointI loc in qrCode.AlignmentPositions)
            {
                RectI alignmentPatternRect = new RectI(loc.X, loc.Y, 5, 5);
                bool blocked = zIsBlocked(alignmentPatternRect, qrCode);
                if (!blocked)
                {
                    zPlaceAlignment(qrCode, alignmentPatternRect.Location);
                }
            }
        }

        public static void PlaceDarkModule(QRData qrCode)
        {
            // Doc : Page "Module Placement in Matrix", Step 5
            qrCode.SetPixel(qrCode.Width - 8, 8, true);
        }

        public static void PlaceDataWords(QRData qrCode, string data)
        {
            // Doc : Page "Module Placement in Matrix", Step 6

            int size = qrCode.Width;
            bool up = true;
            Queue<bool> datawords = new Queue<bool>();

            for (int i = 0; i < data.Length; i++)
                datawords.Enqueue(data[i] != '0');

            int x = size - 1;

            while (x >= 0)
            {
                if (x == 6) x = 5; // Saut du timing vertical

                for (int yMod = 1; yMod <= size; yMod++)
                {
                    if (up)
                    {
                        int y = size - yMod;
                        if (datawords.Count > 0 && !qrCode.IsLocked(x, y)) qrCode.SetPixelToMask(y, x, datawords.Dequeue());
                        if (datawords.Count > 0 && x > 0 && !qrCode.IsLocked(x - 1, y)) qrCode.SetPixelToMask(y, x - 1, datawords.Dequeue());
                    }
                    else
                    {
                        int y = yMod - 1;
                        if (datawords.Count > 0 && !qrCode.IsLocked(x, y)) qrCode.SetPixelToMask(y, x, datawords.Dequeue());
                        if (datawords.Count > 0 && x > 0 && !qrCode.IsLocked(x - 1, y)) qrCode.SetPixelToMask(y, x - 1, datawords.Dequeue());
                    }
                }

                up = !up;
                x -= 2;
            }
        }

        public static void PlaceFinderPatterns(QRData qrCode)
        {
            // Doc : Page "Module Placement in Matrix", Step 1

            int size = qrCode.Width;
            PointIs locations =
            [
                (0, 0),
                (size - 7, 0),
                (0, size - 7)
            ];

            foreach (PointI loc in locations)
                zPlaceFinder(qrCode, loc);
        }

        public static void PlaceFormat(QRData qrCode, string formatStr)
        {
            int size = qrCode.Width;
            string fStr = new string([.. formatStr.Reverse()]);

            List<PointIs> positions = QRTablesShop.CreateFormatPositions(size);

            foreach (PointIs pos in positions)
                for (int i = 0; i <= 15 - 1; i++)
                    qrCode.SetPixel(pos[i].Y, pos[i].X, fStr[i] == '1');
        }

        public static void PlaceSeparators(QRData qrCode)
        {
            // Doc : Page "Module Placment Matrix", Step 2

            PointIs locations =
            [
                (-1, -1),
                (qrCode.Width - 8, -1),
                (-1, qrCode.Width - 8)
            ];

            zPlaceSeparator(qrCode, locations[0]);
            zPlaceSeparator(qrCode, locations[1]);
            zPlaceSeparator(qrCode, locations[2]);
        }

        public static void PlaceTimingPatterns(QRData qrCode)
        {
            // Doc : Page "Module Placement in Matrix", Step 4

            for (int i = 8; i < qrCode.Width - 8; i++)
            {
                qrCode.SetPixel(6, i, i % 2 == 0);
                qrCode.SetPixel(i, 6, i % 2 == 0);
            }
        }

        public static void PlaceVersion(QRData qrCode)
        {
            if ((int)qrCode.Version >= 7)
            {
                string versionString = zGetVersionString(qrCode.Version);
                int size = qrCode.Width;
                string vStr = new string([.. versionString.Reverse()]);
                
                for (int x = 0; x <= 6 - 1; x++)
                    for (int y = 0; y <= 3 - 1; y++)
                    {
                        qrCode.SetPixel(y + size - 11, x, vStr[x * 3 + y] == '1');
                        qrCode.SetPixel(x, y + size - 11, vStr[x * 3 + y] == '1');
                    }
            }
        }

        public static void ReserveFormatAreas(QRData qr)
        {
            qr.SetPixel(0, 8, false);
            qr.SetPixel(1, 8, false);
            qr.SetPixel(2, 8, false);
            qr.SetPixel(3, 8, false);
            qr.SetPixel(4, 8, false);
            qr.SetPixel(5, 8, false);
            qr.SetPixel(7, 8, false);
            qr.SetPixel(8, 8, false);
            qr.SetPixel(8, 7, false);
            qr.SetPixel(8, 5, false);
            qr.SetPixel(8, 4, false);
            qr.SetPixel(8, 3, false);
            qr.SetPixel(8, 2, false);
            qr.SetPixel(8, 1, false);
            qr.SetPixel(8, 0, false);

            qr.SetPixel(qr.Width - 1, 8, false);
            qr.SetPixel(qr.Width - 2, 8, false);
            qr.SetPixel(qr.Width - 3, 8, false);
            qr.SetPixel(qr.Width - 4, 8, false);
            qr.SetPixel(qr.Width - 5, 8, false);
            qr.SetPixel(qr.Width - 6, 8, false);
            qr.SetPixel(qr.Width - 7, 8, false);
            qr.SetPixel(8, qr.Width - 8, false);
            qr.SetPixel(8, qr.Width - 7, false);
            qr.SetPixel(8, qr.Width - 6, false);
            qr.SetPixel(8, qr.Width - 5, false);
            qr.SetPixel(8, qr.Width - 4, false);
            qr.SetPixel(8, qr.Width - 3, false);
            qr.SetPixel(8, qr.Width - 2, false);
            qr.SetPixel(8, qr.Width - 1, false);
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

        private static string zDecToBin(int decNum)
        {
            return Convert.ToString(decNum, 2);
        }

        private static string zDecToBin(int decNum, int padLeftUpTo)
        {
            return zDecToBin(decNum).PadLeft(padLeftUpTo, '0');
        }

        private static string zGetVersionString(QRVersion version)
        {
            string generator = "1111100100101";
            string s = zDecToBin((int)version, 6);
            string strength = s.PadRight(18, '0').TrimStart('0');

            while (strength.Length > 12)
            {
                StringBuilder sb = new StringBuilder();
                generator = generator.PadRight(strength.Length, '0');

                for (int i = 0; i < strength.Length; i++)
                    sb.Append(Convert.ToInt32(strength[i]) ^ Convert.ToInt32(generator[i]));

                strength = sb.ToString().TrimStart('0');
            }

            strength = strength.PadLeft(12, '0');
            s += strength;

            return s;
        }

        private static bool zIsBlocked(RectI rct, QRData qrCode)
        {
            bool blocked = false;

            for (int x = 0; x < rct.Width; x++)
                for (int y = 0; y < rct.Height; y++)
                    blocked = blocked || qrCode.IsLocked(x + rct.X, y + rct.Y);

            return blocked;
        }

        private static void zPlaceAlignment(QRData qrCode, PointI pos)
        {
            // # # # # # Carré de 5 noir
            // #       # Carré de 3 blanc
            // #   #   # Carré de 1 noir
            // #       #
            // # # # # #

            zzPlaceSquare(qrCode, new PointI(pos.X + 0, pos.Y + 0), 5, true);
            zzPlaceSquare(qrCode, new PointI(pos.X + 1, pos.Y + 1), 3, false);
            zzPlaceSquare(qrCode, new PointI(pos.X + 2, pos.Y + 2), 1, true);
        }

        private static void zPlaceFinder(QRData qrCode, PointI pos)
        {
            // # # # # # # #  Carré de 7 noir
            // #           #  Carré de 5 blanc
            // #   # # #   #  Carré de 3 noir
            // #   # # #   #  Carré de 1 noir
            // #   # # #   # 
            // #           # 
            // # # # # # # # 

            zzPlaceSquare(qrCode, new PointI(pos.X + 0, pos.Y + 0), 7, true);
            zzPlaceSquare(qrCode, new PointI(pos.X + 1, pos.Y + 1), 5, false);
            zzPlaceSquare(qrCode, new PointI(pos.X + 2, pos.Y + 2), 3, true);
            zzPlaceSquare(qrCode, new PointI(pos.X + 3, pos.Y + 3), 1, true);
        }

        private static void zPlaceSeparator(QRData qrCode, PointI pos)
        {
            // Carré blanc de 9 autour des finder
            zzPlaceSquare(qrCode, new PointI(pos.X, pos.Y), 9, false);
        }

        private static void zzPlaceSquare(QRData qrCode, PointI pos, int size, bool mark)
        {
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    if (x == 0 || y == 0 || x == size - 1 || y == size - 1)
                    {
                        int globalX = pos.X + x;
                        int globalY = pos.Y + y;

                        if (globalX >= 0 && globalY >= 0 && globalX < qrCode.Width && globalY < qrCode.Width)
                            qrCode.SetPixel(globalX, globalY, mark);
                    }
                }
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}