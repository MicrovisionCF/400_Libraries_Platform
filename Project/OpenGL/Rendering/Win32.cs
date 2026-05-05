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

        [DllImport("kernel32.dll")] public static extern IntPtr LoadLibraryA(string lpFileName);
        [DllImport("opengl32.dll")] public static extern IntPtr wglGetCurrentContext();
        [DllImport("opengl32.dll")] public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hrc);
        [DllImport("opengl32.dll")] public static extern IntPtr wglCreateContext(IntPtr hdc);
        [DllImport("opengl32.dll")] public static extern int wglDeleteContext(IntPtr hrc);
        [DllImport("opengl32.dll")] public static extern IntPtr wglGetProcAddress(string name);
        [DllImport("opengl32.dll")] public static extern bool wglUseFontBitmaps(IntPtr hDC, uint first, uint count, uint listBase);
        [DllImport("opengl32.dll")] public static extern bool wglUseFontOutlinesA(IntPtr hDC, uint first, uint count, uint listBase, float deviation, float extrusion, int format, GLYPHMETRICSFLOAT[]? lpgmf);
        [DllImport("opengl32.dll")] public static extern bool wglShareLists(IntPtr hrc1, IntPtr hrc2);

        [DllImport("gdi32.dll")] public static extern int ChoosePixelFormat(IntPtr hDC, [In][MarshalAs(UnmanagedType.LPStruct)] PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll")] public static extern int SetPixelFormat(IntPtr hDC, int iPixelFormat, [In][MarshalAs(UnmanagedType.LPStruct)] PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll")] public static extern IntPtr GetStockObject(uint fnObject);
        [DllImport("gdi32.dll")] public static extern int SwapBuffers(IntPtr hDC);
        [DllImport("gdi32.dll")] public static extern bool BitBlt(IntPtr hDC, int x, int y, int width, int height, IntPtr hDCSource, int sourceX, int sourceY, uint type);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, DIBColors pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")] public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateFontA(int nHeight, int nWidth, int nEscapement, int nOrientation, uint fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);
        [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")] public static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        [DllImport("user32.dll")] public static extern short RegisterClassExA([In] ref WNDCLASSEX lpwcx);
        [DllImport("user32.dll")] public static extern IntPtr DefWindowProcA(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] public static extern IntPtr CreateWindowExA(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [Flags]
        public enum WindowStylesEx : uint
        {
            WS_EX_LEFT = 0x0U,
            WS_EX_LTRREADING = 0x0U,
            WS_EX_RIGHTSCROLLBAR = 0x0U,
            WS_EX_DLGMODALFRAME = 0x1U,
            WS_EX_NOPARENTNOTIFY = 0x4U,
            WS_EX_TOPMOST = 0x8U,
            WS_EX_ACCEPTFILES = 0x10U,
            WS_EX_TRANSPARENT = 0x20U,
            WS_EX_MDICHILD = 0x40U,
            WS_EX_TOOLWINDOW = 0x80U,
            WS_EX_WINDOWEDGE = 0x100U,
            WS_EX_CLIENTEDGE = 0x200U,
            WS_EX_CONTEXTHELP = 0x400U,
            WS_EX_RIGHT = 0x1000U,
            WS_EX_RTLREADING = 0x2000U,
            WS_EX_LEFTSCROLLBAR = 0x4000U,
            WS_EX_CONTROLPARENT = 0x10000U,
            WS_EX_STATICEDGE = 0x20000U,
            WS_EX_APPWINDOW = 0x40000U,
            WS_EX_LAYERED = 0x80000U,
            WS_EX_NOINHERITLAYOUT = 0x100000U,
            WS_EX_COMPOSITED = 0x2000000U,
            WS_EX_LAYOUTRTL = 0x400000U,
            WS_EX_NOACTIVATE = 0x8000000U,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST
        }

        [Flags()]
        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x0U,
            WS_TABSTOP = 0x10000U,
            WS_MAXIMIZEBOX = 0x10000U,
            WS_MINIMIZEBOX = 0x20000U,
            WS_GROUP = 0x20000U,
            WS_SIZEFRAME = 0x40000U,
            WS_SYSMENU = 0x80000U,
            WS_VSCROLL = 0x200000U,
            WS_BORDER = 0x800000U,
            WS_CAPTION = 0xC00000U,
            WS_CHILD = 0x40000000U,
            WS_CLIPCHILDREN = 0x2000000U,
            WS_CLIPSIBLINGS = 0x4000000U,
            WS_DISABLED = 0x8000000U,
            WS_DLGFRAME = 0x400000U,
            WS_HSCROLL = 0x100000U,
            WS_MAXIMIZE = 0x1000000U,
            WS_MINIMIZE = 0x20000000U,
            WS_POPUP = 0x00000000,
            WS_VISIBLE = 0x10000000U,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU
        }

        [Flags]
        public enum PfdFlags : uint
        {
            PFD_DOUBLEBUFFER = 0x1U,
            PFD_STEREO = 0x2U,
            PFD_DRAW_TO_WINDOW = 0x4U,
            PFD_DRAW_TO_BITMAP = 0x8U,
            PFD_SUPPORT_GDI = 0x10U,
            PFD_SUPPORT_OPENGL = 0x20U,
            PFD_GENERIC_FORMAT = 0x40U,
            PFD_NEED_PALETTE = 0x80U,
            PFD_NEED_SYSTEM_PALETTE = 0x100U,
            PFD_SWAP_EXCHANGE = 0x200U,
            PFD_SWAP_COPY = 0x400U,
            PFD_SWAP_LAYER_BUFFERS = 0x800U,
            PFD_GENERIC_ACCELERATED = 0x1000U,
            PFD_SUPPORT_DIRECTDRAW = 0x2000U
        }

        [Flags]
        public enum ClassStyles : uint
        {
            CS_VREDRAW = 0x1U,
            CS_HREDRAW = 0x2U,
            CS_DBLCLKS = 0x8U,
            CS_OWNDC = 0x20U,
            CS_CLASSDC = 0x40U,
            CS_PARENTDC = 0x80U,
            CS_NOCLOSE = 0x200U,
            CS_SAVEBITS = 0x800U,
            CS_BYTEALIGNCLIENT = 0x1000U,
            CS_BYTEALIGNWINDOW = 0x2000U,
            CS_GLOBALCLASS = 0x4000U,
            CS_DROPSHADOW = 0x20000U
        }

        public enum DIBColors : uint
        {
            DIB_RGB_COLORS = 0U,
            DIB_PAL_COLORS = 1U,
            DIB_PAL_INDICES = 2U
        }

        public enum PFDPixelType : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1
        }

        public enum PFDLayerType : sbyte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = -1
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_NOSIZE = 0x1U,
            SWP_NOZORDER = 0x4U,
            SWP_NOMOVE = 0x2U,
            SWP_NOREDRAW = 0x8U,
            SWP_NOACTIVATE = 0x10U,
            SWP_DRAWFRAME = 0x20U,
            SWP_FRAMECHANGED = 0x20U,
            SWP_SHOWWINDOW = 0x40U,
            SWP_HIDEWINDOW = 0x80U,
            SWP_NOCOPYBITS = 0x100U,
            SWP_NOOWNERZORDER = 0x200U,
            SWP_NOREPOSITION = 0x200U,
            SWP_NOSENDCHANGING = 0x400U,
            SWP_DEFERERASE = 0x2000U,
            SWP_ASYNCWINDOWPOS = 0x4000U
        }

        [StructLayout(LayoutKind.Explicit)]
        public class PIXELFORMATDESCRIPTOR
        {
            [FieldOffset(0)]
            public ushort nSize;
            [FieldOffset(2)]
            public ushort nVersion;
            [FieldOffset(4)]
            public PfdFlags dwFlags;
            [FieldOffset(8)]
            public PFDPixelType iPixelType;
            [FieldOffset(9)]
            public byte cColorBits;
            [FieldOffset(10)]
            public byte cRedBits;
            [FieldOffset(11)]
            public byte cRedShift;
            [FieldOffset(12)]
            public byte cGreenBits;
            [FieldOffset(13)]
            public byte cGreenShift;
            [FieldOffset(14)]
            public byte cBlueBits;
            [FieldOffset(15)]
            public byte cBlueShift;
            [FieldOffset(16)]
            public byte cAlphaBits;
            [FieldOffset(17)]
            public byte cAlphaShift;
            [FieldOffset(18)]
            public byte cAccumBits;
            [FieldOffset(19)]
            public byte cAccumRedBits;
            [FieldOffset(20)]
            public byte cAccumGreenBits;
            [FieldOffset(21)]
            public byte cAccumBlueBits;
            [FieldOffset(22)]
            public byte cAccumAlphaBits;
            [FieldOffset(23)]
            public byte cDepthBits;
            [FieldOffset(24)]
            public byte cStencilBits;
            [FieldOffset(25)]
            public byte cAuxBuffers;
            [FieldOffset(26)]
            public PFDLayerType iLayerType;
            [FieldOffset(27)]
            public byte bReserved;
            [FieldOffset(28)]
            public uint dwLayerMask;
            [FieldOffset(32)]
            public uint dwVisibleMask;
            [FieldOffset(36)]
            public uint dwDamageMask;

            public void Init()
            {
                nSize = (ushort)Marshal.SizeOf(this);
            }
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

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public ClassStyles style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string? lpszMenuName;
            public string? lpszClassName;
            public IntPtr hIconSm;

            public void Init()
            {
                cbSize = (uint)Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;

            public void Init()
            {
                biSize = Marshal.SizeOf(this);
            }
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