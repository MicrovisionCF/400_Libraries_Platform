using System.Runtime.InteropServices;

namespace Microvision.NativeMethods
{
    internal static partial class OpenGl32
    {
        public struct POINTFLOAT
        {
            public POINTFLOAT(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public float x;
            public float y;
        }

        public struct GLYPHMETRICSFLOAT
        {
            public GLYPHMETRICSFLOAT(float gmfBlackBoxX, float gmfBlackBoxY, POINTFLOAT gmfptGlyphOrigin, float gmfCellIncX, float gmfCellIncY)
            {
                this.gmfBlackBoxX = gmfBlackBoxX;
                this.gmfBlackBoxY = gmfBlackBoxY;
                this.gmfptGlyphOrigin = gmfptGlyphOrigin;
                this.gmfCellIncX = gmfCellIncX;
                this.gmfCellIncY = gmfCellIncY;
            }

            public float gmfBlackBoxX;
            public float gmfBlackBoxY;
            public POINTFLOAT gmfptGlyphOrigin;
            public float gmfCellIncX;
            public float gmfCellIncY;
        }

        [DllImport(nameof(OpenGl32))]
        public static extern IntPtr wglGetCurrentContext();

        [DllImport(nameof(OpenGl32))]
        public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hrc);

        [DllImport(nameof(OpenGl32))]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport(nameof(OpenGl32))]
        public static extern int wglDeleteContext(IntPtr hrc);

        [DllImport(nameof(OpenGl32))]
        public static extern IntPtr wglGetProcAddress(string name);

        [DllImport(nameof(OpenGl32))]
        public static extern bool wglUseFontBitmaps(IntPtr hDC, uint first, uint count, uint listBase);

        [DllImport(nameof(OpenGl32))]
        public static extern bool wglUseFontOutlinesA(IntPtr hDC, uint first, uint count, uint listBase, float deviation, float extrusion, int format, GLYPHMETRICSFLOAT[]? lpgmf);

        [DllImport(nameof(OpenGl32))]
        public static extern bool wglShareLists(IntPtr hrc1, IntPtr hrc2);

    }
}
