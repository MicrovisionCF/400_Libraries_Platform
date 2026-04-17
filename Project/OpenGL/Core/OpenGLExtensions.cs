using System.Runtime.InteropServices;

namespace Microvision.OpenGL
{
    public partial class OpenGLContext
    {
        // ***************************************************************************************************
        // 14.05.19 : Création, importation des fonctions d'extensions openGL32.dll qui nous sont utiles
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private delegate void glBindFramebufferEXT(uint target, uint framebuffer);
        private delegate void glBindRenderbufferEXT(uint target, uint renderbuffer);
        private delegate void glBlitFramebuffer(int x0, int y0, int x1, int y1, int dstx0, int dsty0, int dstx1, int dsty1, uint mask, uint filter);
        private delegate void glDeleteFramebuffersEXT(uint n, uint[] framebuffers);
        private delegate void glDeleteRenderbuffersEXT(uint n, uint[] renderbuffers);
        private delegate void glFramebufferRenderbufferEXT(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer);
        private delegate void glGenFramebuffersEXT(uint n, uint[] framebuffers);
        private delegate void glGenRenderbuffersEXT(uint n, uint[] renderbuffers);
        private delegate void glRenderbufferStorageEXT(uint target, uint internalformat, int width, int height);
        private delegate void glRenderbufferStorageMultisampleEXT(uint target, int samples, uint internalformat, int width, int height);
        private delegate void glTexImage2DMultisample(uint target, int samples, uint internalformat, int width, int height, uint fixedsamplelocations);
        private delegate IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext, int[] attribList);

        // ***************************************************************************************************

        private Dictionary<string, Delegate> _delegates = new Dictionary<string, Delegate>();


        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        internal void BindFramebuffer(uint target, uint framebuffer)
        {
            oGetDelegateFor<glBindFramebufferEXT>()(target, framebuffer);
        }

        internal void BindRenderbuffer(uint target, uint renderbuffer)
        {
            oGetDelegateFor<glBindRenderbufferEXT>()(target, renderbuffer);
        }

        internal void BlitFramebuffer(int x0, int y0, int x1, int y1, int dstx0, int dsty0, int dstx1, int dsty1, uint mask, uint filter)
        {
            oGetDelegateFor<glBlitFramebuffer>()(x0, y0, x1, y1, dstx0, dsty0, dstx1, dsty1, mask, filter);
        }

        internal IntPtr CreateContextAttribsARB(IntPtr hShareContext, int[] attribList)
        {
            return oGetDelegateFor<wglCreateContextAttribsARB>()(_renderContext.DeviceContextHandle, hShareContext, attribList);
        }

        internal void DeleteFramebuffers(IEnumerable<uint> framebuffers)
        {
            oGetDelegateFor<glDeleteFramebuffersEXT>()((uint)framebuffers.Count(), framebuffers.ToArray());
        }

        internal void DeleteRenderbuffers(IEnumerable<uint> renderbuffers)
        {
            oGetDelegateFor<glDeleteRenderbuffersEXT>()((uint)renderbuffers.Count(), renderbuffers.ToArray());
        }

        internal void FramebufferRenderbuffer(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer)
        {
            oGetDelegateFor<glFramebufferRenderbufferEXT>()(target, attachment, renderbuffertarget, renderbuffer);
        }

        internal uint GenFramebuffer()
        {
            return GenFramebuffers(1U)[0];
        }

        internal uint[] GenFramebuffers(uint n)
        {
            uint[] framebuffers = new uint[n];
            oGetDelegateFor<glGenFramebuffersEXT>()(n, framebuffers);

            return framebuffers;
        }

        internal uint GenRenderbuffer()
        {
            return GenRenderbuffers(1U)[0];
        }

        internal uint[] GenRenderbuffers(uint n)
        {
            uint[] framebuffers = new uint[n];
            oGetDelegateFor<glGenRenderbuffersEXT>()(n, framebuffers);

            return framebuffers;
        }

        internal void RenderbufferStorage(uint target, uint internalformat, int width, int height)
        {
            oGetDelegateFor<glRenderbufferStorageEXT>()(target, internalformat, width, height);
        }

        internal void RenderbufferStorageMultisample(uint target, int samples, uint internalformat, int width, int height)
        {
            oGetDelegateFor<glRenderbufferStorageMultisampleEXT>()(target, samples, internalformat, width, height);
        }

        internal void TexImage2DMultisample(uint target, int samples, uint internalformat, int width, int height, uint fixedsamplelocations)
        {
            oGetDelegateFor<glTexImage2DMultisample>()(target, samples, internalformat, width, height, fixedsamplelocations);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected T oGetDelegateFor<T>() where T : class
        {
            Type delType = typeof(T);
            string delName = delType.Name;

            if (!_delegates.ContainsKey(delName))
            {
                IntPtr delPtr = Win32.wglGetProcAddress(delName);
                if (delPtr != IntPtr.Zero)
                    _delegates.Add(delName, Marshal.GetDelegateForFunctionPointer(delPtr, delType));
                else
                    throw new Exception("OpenGL : Extension function " + delName + " not supported");
            }

            return _delegates[delName] as T;
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