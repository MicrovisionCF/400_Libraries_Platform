using System;

using Microvision.NativeMethods;

namespace Microvision.OpenGL
{
    internal class HiddenWindowRenderContext : RenderContext
    {
        // ***************************************************************************************************
        // 13.05.19 : Création, contexte de rendu très rapide sur fenêtre non affichée à l'écran
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test réussite création
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected IntPtr _windowHandle = IntPtr.Zero;

        private User32.WndProc _procDelegate;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HiddenWindowRenderContext()
        {
            _procDelegate = new User32.WndProc(User32.DefWindowProcA);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oBlit(IntPtr hdc)
        {
            if (_deviceContextHandle != IntPtr.Zero || _windowHandle != IntPtr.Zero)
            {
                Gdi32.SwapBuffers(_deviceContextHandle);
                Gdi32.BitBlt(hdc, 0, 0, _width, _height, _deviceContextHandle, 0, 0, Gdi32.SRCCOPY);
            }
        }

        protected override bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object? parameter)
        {
            bool ok = base.oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            if (ok)
            {
                _windowHandle = zCreateWindow(_width, _height, _procDelegate);
                _deviceContextHandle = User32.GetDC(_windowHandle);

                Gdi32.PIXELFORMATDESCRIPTOR pfd = zCreatePixelFormat(_bitDepth);
                _renderContextHandle = zCreateRenderHdc(_deviceContextHandle, pfd);

                oMakeCurrent();
                ok = oUpdateContextVersion(gl);
            }

            return ok;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_windowHandle != IntPtr.Zero)
            {
                User32.ReleaseDC(_windowHandle, DeviceContextHandle);
                User32.DestroyWindow(_windowHandle);
                _windowHandle = IntPtr.Zero;
            }

            base.oDispose(isExplicit);
        }

        protected override void oMakeCurrent()
        {
            if (_renderContextHandle != IntPtr.Zero) OpenGl32.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
        }

        protected override void oSetDimensions(int width, int height)
        {
            base.oSetDimensions(width, height);

            User32.SetWindowPos(_windowHandle, IntPtr.Zero, 0, 0, width, height,
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOMOVE |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Gdi32.PIXELFORMATDESCRIPTOR zCreatePixelFormat(int bitDepth)
        {
            Gdi32.PIXELFORMATDESCRIPTOR pfd = new Gdi32.PIXELFORMATDESCRIPTOR();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = Gdi32.PfdFlags.PFD_DRAW_TO_BITMAP | Gdi32.PfdFlags.PFD_SUPPORT_OPENGL | Gdi32.PfdFlags.PFD_DOUBLEBUFFER;
            pfd.iPixelType = Gdi32.PFDPixelType.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)bitDepth;
            pfd.cDepthBits = 16;
            pfd.cStencilBits = 8;
            pfd.iLayerType = Gdi32.PFDLayerType.PFD_MAIN_PLANE;

            return pfd;
        }

        private static IntPtr zCreateRenderHdc(IntPtr hdc, Gdi32.PIXELFORMATDESCRIPTOR pfd)
        {
            IntPtr renderHdc = IntPtr.Zero;
            int iPixelFormat = Gdi32.ChoosePixelFormat(hdc, pfd);

            if (iPixelFormat != 0 && Gdi32.SetPixelFormat(hdc, iPixelFormat, pfd) != 0)
                renderHdc = OpenGl32.wglCreateContext(hdc);

            return renderHdc;
        }

        private static IntPtr zCreateWindow(int width, int height, User32.WndProc procDel)
        {
            const string KWinName = "GLRenderWindow";

            User32.WNDCLASSEX wndClass = new User32.WNDCLASSEX();
            wndClass.Init();
            wndClass.style = User32.ClassStyles.CS_HREDRAW | User32.ClassStyles.CS_VREDRAW | User32.ClassStyles.CS_OWNDC;
            wndClass.lpfnWndProc = procDel;
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = IntPtr.Zero;
            wndClass.hIcon = IntPtr.Zero;
            wndClass.hCursor = IntPtr.Zero;
            wndClass.hbrBackground = IntPtr.Zero;
            wndClass.lpszMenuName = null;
            wndClass.lpszClassName = KWinName;
            wndClass.hIconSm = IntPtr.Zero;

            User32.RegisterClassExA(ref wndClass);
            IntPtr wndHandle = User32.CreateWindowExA((User32.WindowStylesEx)0, KWinName, "", User32.WindowStyles.WS_CLIPCHILDREN | User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_POPUP, 0, 0, width, height, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            return wndHandle;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}