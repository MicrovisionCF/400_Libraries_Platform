using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.OpenGL
{
    internal enum RenderContextType
    {
        NativeWindow,
        HiddenWindow,
        FBO
    }

    internal abstract class RenderContext : Citizen
    {
        // ***************************************************************************************************
        // 13.05.19 : Création, classe de base pour les différents contextes de rendu 3D
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test réussite création
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected IntPtr _renderContextHandle;
        protected IntPtr _deviceContextHandle;
        protected int _width;
        protected int _height;
        protected int _bitDepth;
        protected OpenGLVersion _requestedOpenGLVersion;
        protected OpenGLVersion _createdOpenGLVersion;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public RenderContext()
        {
            _createdOpenGLVersion = (OpenGLVersion)IntPtr.Zero;
            _deviceContextHandle = IntPtr.Zero;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int BitDepth => _bitDepth;

        public OpenGLVersion CreatedOpenGLVersion => _createdOpenGLVersion;

        public IntPtr DeviceContextHandle
        {
            get => _deviceContextHandle;
            protected set => _deviceContextHandle = value;
        }

        public int Height => _height;

        public IntPtr RenderContextHandle
        {
            get => _renderContextHandle;
            protected set => _renderContextHandle = value;
        }

        public OpenGLVersion RequestedOpenGLVersion => _requestedOpenGLVersion;

        public int Width => _width;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool Create(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object? parameter)
        {
            bool ok = oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            if (ok)
            {
                oMakeCurrent();
                oUpdateContextVersion(gl);
            }

            return ok;
        }

        public void MakeCurrent()
        {
            oMakeCurrent();
        }

        public void Render(IntPtr hdcTarget)
        {
            oBlit(hdcTarget);
        }

        public void SetDimensions(int width, int height)
        {
            oSetDimensions(width, height);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected abstract void oBlit(IntPtr hdc);

        protected virtual bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object? parameter)
        {
            _width = width;
            _height = height;
            _bitDepth = bitDepth;
            _requestedOpenGLVersion = openGLVersion;
            _createdOpenGLVersion = openGLVersion;

            return true;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_renderContextHandle != IntPtr.Zero)
            {
                OpenGl32.wglDeleteContext(RenderContextHandle);
                _renderContextHandle = IntPtr.Zero;
            }

            _deviceContextHandle = IntPtr.Zero;

            base.oDispose(isExplicit);
        }

        protected abstract void oMakeCurrent();

        protected virtual void oSetDimensions(int width, int height)
        {
            _width = width;
            _height = height;
        }

        protected bool oUpdateContextVersion(OpenGLContext gl)
        {
            bool ok;
            VersionAttribute requestedVersionNumber = VersionAttribute.GetVersionAttribute(_requestedOpenGLVersion);

            int[] attributes = [  OpenGLConst.WGL_CONTEXT_MAJOR_VERSION_ARB,
                                  requestedVersionNumber.Major,
                                  OpenGLConst.WGL_CONTEXT_MINOR_VERSION_ARB,
                                  requestedVersionNumber.Minor,
                                  OpenGLConst.WGL_CONTEXT_FLAGS_ARB,
                                  OpenGLConst.WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
                                  0 ];
            try
            {
                if (!requestedVersionNumber.IsAtLeastVersion(3, 0))
                {
                    _createdOpenGLVersion = _requestedOpenGLVersion;
                }
                else
                {
                    IntPtr hrc = gl.CreateContextAttribsARB(IntPtr.Zero, attributes);
                    OpenGl32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                    OpenGl32.wglDeleteContext(RenderContextHandle);
                    OpenGl32.wglMakeCurrent(DeviceContextHandle, hrc);
                    _renderContextHandle = hrc;
                }

                ok = true;
            }
            catch
            {
                _createdOpenGLVersion = OpenGLVersion.OpenGL2_1;
                ok = false;
            }

            return ok;
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