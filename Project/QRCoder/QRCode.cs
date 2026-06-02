using System.Drawing;

using Microvision.Geometry;
using Microvision.Types;

namespace Microvision.QRCoder
{
    public class QRCode : Citizen
    {
        // ***************************************************************************************************
        // 16.02.18 : Création
        // 21.11.19 : (libs 2.2) Correction prise en compte du paramètre strength
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly QRData _data;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRCode(string message, QRStrength strength = QRStrength.Quality, int forcedVersion = -1)
        {
            using QRGenerator generator = new QRGenerator();
            _data = generator.CreateQrCode(message, strength, forcedVersion);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public Bitmap GetBitmap(SizeI maxSize, QRCodeGraphics? customGraphics = null)
        {
            if (customGraphics is null)
                customGraphics = zCreateDefaultGraphics();
            else
                customGraphics.AddLife();

            Bitmap img = customGraphics.GenerateBitmap(_data, maxSize);
            customGraphics.Dispose();

            return img;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _data.Dispose();

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static QRCodeGraphics zCreateDefaultGraphics()
        {
            QRCodeGraphics g = new QRCodeGraphics();
            g.FrontColor = Color.Black;
            g.BackColor = Color.White;
            g.RoundedPixels = false;
            g.Icon = null;
            g.WithGradient = false;

            return g;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}