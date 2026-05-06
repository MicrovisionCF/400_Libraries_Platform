using Microvision.NativeMethods;

namespace Microvision.OpenGL
{
    internal class FBORenderContext : HiddenWindowRenderContext
    {
        // ***************************************************************************************************
        // 13.05.19 : Création, contexte de rendu rapide buffurisé pour rendu en mémoire
        // 28.08.19 : BlitFramebuffer avec pas -1 parce que bornes supérieures exclusives
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test réussite création
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected readonly int _antialiasing;
        protected readonly DIBContext _dibBuffer;
        
        protected OpenGLContext? _gl;
        protected IntPtr _dibSectionDeviceContext;

        protected uint _bufferFrameMulti;
        protected uint _bufferColorMulti;
        protected uint _bufferDepthMulti;

        protected uint _bufferFrameFinal;
        protected uint _bufferColorFinal;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public FBORenderContext(int antialiasing)
        {
            _antialiasing = antialiasing;
            _dibSectionDeviceContext = IntPtr.Zero;
            _dibBuffer = new DIBContext();
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

        protected bool oAllocBuffers()
        {
            ArgumentNullException.Check(_gl);

            bool ok;

            try
            {
                _bufferFrameMulti = _gl.GenFramebuffer();
                _gl.BindFramebuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, _bufferFrameMulti);
                _bufferDepthMulti = _gl.GenRenderbuffer();
                _gl.BindRenderbuffer(OpenGLConst.GL_RENDERBUFFER_EXT, _bufferDepthMulti);

                _gl.RenderbufferStorage(OpenGLConst.GL_RENDERBUFFER_EXT, OpenGLConst.GL_DEPTH_COMPONENT24, _width, _height);
                _gl.RenderbufferStorageMultisample(OpenGLConst.GL_RENDERBUFFER_EXT, _antialiasing, OpenGLConst.GL_DEPTH_COMPONENT24, _width, _height);
                _gl.FramebufferRenderbuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, OpenGLConst.GL_DEPTH_ATTACHMENT_EXT, OpenGLConst.GL_RENDERBUFFER_EXT, _bufferDepthMulti);

                _bufferColorMulti = _gl.GenRenderbuffer();
                _gl.BindRenderbuffer(OpenGLConst.GL_RENDERBUFFER_EXT, _bufferColorMulti);
                _gl.RenderbufferStorage(OpenGLConst.GL_RENDERBUFFER_EXT, OpenGLConst.GL_RGBA, _width, _height);
                _gl.RenderbufferStorageMultisample(OpenGLConst.GL_RENDERBUFFER_EXT, _antialiasing, OpenGLConst.GL_RGBA, _width, _height);
                _gl.FramebufferRenderbuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, OpenGLConst.GL_COLOR_ATTACHMENT0_EXT, OpenGLConst.GL_RENDERBUFFER_EXT, _bufferColorMulti);

                _bufferFrameFinal = _gl.GenFramebuffer();
                _bufferColorFinal = _gl.GenRenderbuffer();
                ok = true;
            }
            catch
            {
                ok = false;
            }

            return ok;
        }

        protected override void oBlit(IntPtr hdc)
        {
            ArgumentNullException.Check(_gl);

            if (_deviceContextHandle != IntPtr.Zero)
            {
                _gl.BindFramebuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, _bufferFrameFinal);
                _gl.BindRenderbuffer(OpenGLConst.GL_RENDERBUFFER_EXT, _bufferColorFinal);
                _gl.RenderbufferStorage(OpenGLConst.GL_RENDERBUFFER_EXT, OpenGLConst.GL_RGBA, _width, _height);

                _gl.FramebufferRenderbuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, OpenGLConst.GL_COLOR_ATTACHMENT0_EXT, OpenGLConst.GL_RENDERBUFFER_EXT, _bufferColorFinal);
                _gl.BindFramebuffer(OpenGLConst.GL_READ_FRAMEBUFFER, _bufferFrameMulti);
                _gl.BindFramebuffer(OpenGLConst.GL_DRAW_FRAMEBUFFER, _bufferFrameFinal);
                _gl.BlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, OpenGLConst.GL_COLOR_BUFFER_BIT, OpenGLConst.GL_NEAREST);

                _gl.BindFramebuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, _bufferFrameFinal);
                _gl.ReadBuffer((ReadBufferMode)OpenGLConst.GL_COLOR_ATTACHMENT0_EXT);
                _gl.ReadPixels(0, 0, _width, _height, PixelFormat.Bgra, PixelType.UnsignedByte, _dibBuffer.Bits);
                Gdi32.BitBlt(hdc, 0, 0, _width, _height, _dibSectionDeviceContext, 0, 0, Gdi32.SRCCOPY);

                _gl.BindFramebuffer(OpenGLConst.GL_FRAMEBUFFER_EXT, _bufferFrameMulti);
            }
        }

        protected override bool oCreate(OpenGLVersion openGLVersion, OpenGLContext gl, int width, int height, int bitDepth, object? parameter)
        {
            bool ok = base.oCreate(openGLVersion, gl, width, height, bitDepth, parameter);

            if (ok)
            {
                _gl = gl;

                _dibSectionDeviceContext = Gdi32.CreateCompatibleDC(_deviceContextHandle);
                _dibBuffer.Create(_dibSectionDeviceContext, _width, _height, _bitDepth);

                ok = oAllocBuffers();
            }

            return ok;
        }

        protected void oDestroyFramebuffers()
        {
            ArgumentNullException.Check(_gl);

            if (_bufferColorMulti != 0) _gl.DeleteRenderbuffers([_bufferColorMulti, _bufferDepthMulti, _bufferColorFinal]);
            if (_bufferFrameMulti != 0) _gl.DeleteFramebuffers([_bufferFrameMulti, _bufferFrameFinal]);
            _bufferColorMulti = 0U;
            _bufferDepthMulti = 0U;
            _bufferColorFinal = 0U;
            _bufferFrameMulti = 0U;
            _bufferFrameFinal = 0U;
        }

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _dibBuffer.Dispose();

            oDestroyFramebuffers();
            Gdi32.DeleteDC(_dibSectionDeviceContext);

            base.oDispose(isExplicit);
        }

        protected override void oSetDimensions(int width, int height)
        {
            base.oSetDimensions(width, height);

            _dibBuffer.Resize(_width, _height, _bitDepth);
            oDestroyFramebuffers();
            oAllocBuffers();
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