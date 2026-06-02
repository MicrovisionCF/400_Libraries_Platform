using System;
using System.Collections.Generic;

using Microvision.NativeMethods;
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

        public static List<Hid.HIDP_BUTTON_CAPS> GetButtonsCaps(Bytes desc, Hid.HIDP_CAPS cps)
        {
            // -- le 1er élément de btns décrit TOUS les boutons
            // les suivants décrivent des usages particuliers définis par le constructeur.

            List<Hid.HIDP_BUTTON_CAPS> bcps = new List<Hid.HIDP_BUTTON_CAPS>().Resize(cps.NumberInputButtonCaps);
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            IntPtr hcps = MarshShop.LockStructs(bcps);

            ushort bcpsCount = (ushort)bcps.Count;
            Hid.HidP_GetButtonCaps(Hid.HIDP_REPORT_TYPE.HidP_Input, hcps, ref bcpsCount, hdesc.Address(0));

            MarshShop.UnlockStructs(hcps, bcps);
            hdesc.Free();
            return bcps;
        }

        public static int GetButtonsPressed(Bytes desc, Hid.HIDP_BUTTON_CAPS bcaps, Bytes rprt, int rptlg)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            LockTable<byte> hrprt = new LockTable<byte>(rprt.Array, rprt.Length);

            uint usgnb = (uint)(1 + bcaps.UsageMax - bcaps.UsageMin);
            List<ushort> usgs = new List<ushort>().Resize((int)usgnb);
            LockList<ushort> husg = new LockList<ushort>(usgs);

            Hid.HIDP_STATUS erc = Hid.HidP_GetUsages(Hid.HIDP_REPORT_TYPE.HidP_Input, bcaps.UsagePage, bcaps.LinkCollection, husg.Address(0), ref usgnb, hdesc.Address(0), hrprt.Address(0), (uint)rptlg);

            husg.Free();
            hrprt.Free();
            hdesc.Free();

            int btns = 0;

            if (erc == Hid.HIDP_STATUS.SUCCESS)
                for (int i = 0; i < usgnb; i++)
                    btns |= ((int)Math.Pow(2, usgs[i] - bcaps.UsageMin));

            return btns;
        }

        public static Hid.HIDP_CAPS GetCaps(Bytes desc)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            Hid.HIDP_CAPS cps = default;
            Hid.HIDP_STATUS erc = Hid.HidP_GetCaps(hdesc.Address(0), ref cps);
            hdesc.Free();

            if (erc != Hid.HIDP_STATUS.SUCCESS)
                cps = default;

            return cps;
        }

        public static List<Hid.HIDP_VALUE_CAPS> GetValuesCaps(Bytes desc, Hid.HIDP_CAPS cps)
        {
            List<Hid.HIDP_VALUE_CAPS> vcps = new List<Hid.HIDP_VALUE_CAPS>().Resize(cps.NumberInputValueCaps);
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            IntPtr hcps = MarshShop.LockStructs(vcps);

            ushort cnt = (ushort)vcps.Count;
            Hid.HidP_GetValueCaps(Hid.HIDP_REPORT_TYPE.HidP_Input, hcps, ref cnt, hdesc.Address(0));

            MarshShop.UnlockStructs(hcps, vcps);
            hdesc.Free();

            return vcps;
        }

        public static int GetValueValue(Bytes desc, Hid.HIDP_VALUE_CAPS vcps, Bytes rprt, int rptlg)
        {
            LockTable<byte> hdesc = new LockTable<byte>(desc.Array, desc.Length);
            LockTable<byte> hrprt = new LockTable<byte>(rprt.Array, rprt.Length);

            uint v = 0;
            Hid.HIDP_STATUS erc = Hid.HidP_GetUsageValue(Hid.HIDP_REPORT_TYPE.HidP_Input, vcps.UsagePage, vcps.LinkCollection, vcps.UsageMin, ref v, hdesc.Address(0), hrprt.Address(0), (uint)rptlg);

            hrprt.Free();
            hdesc.Free();

            if (erc != Hid.HIDP_STATUS.SUCCESS) v = 0;

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