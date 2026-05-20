using Microvision.Geometry;
using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.Avi
{
    public class DrawDibDC : Citizen
    {
        // ***************************************************************************************************
        // 08.03.10 : (création) fonctions d'Msvfw32.dll (officiellement vfw.dll)
        // 14.06.11 : libs 1.8
        // 05.02.14 : libs 2.0, intégration à µV.Platform (public au cas où, mais en principe pas utile).
        // 26.03.15 : lockstruct et locktable avant appels, par précaution.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 26.08.20 : Correction des déclarations en C (types de retour incorrects)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************


        private IntPtr _handle;
        private IntPtr _hDC;         // -- entre BeginDraw et EndDraw.


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DrawDibDC() : base()
        {
            _handle = Msvfw32.DrawDibOpen();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool BeginDraw(Graphics gf, ref NativeMethods.Gdi32.BITMAPINFO bmi)
        {
            _hDC = gf.GetHdc();

            IntPtr hbmi = MarshShop.LockStruct(bmi);
            bool ok = Msvfw32.DrawDibBegin(_handle, _hDC, -1, -1, hbmi, -1, -1, 0);
            bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);

            return ok;
        }

        public bool BeginDraw(Graphics gf, ref NativeMethods.Gdi32.BITMAPINFO bmi, SizeI siz)
        {
            _hDC = gf.GetHdc();

            IntPtr hbmi = MarshShop.LockStruct(bmi);
            bool ok = Msvfw32.DrawDibBegin(_handle, _hDC, siz.Width, siz.Height, hbmi, -1, -1, 0);
            bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);

            return ok;
        }

        public bool Draw(ref NativeMethods.Gdi32.BITMAPINFO bmi, Bytes bts, PointI p)
        {
            bool ok = false;

            if (_handle != IntPtr.Zero)
            {
                // DDF_SAME_DRAW incompatible avec zoom
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                LockTable<byte> hbf = new LockTable<byte>(bts.Array, bts.Length);
                ok = Msvfw32.DrawDibDraw(_handle, _hDC, p.X, p.Y, -1, -1, hbmi, hbf.Address(0), 0, 0, -1, -1, Msvfw32.DDF_SAME_HDC);
                hbf.Free();
                bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);
            }

            return ok;
        }

        public bool Draw(ref NativeMethods.Gdi32.BITMAPINFO bmi, Bytes bts, RectI rct)
        {
            bool ok = false;

            if (_handle != IntPtr.Zero)
            {
                // DDF_SAME_DRAW incompatible avec zoom
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                LockTable<byte> hbf = new LockTable<byte>(bts.Array, bts.Length);
                ok = Msvfw32.DrawDibDraw(_handle, _hDC, rct.X, rct.Y, rct.Width, rct.Height, hbmi, hbf.Address(0), 0, 0, -1, -1, Msvfw32.DDF_SAME_HDC);
                hbf.Free();
                bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);
            }

            return ok;
        }

        public bool EndDraw(Graphics gf)
        {
            gf.ReleaseHdc(_hDC);        // -- nouveau 2.0
            bool ok = Msvfw32.DrawDibEnd(_handle);

            return ok;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_handle != IntPtr.Zero)
            {
                Msvfw32.DrawDibClose(_handle);
                _handle = IntPtr.Zero;
            }

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