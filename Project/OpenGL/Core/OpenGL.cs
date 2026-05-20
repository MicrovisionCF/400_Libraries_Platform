using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.OpenGL
{
    public partial class OpenGLContext : Citizen
    {
        // ***************************************************************************************************
        // 14.05.19 : Création, passerelle vers la librairie OpenGL
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test réussite création
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private readonly IntPtr _openGLLib;

        private RenderContext? _renderContext;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public OpenGLContext()
        {
            _openGLLib = Kernel32.LoadLibraryA("opengl32.dll");

            _fontEntries = [];
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Blit(IntPtr deviceContextHandle)
        {
            _renderContext?.Render(deviceContextHandle);
        }

        public bool CreateInMemory()
        {
            _renderContext = new FBORenderContext(4);
            bool ok = _renderContext.Create(OpenGLVersion.OpenGL2_1, this, 1000, 1000, 32, null);

            return ok;
        }

        public bool CreateOnWindow(IntPtr hWnd)
        {
            _renderContext = new NativeWindowRenderContext();
            bool ok = _renderContext.Create(OpenGLVersion.OpenGL2_1, this, 1000, 1000, 32, hWnd);

            return ok;
        }

        public void MakeCurrent()
        {
            _renderContext?.MakeCurrent();
        }

        public void MakeNothingCurrent()
        {
            OpenGl32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        public void SetDimensions(int width, int height)
        {
            _renderContext?.SetDimensions(width, height);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_renderContext is not null)
            {
                if (isExplicit) _renderContext.Dispose();
                _renderContext = null;
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