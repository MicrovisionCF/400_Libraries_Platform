using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.HID
{
    public sealed class HIDLib
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) accès à la librairie HID.dll, qui elle-même accède au driver HDIClass.
        //            Il y a beaucoup de choses dans cette librairie, seules sont implantées ici les routines
        //            de décodage pour joystick qui complètent la librairie RawInput, de plus haut niveau.
        // 19.09.16 : surcharges à list, et tous accès à HID.dll via des LockList.
        // 12.04.17 : changement d'avis, surcharges à bytes, et tous accès à HID.dll via des LockTable.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public struct USAGE_AND_PAGE
        {
            public SomeUsage Usage;
            public SomeUsagePage UsagePage;

            public USAGE_AND_PAGE(SomeUsage us, SomeUsagePage uspg)
            {
                Usage = us;
                UsagePage = uspg;
            }

            public static bool operator ==(USAGE_AND_PAGE a, USAGE_AND_PAGE b)
            {
                return a.Usage == b.Usage && a.UsagePage == b.UsagePage;
            }

            public static bool operator !=(USAGE_AND_PAGE a, USAGE_AND_PAGE b)
            {
                return a.Usage != b.Usage || a.UsagePage != b.UsagePage;
            }

            public override bool Equals(object obj)
            {
                return obj is USAGE_AND_PAGE objT && objT == this;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Usage.GetHashCode();
                    hash = hash * 23 + UsagePage.GetHashCode();
                    return hash;
                }
            }
        }

        public struct HIDP_CAPS
        {
            public ushort Usage;
            public ushort UsagePage;
            public ushort InputReportByteLength;
            public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps;
            public ushort NumberInputValueCaps;
            public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps;
            public ushort NumberOutputValueCaps;
            public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps;
            public ushort NumberFeatureValueCaps;
            public ushort NumberFeatureDataIndices;
        }

        public struct HIDP_BUTTON_CAPS
        {
            public ushort UsagePage;         // -- cf SomeUsagePage
            public byte ReportID;
            public byte IsAlias;             // -- Boolean
            public ushort BitField;
            public ushort LinkCollection;    // --// A unique internal index pointer
            public ushort LinkUsage;
            public ushort LinkUsagePage;
            public byte IsRange;             // -- Boolean
            public byte IsStringRange;       // -- Boolean
            public byte IsDesignatorRange;   // -- Boolean
            public byte IsAbsolute;          // -- Boolean
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public uint[] Reserved;
            public ushort UsageMin;           // ... si Range, Usage si NotRange
            public ushort UsageMax;           // ... si Range, rien si NotRange
            public ushort StringMin;          // ... si Range, StringIndex si NotRange
            public ushort StringMax;          // ... si Range, rien si NotRange
            public ushort DesignatorMin;      // ... si Range, Designator si NotRange
            public ushort DesignatorMax;      // ... si Range, rien si NotRange
            public ushort DataIndexMin;       // ... si Range, DataIndex si NotRange
            public ushort DataIndexMax;       // ... si Range, rien si NotRange
        }

        public struct HIDP_VALUE_CAPS
        {
            public ushort UsagePage;
            public byte ReportID;
            public byte IsAlias;             // -- boolean
            public ushort BitField;
            public ushort LinkCollection;
            public ushort LinkUsage;
            public ushort LinkUsagePage;
            public byte IsRange;             // -- boolean
            public byte IsStringRange;       // -- boolean
            public byte IsDesignatorRange;   // -- boolean
            public byte IsAbsolute;          // -- boolean
            public byte HasNull;             // -- boolean
            public byte Reserved;
            public ushort BitSize;
            public ushort ReportCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public ushort[] Reserved2;
            public uint UnitsExp;
            public uint Units;
            public int LogicalMin;
            public int LogicalMax;
            public int PhysicalMin;
            public int PhysicalMax;
            public ushort UsageMin;
            public ushort UsageMax;
            public ushort StringMin;
            public ushort StringMax;
            public ushort DesignatorMin;
            public ushort DesignatorMax;
            public ushort DataIndexMin;
            public ushort DataIndexMax;
        }


        public enum SomeUsage  // -- extrait d'exemples
        {
            // -- génériques
            Mouse = 0x2,
            Joystick = 0x4,
            GamePad = 0x5,
            Keyboard = 0x6,
            XAxis = 0x30,
            YAxis = 0x31,
            ZAxis = 0x32,
            XRotate = 0x33,
            YRotate = 0x34,
            ZRotate = 0x35,
            Wheel = 0x38,
            HatSwitch = 0x39,

            // -- téléphonie
            Chaispas = 0x0,
            AnsweringMachine = 0x2
        }

        public enum SomeUsagePage
        {
            GenericDesktopControls = 0x1,
            Button = 0x9,
            Telephone = 0xB
        }

        private enum HIDP_REPORT_TYPE
        {
            HidP_Input,
            HidP_Output,
            HidP_Feature
        }

        private enum HIDP_STATUS
        {
            SUCCESS = 0x110000,
            NULL = int.MinValue + 0x00110001,
            INVALID_PREPARSED_DATA = int.MinValue + 0x40110001,
            INVALID_REPORT_TYPE = int.MinValue + 0x40110002,
            INVALID_REPORT_LENGTH = int.MinValue + 0x40110003,
            USAGE_NOT_FOUND = int.MinValue + 0x40110004,
            VALUE_OUT_OF_RANGE = int.MinValue + 0x40110005,
            BAD_LOG_PHY_VALUES = int.MinValue + 0x40110006,
            BUFFER_TOO_SMALL = int.MinValue + 0x40110007,
            INTERNAL_ERROR = int.MinValue + 0x40110008,
            I8042_TRANS_UNKNOWN = int.MinValue + 0x40110009,
            INCOMPATIBLE_REPORT_ID = int.MinValue + 0x4011000A,
            NOT_VALUE_ARRAY = int.MinValue + 0x4011000B,
            IS_VALUE_ARRAY = int.MinValue + 0x4011000C,
            DATA_INDEX_NOT_FOUND = int.MinValue + 0x4011000D,
            DATA_INDEX_OUT_OF_RANGE = int.MinValue + 0x4011000E,
            BUTTON_NOT_PRESSED = int.MinValue + 0x4011000F,
            REPORT_DOES_NOT_EXIST = int.MinValue + 0x40110010,
            NOT_IMPLEMENTED = int.MinValue + 0x40110020
        }


        [DllImport("HID.dll")] private static extern HIDP_STATUS HidP_GetButtonCaps(HIDP_REPORT_TYPE ReportType, IntPtr ButtonCaps, ref ushort ButtonCapsLength, IntPtr PreparsedData);
        [DllImport("HID.dll")] private static extern HIDP_STATUS HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);
        [DllImport("HID.dll")] private static extern HIDP_STATUS HidP_GetUsages(HIDP_REPORT_TYPE ReportType, ushort UsagePage, ushort LinkCollection, IntPtr UsageList, ref uint UsageLength, IntPtr PreparsedData, IntPtr Report, uint ReportLength);
        [DllImport("HID.dll")] private static extern HIDP_STATUS HidP_GetUsageValue(HIDP_REPORT_TYPE ReportType, ushort UsagePage, ushort LinkCollection, ushort Usage, ref uint UsageValue, IntPtr PreparsedData, IntPtr Report, uint ReportLength);
        [DllImport("HID.dll")] private static extern HIDP_STATUS HidP_GetValueCaps(HIDP_REPORT_TYPE ReportType, IntPtr ValueCaps, ref ushort ValueCapsLength, IntPtr PreparsedData);


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        private HIDLib()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static List<HIDP_BUTTON_CAPS> GetButtonsCaps(Bytes desc, HIDP_CAPS cps)
        {
            // -- le 1er élément de btns décrit TOUS les boutons
            // les suivants décrivent des usages particuliers définis par le constructeur.

            List<HIDP_BUTTON_CAPS> bcps = new List<HIDP_BUTTON_CAPS>().Resize(cps.NumberInputButtonCaps);
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            IntPtr hcps = MarshShop.LockStructs(bcps);

            ushort bcpsCount = (ushort)bcps.Count;
            HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, hcps, ref bcpsCount, hdesc.Address(0));

            MarshShop.UnlockStructs(hcps, bcps);
            hdesc.Free();
            return bcps;
        }

        public static int GetButtonsPressed(Bytes desc, HIDP_BUTTON_CAPS bcaps, Bytes rprt, int rptlg)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            LockTable<byte> hrprt = new LockTable<byte>(rprt.Array, rprt.Length);

            uint usgnb = (uint)(1 + bcaps.UsageMax - bcaps.UsageMin);
            List<ushort> usgs = new List<ushort>().Resize((int)usgnb);
            LockList<ushort> husg = new LockList<ushort>(usgs);

            HIDP_STATUS erc = HidP_GetUsages(HIDP_REPORT_TYPE.HidP_Input, bcaps.UsagePage, bcaps.LinkCollection, husg.Address(0), ref usgnb, hdesc.Address(0), hrprt.Address(0), (uint)rptlg);

            husg.Free();
            hrprt.Free();
            hdesc.Free();

            int btns = 0;

            if (erc == HIDP_STATUS.SUCCESS)
                for (int i = 0; i < usgnb; i++)
                    btns = (btns | ((int)Math.Pow(2, usgs[i] - bcaps.UsageMin)));

            return btns;
        }

        public static HIDP_CAPS GetCaps(Bytes desc)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            HIDP_CAPS cps = default;
            HIDP_STATUS erc = HidP_GetCaps(hdesc.Address(0), ref cps);
            hdesc.Free();

            if (erc != HIDP_STATUS.SUCCESS)
                cps = default;

            return cps;
        }

        public static List<HIDP_VALUE_CAPS> GetValuesCaps(Bytes desc, HIDP_CAPS cps)
        {
            List<HIDP_VALUE_CAPS> vcps = new List<HIDP_VALUE_CAPS>().Resize(cps.NumberInputValueCaps);
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            IntPtr hcps = MarshShop.LockStructs(vcps);

            ushort cnt = (ushort)vcps.Count;
            HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, hcps, ref cnt, hdesc.Address(0));

            MarshShop.UnlockStructs(hcps, vcps);
            hdesc.Free();

            return vcps;
        }

        public static int GetValueValue(Bytes desc, HIDP_VALUE_CAPS vcps, Bytes rprt, int rptlg)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            LockTable<byte> hrprt = new LockTable<byte>(rprt.Array, rprt.Length);

            uint v = 0;
            HIDP_STATUS erc = HidP_GetUsageValue(HIDP_REPORT_TYPE.HidP_Input, vcps.UsagePage, vcps.LinkCollection, vcps.UsageMin, ref v, hdesc.Address(0), hrprt.Address(0), (uint)rptlg);

            hrprt.Free();
            hdesc.Free();

            if (erc != HIDP_STATUS.SUCCESS) v = 0;

            return (int)v;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


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