using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaItem : Citizen
    {
        // ***************************************************************************************************
        // 08.02.13 : ébauche
        // 31.07.14 : truncate dans zSetExtend.
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public enum ImageIntent // -- enum dupliquée pour ne pas imposer de référence à WIA aux utilisateurs de la librairie
        {
            UnspecifiedIntent = WIA.WiaImageIntent.UnspecifiedIntent,
            Colorintent = WIA.WiaImageIntent.ColorIntent,
            GrayscaleIntent = WIA.WiaImageIntent.GrayscaleIntent,
            TextIntent = WIA.WiaImageIntent.TextIntent
        }


        public static string wiaFormatBmp = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}"; // WIA.FormatID.wiaFormatBMP;
        public static string wiaFormatGIF = "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}"; // WIA.FormatID.wiaFormatGIF;
        public static string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}"; // WIA.FormatID.wiaFormatJPEG;
        public static string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}"; // WIA.FormatID.wiaFormatPNG;
        public static string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}"; // WIA.FormatID.wiaFormatTIFF;


        private const string KPropBitsPerPixel = "Bits Per Pixel";
        private const string KPropExtendX = "Horizontal Start Position";
        private const string KPropExtendY = "Vertical Start Position";
        private const string KPropExtendW = "Horizontal Extent";
        private const string KPropExtendH = "Vertical Extent";
        private const string KPropImageIntent = "Current Intent";
        private const string KPropResolX = "Horizontal Resolution";
        private const string KPropResolY = "Vertical Resolution";

        private const float KMMPerInch = 25.4f;


        // -- commandes constatées sur Epson Expression 1680, premier Item :
        // -- (idem WiaDevice)
        // Synchronize	
        // Delete device tree	
        // Build device tree	

        // -- propriétés constatées sur Epson Expression 1680, premier Item :
        // Item Name	String	Top
        // Full Item Name	String	0000\Root\Top
        // Item Flags	Integer	67
        // Color Profile Name	String	C:\Windows\system32\spool\drivers\color\sRGB Color Space Profile.icm
        // Horizontal Resolution	Integer	100
        // Vertical Resolution	Integer	100
        // Horizontal Extent	Integer	850
        // Vertical Extent	Integer	1170
        // Horizontal Start Position	Integer	0
        // Vertical Start Position	Integer	0
        // Data Type	Integer	3
        // Bits Per Pixel	Integer	24
        // Brightness	Integer	0
        // Contrast	Integer	0
        // Current Intent	Integer	0
        // Pixels Per Line	Integer	850
        // Number of Lines	Integer	1170
        // Preferred Format	15	{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}
        // Item Size	Integer	2985880
        // Threshold	Integer	110
        // Format	15	{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}
        // Media Type	Integer	128
        // Channels Per Pixel	Integer	3
        // Bits Per Channel	Integer	8
        // Planar	Integer	0
        // Bytes Per Line	Integer	2552
        // Buffer Size	Integer	65536
        // Access Rights	Integer	3
        // Compression	Integer	0
        // Photometric Interpretation	Integer	0
        // Lamp Warm up Time	Integer	90000

        // -- formats constatés sur Epson Expression 1680, premier Item :
        // wiaFormatBMP

        private WIA.Item _item;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaItem() : base()
        {
        }

        internal WiaItem(WIA.Item itm) : this()
        {
            _item = itm;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int BitsPerPixel => ConvertShop.ReadInt(_item.Properties[KPropBitsPerPixel].get_Value());

        public ImageIntent ColorIntent
        {
            get => (ImageIntent)ConvertShop.ReadInt(_item.Properties[KPropImageIntent].get_Value());
            set => _item.Properties[KPropImageIntent].set_Value(value);
        }

        public ImageIntent ColorIntentCaps
        {
            get
            {
                WiaProperty prop = new WiaProperty(_item.Properties[KPropImageIntent]);
                ImageIntent cps = (ImageIntent)prop.GetFlagMap();
                prop.Dispose();

                return cps;
            }
        }

        public int CommandsCount => _item.Commands.Count;

        internal WIA.Item Core => _item;

        public RectangleF Extend
        {
            get
            {
                RectangleF rct = zGetExtend(_item.Properties);
                ScaleF facs = zCalcMMFacs(zGetResolution(_item.Properties));

                return rct * facs;
            }

            set
            {
                ScaleF facs = zCalcMMFacs(zGetResolution(_item.Properties));
                zSetExtend(_item.Properties, value / facs);
            }
        }

        public int FormatsCount => _item.Formats.Count;

        public bool HasResolutionRange
        {
            get
            {
                WiaProperty prop = new WiaProperty(_item.Properties[KPropResolX]);
                bool fhas = prop.SubType == WiaProperty.PropertySubType.RangeSubType;
                prop.Dispose();

                return fhas;
            }
        }

        public bool HasResolutionTable
        {
            get
            {
                WiaProperty prop = new WiaProperty(_item.Properties[KPropResolX]);
                bool fhas = prop.SubType == WiaProperty.PropertySubType.ListSubType;
                prop.Dispose();

                return fhas;
            }
        }

        public string ItemID => _item.ItemID;

        public int PropertiesCount => _item.Properties.Count;

        public PointF Resolution
        {
            get => zGetResolution(_item.Properties);
            set => zSetResolution(_item.Properties, value);
        }

        public int SubItemsCount => _item.Items.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugItem(this, pfx);
        }

        public int FindProperty(string pnam)
        {
            return zFindProperty(pnam, _item.Properties.ToList());
        }

        public WiaCommand GetCommand(int no)
        {
            return new WiaCommand(_item.Commands.ToList()[no]);
        }

        public string GetFormatID(int no)
        {
            return _item.Formats.ToList()[no];
        }

        public WiaProperty GetProperty(int no)
        {
            return new WiaProperty(_item.Properties.ToList()[no]);
        }

        public (int minX, int maxX, int minY, int maxY) GetResolutionRange()
        {
            WiaProperty prop = new WiaProperty(_item.Properties[KPropResolX]);
            prop.GetRange(out int minX, out int maxX, out _);
            prop.Dispose();

            prop = new WiaProperty(_item.Properties[KPropResolY]);
            prop.GetRange(out int minY, out int maxY, out _);
            prop.Dispose();

            return (minX, maxX, minY, maxY);
        }

        public WiaItem GetSubItem(int no)
        {
            return new WiaItem(_item.Items.ToList()[no]);
        }

        public List<int> GetXResolutionTable()
        {
            WiaProperty prop = new WiaProperty(_item.Properties[KPropResolX]);
            List<int> resx = prop.GetTable<int>();
            prop.Dispose();

            return resx;
        }

        public List<int> GetYResolutionTable()
        {
            WiaProperty prop = new WiaProperty(_item.Properties[KPropResolY]);
            List<int> resy = prop.GetTable<int>();
            prop.Dispose();

            return resy;
        }

        public bool HasProperty(string nam)
        {
            return _item.Properties.Exists(nam);
        }

        public WiaImageFile Transfer()
        {
            return new WiaImageFile((WIA.ImageFile)_item.Transfer());
        }

        public WiaImageFile Transfer(string fmtid)
        {
            return new WiaImageFile((WIA.ImageFile)_item.Transfer(fmtid));
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_item is not null)
            {
                Marshal.ReleaseComObject(_item);
                _item = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static ScaleF zCalcMMFacs(PointF resol)
        {
            return new ScaleF(0, 0, KMMPerInch / resol.X, KMMPerInch / resol.Y);
        }

        private static string zDebugItem(WiaItem itm, string pfx)
        {
            string ch = itm.ItemID;

            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Resolution : " + itm.Resolution.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Extend (mm) : " + itm.Extend.ToString();
            ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Bpp    : " + itm.BitsPerPixel;

            for (int i = 0; i < itm.FormatsCount; i++)
                ch += SpecialChars.NewLine + pfx + SpecialChars.Tab + "Format = " + itm.GetFormatID(i);

            for (int i = 0; i < itm.PropertiesCount; i++)
            {
                WiaProperty prp = itm.GetProperty(i);
                ch += SpecialChars.NewLine + prp.DebugString(pfx + SpecialChars.Tab);
                prp.Dispose();
            }

            for (int i = 0; i < itm.CommandsCount; i++)
            {
                WiaCommand cmd = itm.GetCommand(i);
                ch += SpecialChars.NewLine + cmd.DebugString(pfx + SpecialChars.Tab);
                cmd.Dispose();
            }

            for (int i = 0; i < itm.SubItemsCount; i++)
            {
                WiaItem sitm = itm.GetSubItem(i);
                ch += SpecialChars.NewLine + sitm.DebugString(pfx + SpecialChars.Tab);
                sitm.Dispose();
            }

            return ch;
        }

        private static int zFindProperty(string nam, List<WIA.Property> prps)
        {
            return prps.FindIndex(p => nam.EqualsWithoutCase(p.Name));
        }

        private static RectangleF zGetExtend(WIA.Properties prps)
        {
            return new RectangleF(ConvertShop.ReadFloat(prps[KPropExtendX].get_Value()),
                                  ConvertShop.ReadFloat(prps[KPropExtendY].get_Value()),
                                  ConvertShop.ReadFloat(prps[KPropExtendW].get_Value()),
                                  ConvertShop.ReadFloat(prps[KPropExtendH].get_Value()));
        }

        private static PointF zGetResolution(WIA.Properties prps)
        {
            return new PointF(ConvertShop.ReadFloat(prps[KPropResolX].get_Value()), ConvertShop.ReadFloat(prps[KPropResolY].get_Value()));
        }

        private static void zSetExtend(WIA.Properties prps, RectangleF rct)
        {
            rct = Rectangle.Truncate(rct);       // -- 31.07.14

            prps[KPropExtendX].set_Value(rct.X);
            prps[KPropExtendY].set_Value(rct.Y);
            prps[KPropExtendW].set_Value(rct.Width);
            prps[KPropExtendH].set_Value(rct.Height);
        }

        private static void zSetResolution(WIA.Properties prps, PointF res)
        {
            prps[KPropResolX].set_Value(res.X);
            prps[KPropResolY].set_Value(res.Y);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}