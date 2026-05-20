using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.OpenGL
{
    internal class DIBContext : Citizen
    {
        // ***************************************************************************************************
        // 14.05.19 : Création, rendu lent dans une image allouée automatiquement en mémoire
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected IntPtr _parentDC;
        protected IntPtr _hBitmap;
        protected IntPtr _bits;
        protected int _bitDepth;
        protected int _width;
        protected int _height;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DIBContext()
        {
            _parentDC = IntPtr.Zero;
            _hBitmap = IntPtr.Zero;
            _bits = IntPtr.Zero;
            _bitDepth = 0;
            _width = 0;
            _height = 0;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public IntPtr Bits => _bits;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool Create(IntPtr hDC, int width, int height, int bitDepth)
        {
            _parentDC = hDC;

            oDeleteBitmap();
            oCreateBitmap(width, height, bitDepth);

            return oSetPixelFormat(_parentDC, _bitDepth);
        }

        public void Resize(int width, int height, int bitDepth)
        {
            oDeleteBitmap();
            oCreateBitmap(width, height, bitDepth);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected void oCreateBitmap(int width, int height, int bitDepth)
        {
            _bitDepth = bitDepth;
            _width = width;
            _height = height;

            Gdi32.BITMAPINFO info = zCreateBitmapInfo(_bitDepth, _width, _height);
            _hBitmap = zCreateBitmapPtr(_parentDC, info, out _bits);
        }

        protected void oDeleteBitmap()
        {
            if (_hBitmap != IntPtr.Zero)
            {
                Gdi32.DeleteObject(_hBitmap);
                _hBitmap = IntPtr.Zero;
            }
        }

        protected override void oDispose(bool isExplicit)
        {
            oDeleteBitmap();

            base.oDispose(isExplicit);
        }

        protected virtual bool oSetPixelFormat(IntPtr hDC, int bitDepth)
        {
            bool ok = false;
            _bitDepth = bitDepth;

            Gdi32.PIXELFORMATDESCRIPTOR pfd = zCreatePixelFormat(_bitDepth);
            int iPixelformat = Gdi32.ChoosePixelFormat(hDC, pfd);

            if (iPixelformat != 0)
            {
                if (Gdi32.SetPixelFormat(hDC, iPixelformat, pfd) != 0)
                {
                    ok = true;
                }
                else
                {
                    ok = false;
                    int lastError = Marshal.GetLastWin32Error();
                    Debug.Print("OpenGL : Error in SetPixelFormat " + lastError);
                }
            }

            return ok;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Gdi32.BITMAPINFO zCreateBitmapInfo(int bitDepth, int width, int height)
        {
            Gdi32.BITMAPINFOHEADER header = new Gdi32.BITMAPINFOHEADER();
            header.biSize = (uint)Marshal.SizeOf<Gdi32.BITMAPINFOHEADER>();
            header.biBitCount = (ushort)bitDepth;
            header.biPlanes = 1;
            header.biWidth = width;
            header.biHeight = height;

            Gdi32.BITMAPINFO info = new Gdi32.BITMAPINFO(header);

            return info;
        }

        private static IntPtr zCreateBitmapPtr(IntPtr hDC, Gdi32.BITMAPINFO info, out IntPtr bits)
        {
            IntPtr ptr = Gdi32.CreateDIBSection(hDC, ref info, Gdi32.DIBColors.DIB_RGB_COLORS, out bits, IntPtr.Zero, 0U);
            Gdi32.SelectObject(hDC, ptr);

            return ptr;
        }

        private static Gdi32.PIXELFORMATDESCRIPTOR zCreatePixelFormat(int bitDepth)
        {
            Gdi32.PIXELFORMATDESCRIPTOR pfd = new Gdi32.PIXELFORMATDESCRIPTOR();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = Gdi32.PfdFlags.PFD_DRAW_TO_BITMAP | Gdi32.PfdFlags.PFD_SUPPORT_OPENGL | Gdi32.PfdFlags.PFD_SUPPORT_GDI;
            pfd.iPixelType = Gdi32.PFDPixelType.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)bitDepth;
            pfd.cDepthBits = 32;
            pfd.iLayerType = Gdi32.PFDLayerType.PFD_MAIN_PLANE;

            return pfd;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}