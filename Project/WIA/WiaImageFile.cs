using System.Drawing.Imaging;
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
        // ***************************************************************************************************

        private readonly WIA.ImageFile _imgFile;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaImageFile(WIA.ImageFile imgf) : base()
        {
            _imgFile = imgf;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string FormatID => _imgFile.FormatID;

        public int PropertiesCount => _imgFile.Properties.Count;

        public PointG Resolution => new PointG((float)_imgFile.HorizontalResolution, (float)_imgFile.VerticalResolution); // dpi

        public SizeI Size => new SizeI(_imgFile.Width, _imgFile.Height);


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugImageFile(this, pfx);
        }

        public int FindProperty(string pnam)
        {
            return zFindProperty(pnam, _imgFile.Properties.ToList());
        }

        public Bitmap GetBitmap()
        {
            // -- pas d'usage de BasicBitmap parce que je sais pas encore dans quelle librairie cet objet va aboutir.

            string fnam = FileName.GetTempFileName(_imgFile.FileExtension);

            _imgFile.SaveFile(fnam);
            Bitmap tmp = new Bitmap(fnam);

            Bytes bf = new Bytes(zGetDataBytesCount(tmp));
            zGetDataBytes(tmp, bf, 0);
            Bitmap bmp = zCreateCoreBitmap(tmp.Size, tmp.PixelFormat, tmp.Palette);
            zSetDataBytes(bf, 0, bmp);

            tmp.Dispose();
            File.Delete(fnam);

            return bmp;
        }

        public WiaProperty GetProperty(int no)
        {
            return new WiaProperty(_imgFile.Properties.ToList()[no]);
        }

        public void SaveFile(string fnam)
        {
            _imgFile.SaveFile(fnam);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_imgFile);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Bitmap zCreateCoreBitmap(SizeI sz, PixelFormat fmt, ColorPalette pal)
        {
            // -- fonction pompée sur BasicDibMng

            Bitmap bmp = new Bitmap(sz.Width, sz.Height, fmt);

            if (fmt.HasFlag(PixelFormat.Indexed) && pal is not null)
                bmp.Palette = pal;

            return bmp;
        }

        private static string zDebugImageFile(WiaImageFile imgf, string pfx)
        {
            string ch = "";
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Résolution : " + imgf.Resolution.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Taille : " + imgf.Size.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Format : " + imgf.FormatID.ToString();

            for (int i = 0; i < imgf.PropertiesCount; i++)
            {
                WiaProperty prp = imgf.GetProperty(i);
                ch = ch + SpecialChars.NewLine + prp.DebugString(pfx + SpecialChars.Tab);
                prp.Dispose();
            }

            return ch;
        }

        private static int zFindProperty(string nam, List<WIA.Property> prps)
        {
            return prps.FindIndex(p => nam.EqualsWithoutCase(p.Name));
        }

        private static int zGetDataBytes(Bitmap bmp, Bytes bf, int bfpos)
        {
            // -- fonction inspirée de BasicDibMng

            BitmapData dt = bmp.LockBits(new RectI(new PointI(), bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;
            MarshShop.PointerToBuffer(dt.Scan0, cnt, bf, bfpos);

            bmp.UnlockBits(dt);

            return cnt;
        }

        private static int zGetDataBytesCount(Bitmap bmp)
        {
            BitmapData dt = bmp.LockBits(new RectI(new PointI(), bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;

            bmp.UnlockBits(dt);

            return cnt;
        }

        private static int zSetDataBytes(Bytes bf, int bfpos, Bitmap bmp)
        {
            // -- fonction inspirée de BasicDibMng

            BitmapData dt = bmp.LockBits(new RectI(new PointI(), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);

            int cnt = Math.Abs(dt.Stride) * dt.Height;
            MarshShop.BufferToPointer(bf, bfpos, cnt, dt.Scan0);

            bmp.UnlockBits(dt);

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