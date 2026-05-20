using System;
using System.Collections.Generic;

using Microvision.NativeMethods;

namespace Microvision.OpenGL
{
    public partial class OpenGLContext
    {
        // ***************************************************************************************************
        // 15.05.19 : Création, mécanismes bas niveau pour objets textes 3D
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        internal struct xFontEntry
        {
            public IntPtr HDC;
            public IntPtr HRC;
            public string faceName;
            public uint listBase;
            public float extrusion;
            public FontOutlineFormat fontOutlineFormat;
        }


        public enum FontOutlineFormat : int
        {
            Lines = 0,
            Polygons = 1
        }


        private readonly List<xFontEntry> _fontEntries;


        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void DrawText(string text, string fontName, float extrusion, bool fill)
        {
            ArgumentNullException.Check(_renderContext);

            FontOutlineFormat outline = fill ? FontOutlineFormat.Polygons : FontOutlineFormat.Lines;

            int fontNo = _fontEntries.FindIndex(o => o.HDC == _renderContext.DeviceContextHandle &&
                                                     o.HRC == _renderContext.RenderContextHandle &&
                                                     o.faceName.EqualsWithoutCase(fontName) &&
                                                     o.extrusion == extrusion &&
                                                     o.fontOutlineFormat == outline);

            if (fontNo == -1) fontNo = oAddFontOutlineEntry(fontName, extrusion, outline);

            this.ListBase(_fontEntries[fontNo].listBase);
            this.CallLists(DataType.UnsignedShort, text.Length, System.Text.Encoding.Unicode.GetBytes(text));
            this.Flush();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected int oAddFontOutlineEntry(string fontName, float extrusion, FontOutlineFormat fontOutlineFormat)
        {
            ArgumentNullException.Check(_renderContext);

            this.MakeCurrent();

            IntPtr hFont = zCreateFont(fontName);

            IntPtr hOldObject = Gdi32.SelectObject(_renderContext.DeviceContextHandle, hFont);
            uint listBase = GenLists(1);

            OpenGl32.wglUseFontOutlinesA(_renderContext.DeviceContextHandle, 0U, 255U, listBase, 0, extrusion, (int)fontOutlineFormat, null);
            Gdi32.SelectObject(_renderContext.DeviceContextHandle, hOldObject);
            Gdi32.DeleteObject(hFont);

            xFontEntry foe = new xFontEntry();
            foe.HDC = _renderContext.DeviceContextHandle;
            foe.HRC = _renderContext.RenderContextHandle;
            foe.faceName = fontName;
            foe.listBase = listBase;
            foe.extrusion = extrusion;
            foe.fontOutlineFormat = fontOutlineFormat;

            _fontEntries.Add(foe);

            return _fontEntries.Count - 1;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static IntPtr zCreateFont(string fontName)
        {
            return Gdi32.CreateFontA(10, 0, 0, 0, Gdi32.FW_DONTCARE, 0, 0, 0, Gdi32.DEFAULT_CHARSET, Gdi32.OUT_OUTLINE_PRECIS, Gdi32.CLIP_DEFAULT_PRECIS, Gdi32.CLEARTYPE_QUALITY, Gdi32.VARIABLE_PITCH, fontName);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}