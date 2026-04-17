namespace Microvision.OpenGL
{
    internal class NativeWindowRenderContext : RenderContext
    {
        // ***************************************************************************************************
        // 14.05.19 : Création, contexte de rendu très rapide sur un pointeur écran classique
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected IntPtr _windowHandle;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public NativeWindowRenderContext()
        {
            _windowHandle = IntPtr.Zero;
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
                Win32.SwapBuffers(_deviceContextHandle);
        }

        protected override bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object parameter)
        {
            base.oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            try
            {
                _windowHandle = (IntPtr)parameter;
            }
            catch
            {
                throw new Exception("A valid Window Handle must be provided for the NativeWindowRenderContext");
            }

            _deviceContextHandle = Win32.GetDC(_windowHandle);
            Win32.PIXELFORMATDESCRIPTOR pfd = zCreatePixelFormat(_bitDepth);
            _renderContextHandle = zCreateRenderHdc(_deviceContextHandle, pfd);

            return _renderContextHandle != IntPtr.Zero;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_windowHandle != IntPtr.Zero)
            {
                Win32.ReleaseDC(_windowHandle, _deviceContextHandle);
                _windowHandle = IntPtr.Zero;
            }

            base.oDispose(isExplicit);
        }

        protected override void oMakeCurrent()
        {
            if (_renderContextHandle != IntPtr.Zero)
                Win32.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
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


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}