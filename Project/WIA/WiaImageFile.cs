using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using Microvision.Geometry;
using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaImageFile : Citizen
    {
        // ***************************************************************************************************
        // 11.02.13 : création
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly WIA.ImageFile _imageFile;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaImageFile(WIA.ImageFile image) : base()
        {
            _imageFile = image;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string FormatID => _imageFile.FormatID;

        public int PropertiesCount => _imageFile.Properties.Count;

        public PointG Resolution => new PointG((float)_imageFile.HorizontalResolution, (float)_imageFile.VerticalResolution); // dpi

        public SizeI Size => new SizeI(_imageFile.Width, _imageFile.Height);


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return $"{pfx}{GetType().Name} = {zDebugImageFile(this, pfx)}";
        }

        public int FindProperty(string propertyName)
        {
            return zFindProperty(propertyName, _imageFile.Properties.ToList());
        }

        public Bitmap GetBitmap()
        {
            // -- pas d'usage de BasicBitmap parce que je sais pas encore dans quelle librairie cet objet va aboutir.

            FileName fileName = FileName.GetTempFileName(_imageFile.FileExtension);

            _imageFile.SaveFile(fileName);
            using Bitmap tmp = new Bitmap(fileName);

            Bytes bytes = new Bytes(zGetDataBytesCount(tmp));
            zGetDataBytes(tmp, bytes, 0);
            Bitmap bmp = zCreateCoreBitmap(tmp.Size, tmp.PixelFormat, tmp.Palette);
            zSetDataBytes(bytes, 0, bmp);

            File.Delete(fileName);

            return bmp;
        }

        public WiaProperty GetProperty(int no)
        {
            return new WiaProperty(_imageFile.Properties.ToList()[no]);
        }

        public void SaveFile(FileName fileName)
        {
            _imageFile.SaveFile(fileName);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_imageFile);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Bitmap zCreateCoreBitmap(SizeI size, PixelFormat format, ColorPalette palette)
        {
            // -- fonction pompée sur BasicDibMng

            Bitmap bmp = new Bitmap(size.Width, size.Height, format);

            if (format.HasFlag(PixelFormat.Indexed) && palette is not null)
                bmp.Palette = palette;

            return bmp;
        }

        private static string zDebugImageFile(WiaImageFile image, string pfx)
        {
            string ch = "";
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Résolution : " + image.Resolution.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Taille : " + image.Size.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Format : " + image.FormatID.ToString();

            for (int i = 0; i < image.PropertiesCount; i++)
            {
                using WiaProperty prp = image.GetProperty(i);
                ch = ch + SpecialChars.NewLine + prp.DebugString(pfx + SpecialChars.Tab);
            }

            return ch;
        }

        private static int zFindProperty(string propertyName, List<WIA.Property> properties)
        {
            return properties.FindIndex(p => propertyName.EqualsWithoutCase(p.Name));
        }

        private static int zGetDataBytes(Bitmap src, Bytes dst, int dstOffset)
        {
            // -- fonction inspirée de BasicDibMng

            BitmapData dt = src.LockBits(new RectI(new PointI(), src.Size), ImageLockMode.ReadOnly, src.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;
            MarshShop.PointerToBuffer(dt.Scan0, cnt, dst, dstOffset);

            src.UnlockBits(dt);

            return cnt;
        }

        private static int zGetDataBytesCount(Bitmap bmp)
        {
            BitmapData dt = bmp.LockBits(new RectI(new PointI(), bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;

            bmp.UnlockBits(dt);

            return cnt;
        }

        private static int zSetDataBytes(Bytes src, int srcOffset, Bitmap dst)
        {
            // -- fonction inspirée de BasicDibMng

            BitmapData dt = dst.LockBits(new RectI(new PointI(), dst.Size), ImageLockMode.ReadWrite, dst.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;
            MarshShop.BufferToPointer(src, srcOffset, cnt, dt.Scan0);

            dst.UnlockBits(dt);

            return cnt;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}