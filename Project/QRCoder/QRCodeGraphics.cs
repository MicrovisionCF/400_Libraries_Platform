using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.Types;

namespace Microvision.QRCoder
{
    public class QRCodeGraphics : Citizen
    {
        // ***************************************************************************************************
        // 16.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private HColor _frontColor, _backColor;
        private bool _roundedPixels;
        private Bitmap? _icon;
        private int _iconSizePercent;
        private int _iconBorderPercent;
        private bool _withGradient;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRCodeGraphics()
        {
            _frontColor = Color.Black;
            _backColor = Color.White;
            _roundedPixels = false;
            _icon = null;
            _iconSizePercent = 15;
            _iconBorderPercent = 15;
            _withGradient = false;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public HColor BackColor
        {
            get => _backColor;

            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                }
            }
        }

        public HColor FrontColor
        {
            get => _frontColor;

            set
            {
                if (_frontColor != value)
                {
                    _frontColor = value;
                }
            }
        }

        public Bitmap? Icon
        {
            get => _icon;

            set
            {
                if (_icon != value)
                {
                    _icon = value;
                }
            }
        }

        public int IconBorderPercent
        {
            get => _iconBorderPercent;

            set
            {
                if (_iconBorderPercent != value)
                {
                    _iconBorderPercent = value;
                }
            }
        }

        public int IconSizePercent
        {
            get => _iconSizePercent;

            set
            {
                if (_iconSizePercent != value)
                {
                    _iconSizePercent = value;
                }
            }
        }

        public bool RoundedPixels
        {
            get => _roundedPixels;

            set
            {
                if (_roundedPixels != value)
                {
                    _roundedPixels = value;
                }
            }
        }

        public bool WithGradient
        {
            get => _withGradient;

            set
            {
                if (_withGradient != value)
                {
                    _withGradient = value;
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        internal Bitmap GenerateBitmap(QRData data, SizeI maxSize)
        {
            int w = (maxSize.Width / (double)data.Width).ToFloorInt();
            int h = (maxSize.Height / (double)data.Width).ToFloorInt();
            int pixelSize = Math.Min(w, h);
            pixelSize = Math.Max(1, pixelSize);
            Bitmap bmp = GenerateBitmap(data, pixelSize);

            return bmp;
        }

        internal Bitmap GenerateBitmap(QRData data, int pixelWidth)
        {
            int size = data.Width * pixelWidth;

            Bitmap bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics gfx = Graphics.FromImage(bmp);

            gfx.Clear(BackColor);

            Brush lightBrush = new SolidBrush(_backColor);
            Brush darkBrush;
            if (_withGradient)
                darkBrush = new LinearGradientBrush(new PointI(0, 0), new PointI(bmp.Width, bmp.Height), HColor.Darker(_frontColor, 0.2f), HColor.Lighter(_frontColor, 0.2f));
            else
                darkBrush = new SolidBrush(_frontColor);

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Width; y++)
                {
                    if (data.GetPixel(y, x))
                    {
                        RectI rect = new RectI(x * pixelWidth, y * pixelWidth, pixelWidth, pixelWidth);
                        if (_roundedPixels && pixelWidth > 2)
                        {
                            gfx.SmoothingMode = SmoothingMode.AntiAlias;
                            if (pixelWidth >= 6)
                                rect = RectI.Inflate(rect, ((float)pixelWidth / 6).ToRoundInt(), ((float)pixelWidth / 6).ToRoundInt());
                            else
                                rect = new RectI(rect.X, rect.Y, rect.Width + 1, rect.Height + 1);

                            gfx.FillEllipse(darkBrush, rect);
                        }
                        else
                        {
                            gfx.SmoothingMode = SmoothingMode.None;
                            gfx.FillRectangle(darkBrush, rect);
                        }
                    }
                }
            }

            if (_icon is not null)
            {
                float iconDestWidth = _iconSizePercent * bmp.Width / 100f;
                float iconDestHeight = iconDestWidth * _icon.Height / _icon.Width;
                float iconX = (bmp.Width - iconDestWidth) / 2;
                float iconY = (bmp.Height - iconDestHeight) / 2;

                if (_iconBorderPercent > 0)
                {
                    gfx.SmoothingMode = SmoothingMode.AntiAlias;
                    gfx.FillEllipse(lightBrush, RectG.Inflate(new RectG(iconX, iconY, iconDestWidth, iconDestHeight), iconDestWidth / 100 * _iconBorderPercent, iconDestHeight / 100 * _iconBorderPercent));
                }

                gfx.DrawImage(_icon, iconX, iconY, iconDestWidth, iconDestHeight);
            }

            lightBrush.Dispose();
            darkBrush.Dispose();
            gfx.Save();

            return bmp;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _icon = null;
            base.oDispose(isExplicit);
        }


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