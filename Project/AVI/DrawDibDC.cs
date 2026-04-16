using System;
using System.Drawing;
using System.Runtime.InteropServices;

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

        [DllImport("Msvfw32.dll")] private static extern bool DrawDibBegin(IntPtr hdd, IntPtr hdc, int dxDest, int dyDest, IntPtr lpbi, int dxSrc, int dySrc, int wFlags);
        [DllImport("Msvfw32.dll")] private static extern bool DrawDibClose(IntPtr hdd);
        [DllImport("Msvfw32.dll")] private static extern bool DrawDibDraw(IntPtr hdd, IntPtr hdc, int xDst, int yDst, int dxDst, int dyDst, IntPtr lpbi, IntPtr lpBits, int xSrc, int ySrc, int dxSrc, int dySrc, int wFlags);
        [DllImport("Msvfw32.dll")] private static extern bool DrawDibEnd(IntPtr hdd);
        [DllImport("Msvfw32.dll")] private static extern IntPtr DrawDibGetBuffer(IntPtr hdd, IntPtr lpbi, int dwSize, int dwFlags);
        [DllImport("Msvfw32.dll")] private static extern IntPtr DrawDibOpen();
        [DllImport("Msvfw32.dll")] private static extern uint DrawDibRealize(IntPtr hdd, IntPtr hdc, int fBackground);


        private const short DDF_UPDATE = 2; // /* re-draw the last DIB */
        private const short DDF_SAME_HDC = 4; // /* HDC same As last call (all setup) */
        private const short DDF_SAME_DRAW = 8; // /* draw params are the same */
        private const short DDF_DONTDRAW = 16; // /* dont draw frame, just decompress */
        private const short DDF_ANIMATE = 32; // /* allow palette animation */
        private const short DDF_BUFFER = 64; // /* always buffer image */
        private const short DDF_JUSTDRAWIT = 128; // /* just draw it with GDI */
        private const short DDF_FULLSCREEN = 256; // /* use DisplayDib */
        private const short DDF_BACKGROUNDPAL = 512; // /* Realize palette in background */
        private const short DDF_NOTKEYFRAME = 1024; // /* this is a partial frame update, hint */
        private const short DDF_HURRYUP = 2048; // /* hurry up please! */
        private const short DDF_HALFTONE = 4096; // /* always halftone */


        private IntPtr _handle;
        private IntPtr _hDC;         // -- entre BeginDraw et EndDraw.


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DrawDibDC() : base()
        {
            _handle = DrawDibOpen();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool BeginDraw(Graphics gf, ref BasicDibApi.BITMAPINFO bmi)
        {
            _hDC = gf.GetHdc();

            IntPtr hbmi = MarshShop.LockStruct(bmi);
            bool ok = DrawDibBegin(_handle, _hDC, -1, -1, hbmi, -1, -1, 0);
            bmi = MarshShop.UnlockStruct<BasicDibApi.BITMAPINFO>(hbmi);

            return ok;
        }

        public bool BeginDraw(Graphics gf, ref BasicDibApi.BITMAPINFO bmi, Size siz)
        {
            _hDC = gf.GetHdc();

            IntPtr hbmi = MarshShop.LockStruct(bmi);
            bool ok = DrawDibBegin(_handle, _hDC, siz.Width, siz.Height, hbmi, -1, -1, 0);
            bmi = MarshShop.UnlockStruct<BasicDibApi.BITMAPINFO>(hbmi);

            return ok;
        }

        public bool Draw(ref BasicDibApi.BITMAPINFO bmi, Bytes bts, Point p)
        {
            bool ok = false;
            
            if (_handle != IntPtr.Zero)
            {
                // DDF_SAME_DRAW incompatible avec zoom
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                LockTable<byte> hbf = new LockTable<byte>(bts.Array, bts.Length);
                ok = DrawDibDraw(_handle, _hDC, p.X, p.Y, -1, -1, hbmi, hbf.Address(0), 0, 0, -1, -1, DDF_SAME_HDC);
                hbf.Free();
                bmi = MarshShop.UnlockStruct<BasicDibApi.BITMAPINFO>(hbmi);
            }

            return ok;
        }

        public bool Draw(ref BasicDibApi.BITMAPINFO bmi, Bytes bts, Rectangle rct)
        {
            bool ok = false;
            
            if (_handle != IntPtr.Zero)
            {
                // DDF_SAME_DRAW incompatible avec zoom
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                LockTable<byte> hbf = new LockTable<byte>(bts.Array, bts.Length);
                ok = DrawDibDraw(_handle, _hDC, rct.X, rct.Y, rct.Width, rct.Height, hbmi, hbf.Address(0), 0, 0, -1, -1, DDF_SAME_HDC);
                hbf.Free();
                bmi = MarshShop.UnlockStruct<BasicDibApi.BITMAPINFO>(hbmi);
            }

            return ok;
        }

        public bool EndDraw(Graphics gf)
        {
            gf.ReleaseHdc(_hDC);        // -- nouveau 2.0
            bool ok = DrawDibEnd(_handle);

            return ok;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_handle != IntPtr.Zero)
            {
                DrawDibClose(_handle);
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