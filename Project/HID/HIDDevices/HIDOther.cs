using System;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.HID
{
    public class HIDOther : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) périphérique HID ni clavier ni souris
        // 20.09.16 : _preparsedData as list
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        protected readonly Bytes _preparsedData;
        protected readonly Hid.HIDP_CAPS _caps;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDOther(IntPtr hdl) : base(User32.RIM.RIM_TYPEHIDField, hdl)
        {
            _preparsedData = RawInputLib.GetPreparsedData(_handle);
            _caps = HIDLib.GetCaps(_preparsedData);
            oSetUsage((Hid.SomeUsagePage)_info.Hid().usUsagePage, (Hid.SomeUsage)_info.Hid().usUsage);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int ProductId => _info.Hid().dwProductId;

        public int VendorId => _info.Hid().dwVendorId;

        public int VersionNumber => _info.Hid().dwVersionNumber;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
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