namespace Microvision.QRCoder
{
    internal static class QRPenaltyShop
    {
        // ***************************************************************************************************
        // 15.02.18 : Création
        // 21.11.19 : (libs 2.2) NotInheritable
        // 14.04.22 : (libs 3.0)
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

        public static int CalcPenalty(QRData qrCode)
        {
            int penalty1 = zCalcPenalty1(qrCode);
            int penalty2 = zCalcPenalty2(qrCode);
            int penalty3 = zCalcPenalty3(qrCode);
            int penalty4 = zCalcPenalty4(qrCode);

            return penalty1 + penalty2 + penalty3 + penalty4;
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

        private static int zCalcPenalty1(QRData qrCode)
        {
            // Doc : Page "Data masking", paragraphe "Evaluation Condition #1"
            // Pénalité de 3 sur les suites de 5 valeurs identiques, et +1 par valeur identique supplémentaire

            int penalty = 0;

            int size = qrCode.Width;
            for (int x = 0; x < size; x++)
            {
                int count = 0;
                bool lastVal = qrCode.GetPixel(x, 0);
                for (int y = 0; y < size; y++)
                {
                    if (qrCode.GetPixel(x, y) == lastVal)
                        count++;
                    else
                        count = 1;

                    if (count == 5)
                        penalty += 3;
                    else if (count > 5)
                        penalty++;

                    lastVal = qrCode.GetPixel(x, y);
                }
            }

            for (int y = 0; y < size; y++)
            {
                int count = 0;
                bool lastVal = qrCode.GetPixel(0, y);
                for (int x = 0; x < size; x++)
                {
                    if (qrCode.GetPixel(x, y) == lastVal)
                        count++;
                    else
                        count = 1;

                    if (count == 5)
                        penalty += 3;
                    else if (count > 5)
                        penalty++;

                    lastVal = qrCode.GetPixel(x, y);
                }
            }

            return penalty;
        }

        private static int zCalcPenalty2(QRData qrCode)
        {
            // Doc : Page "Data masking", paragraphe "Evaluation Condition #2"
            // Pénalité de 3 sur les patterns de 4 true ou false en carré

            int size = qrCode.Width;

            int penalty = 0;

            for (int y = 0; y < size - 1; y++)
                for (int x = 0; x < size - 1; x++)
                    if (qrCode.GetPixel(y, x) == qrCode.GetPixel(y, x + 1) &&
                        qrCode.GetPixel(y, x) == qrCode.GetPixel(y + 1, x) &&
                        qrCode.GetPixel(y, x) == qrCode.GetPixel(y + 1, x + 1))
                        penalty += 3;

            return penalty;
        }

        private static int zCalcPenalty3(QRData qrCode)
        {
            // Doc : Page "Data masking", paragraphe "Evaluation Condition #3"
            // Pénalité de 40 sur les patterns " . . . . # . ### . #"

            int penalty = 0;
            int size = qrCode.Width;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size - 10; x++)
                {
                    if (qrCode.GetPixel(y, x) && !qrCode.GetPixel(y, x + 1) && qrCode.GetPixel(y, x + 2) && qrCode.GetPixel(y, x + 3) && qrCode.GetPixel(y, x + 4) && !qrCode.GetPixel(y, x + 5) && qrCode.GetPixel(y, x + 6) && !qrCode.GetPixel(y, x + 7) && !qrCode.GetPixel(y, x + 8) && !qrCode.GetPixel(y, x + 9) && !qrCode.GetPixel(y, x + 10) || !qrCode.GetPixel(y, x) && !qrCode.GetPixel(y, x + 1) && !qrCode.GetPixel(y, x + 2) && !qrCode.GetPixel(y, x + 3) && qrCode.GetPixel(y, x + 4) && !qrCode.GetPixel(y, x + 5) && qrCode.GetPixel(y, x + 6) && qrCode.GetPixel(y, x + 7) && qrCode.GetPixel(y, x + 8) && !qrCode.GetPixel(y, x + 9) && qrCode.GetPixel(y, x + 10))
                        penalty += 40;

                    if (qrCode.GetPixel(x, y) && !qrCode.GetPixel(x + 1, y) && qrCode.GetPixel(x + 2, y) && qrCode.GetPixel(x + 3, y) && qrCode.GetPixel(x + 4, y) && !qrCode.GetPixel(x + 5, y) && qrCode.GetPixel(x + 6, y) && !qrCode.GetPixel(x + 7, y) && !qrCode.GetPixel(x + 8, y) && !qrCode.GetPixel(x + 9, y) && !qrCode.GetPixel(x + 10, y) || !qrCode.GetPixel(x, y) && !qrCode.GetPixel(x + 1, y) && !qrCode.GetPixel(x + 2, y) && !qrCode.GetPixel(x + 3, y) && qrCode.GetPixel(x + 4, y) && !qrCode.GetPixel(x + 5, y) && qrCode.GetPixel(x + 6, y) && qrCode.GetPixel(x + 7, y) && qrCode.GetPixel(x + 8, y) && !qrCode.GetPixel(x + 9, y) && qrCode.GetPixel(x + 10, y))
                        penalty += 40;
                }
            }

            return penalty;
        }

        private static int zCalcPenalty4(QRData qrCode)
        {
            // Doc : Page "Data masking", paragraphe "Evaluation Condition #4"
            // Pénalité en fonction de l'équart de propertion modules blanc/ modules noir

            int blackModules = 0;

            for (int x = 0; x < qrCode.Width; x++)
                for (int y = 0; y < qrCode.Width; y++)
                    if (qrCode.GetPixel(x, y))
                        blackModules++;

            float percent = (float)blackModules / (qrCode.Width * qrCode.Width) * 100;
            int prevMultipleOf5 = Math.Abs((percent / 5).ToFloorInt() * 5 - 50) / 5;
            int nextMultipleOf5 = Math.Abs((percent / 5).ToFloorInt() * 5 - 45) / 5;
            int penalty = Math.Min(prevMultipleOf5, nextMultipleOf5) * 10;

            return penalty;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}