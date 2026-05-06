using Microvision.NativeMethods;

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
                Gdi32.SwapBuffers(_deviceContextHandle);
        }

        protected override bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object? parameter)
        {
            base.oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            try
            {
                ArgumentNullException.Check(parameter);
                _windowHandle = (IntPtr)parameter;
            }
            catch
            {
                throw new Exception("A valid Window Handle must be provided for the NativeWindowRenderContext");
            }

            _deviceContextHandle = User32.GetDC(_windowHandle);
            Gdi32.PIXELFORMATDESCRIPTOR pfd = zCreatePixelFormat(_bitDepth);
            _renderContextHandle = zCreateRenderHdc(_deviceContextHandle, pfd);

            return _renderContextHandle != IntPtr.Zero;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_windowHandle != IntPtr.Zero)
            {
                User32.ReleaseDC(_windowHandle, _deviceContextHandle);
                _windowHandle = IntPtr.Zero;
            }

            base.oDispose(isExplicit);
        }

        protected override void oMakeCurrent()
        {
            if (_renderContextHandle != IntPtr.Zero)
                OpenGl32.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
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


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}