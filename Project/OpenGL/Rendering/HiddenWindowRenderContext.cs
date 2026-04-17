using System;

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

        private Win32.WndProc _procDelegate;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HiddenWindowRenderContext()
        {
            _procDelegate = new Win32.WndProc(Win32.DefWindowProcA);
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
                Win32.SwapBuffers(_deviceContextHandle);
                Win32.BitBlt(hdc, 0, 0, _width, _height, _deviceContextHandle, 0, 0, Win32.SRCCOPY);
            }
        }

        protected override bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object parameter)
        {
            bool ok = base.oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            if (ok)
            {
                _windowHandle = zCreateWindow(_width, _height, _procDelegate);
                _deviceContextHandle = Win32.GetDC(_windowHandle);

                Win32.PIXELFORMATDESCRIPTOR pfd = zCreatePixelFormat(_bitDepth);
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
                Win32.ReleaseDC(_windowHandle, DeviceContextHandle);
                Win32.DestroyWindow(_windowHandle);
                _windowHandle = IntPtr.Zero;
            }

            base.oDispose(isExplicit);
        }

        protected override void oMakeCurrent()
        {
            if (_renderContextHandle != IntPtr.Zero) Win32.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
        }

        protected override void oSetDimensions(int width, int height)
        {
            base.oSetDimensions(width, height);

            Win32.SetWindowPos(_windowHandle, IntPtr.Zero, 0, 0, width, height,
                Win32.SetWindowPosFlags.SWP_NOACTIVATE |
                Win32.SetWindowPosFlags.SWP_NOCOPYBITS |
                Win32.SetWindowPosFlags.SWP_NOMOVE |
                Win32.SetWindowPosFlags.SWP_NOOWNERZORDER);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Win32.PIXELFORMATDESCRIPTOR zCreatePixelFormat(int bitDepth)
        {
            Win32.PIXELFORMATDESCRIPTOR pfd = new Win32.PIXELFORMATDESCRIPTOR();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = Win32.PfdFlags.PFD_DRAW_TO_BITMAP | Win32.PfdFlags.PFD_SUPPORT_OPENGL | Win32.PfdFlags.PFD_DOUBLEBUFFER;
            pfd.iPixelType = Win32.PFDPixelType.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)bitDepth;
            pfd.cDepthBits = 16;
            pfd.cStencilBits = 8;
            pfd.iLayerType = Win32.PFDLayerType.PFD_MAIN_PLANE;

            return pfd;
        }

        private static IntPtr zCreateRenderHdc(IntPtr hdc, Win32.PIXELFORMATDESCRIPTOR pfd)
        {
            IntPtr renderHdc = IntPtr.Zero;
            int iPixelFormat = Win32.ChoosePixelFormat(hdc, pfd);

            if (iPixelFormat != 0 && Win32.SetPixelFormat(hdc, iPixelFormat, pfd) != 0)
                renderHdc = Win32.wglCreateContext(hdc);

            return renderHdc;
        }

        private static IntPtr zCreateWindow(int width, int height, Win32.WndProc procDel)
        {
            const string KWinName = "GLRenderWindow";

            Win32.WNDCLASSEX wndClass = new Win32.WNDCLASSEX();
            wndClass.Init();
            wndClass.style = Win32.ClassStyles.CS_HREDRAW | Win32.ClassStyles.CS_VREDRAW | Win32.ClassStyles.CS_OWNDC;
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

            Win32.RegisterClassExA(ref wndClass);
            IntPtr wndHandle = Win32.CreateWindowExA((Win32.WindowStylesEx)0, KWinName, "", Win32.WindowStyles.WS_CLIPCHILDREN | Win32.WindowStyles.WS_CLIPSIBLINGS | Win32.WindowStyles.WS_POPUP, 0, 0, width, height, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

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