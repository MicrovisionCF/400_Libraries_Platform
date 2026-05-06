using System.Runtime.InteropServices;

namespace Microvision.OpenGL
{
    internal class Win32
    {
        // ***************************************************************************************************
        // 13.05.19 : Importation des fonctions natives utiles pour notre implémentation d'openGL
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public const uint SRCCOPY = 0xCC0020U;

        [DllImport("opengl32.dll")] public static extern IntPtr wglGetCurrentContext();
        [DllImport("opengl32.dll")] public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hrc);
        [DllImport("opengl32.dll")] public static extern IntPtr wglCreateContext(IntPtr hdc);
        [DllImport("opengl32.dll")] public static extern int wglDeleteContext(IntPtr hrc);
        [DllImport("opengl32.dll")] public static extern IntPtr wglGetProcAddress(string name);
        [DllImport("opengl32.dll")] public static extern bool wglUseFontBitmaps(IntPtr hDC, uint first, uint count, uint listBase);
        [DllImport("opengl32.dll")] public static extern bool wglUseFontOutlinesA(IntPtr hDC, uint first, uint count, uint listBase, float deviation, float extrusion, int format, GLYPHMETRICSFLOAT[]? lpgmf);
        [DllImport("opengl32.dll")] public static extern bool wglShareLists(IntPtr hrc1, IntPtr hrc2);

        
        

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


        // TODO enums :

        public const uint ANSI_CHARSET = 0U;
        public const uint DEFAULT_CHARSET = 1U;
        public const uint SYMBOL_CHARSET = 2U;
        public const uint FW_DONTCARE = 0U;
        public const uint FW_THIN = 100U;
        public const uint FW_EXTRALIGHT = 200U;
        public const uint FW_LIGHT = 300U;
        public const uint FW_NORMAL = 400U;
        public const uint FW_MEDIUM = 500U;
        public const uint FW_SEMIBOLD = 600U;
        public const uint FW_BOLD = 700U;
        public const uint FW_EXTRABOLD = 800U;
        public const uint FW_HEAVY = 900U;
        public const uint OUT_DEFAULT_PRECIS = 0U;
        public const uint OUT_STRING_PRECIS = 1U;
        public const uint OUT_CHARACTER_PRECIS = 2U;
        public const uint OUT_STROKE_PRECIS = 3U;
        public const uint OUT_TT_PRECIS = 4U;
        public const uint OUT_DEVICE_PRECIS = 5U;
        public const uint OUT_RASTER_PRECIS = 6U;
        public const uint OUT_TT_ONLY_PRECIS = 7U;
        public const uint OUT_OUTLINE_PRECIS = 8U;
        public const uint OUT_SCREEN_OUTLINE_PRECIS = 9U;
        public const uint OUT_PS_ONLY_PRECIS = 10U;
        public const uint CLIP_DEFAULT_PRECIS = 0U;
        public const uint CLIP_CHARACTER_PRECIS = 1U;
        public const uint CLIP_STROKE_PRECIS = 2U;
        public const uint CLIP_MASK = 0xFU;
        public const uint DEFAULT_QUALITY = 0U;
        public const uint DRAFT_QUALITY = 1U;
        public const uint PROOF_QUALITY = 2U;
        public const uint NONANTIALIASED_QUALITY = 3U;
        public const uint ANTIALIASED_QUALITY = 4U;
        public const uint CLEARTYPE_QUALITY = 5U;
        public const uint DEFAULT_PITCH = 0U;
        public const uint FIXED_PITCH = 1U;
        public const uint VARIABLE_PITCH = 2U;
    }
}