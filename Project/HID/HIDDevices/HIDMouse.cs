using System;

using Microvision.NativeMethods;

namespace Microvision.HID
{
    public class HIDMouse : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) ébauche pour souris HID
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public delegate void MouseChangeEventHandler(HIDMouse sender, User32.MouseButtonFlags buttonsFlags, int x, int y, bool isPosAbsolute);
        public delegate void MouseWheelEventHandler(HIDMouse sender, int deltaY);

        public event MouseChangeEventHandler? MouseChange;
        public event MouseWheelEventHandler? MouseWheel;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDMouse(IntPtr handle) : base(User32.RIM.RIM_TYPEMOUSEField, handle)
        {
            oSetUsage(Hid.SomeUsagePage.GenericDesktopControls, Hid.SomeUsage.Mouse);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool HasHorizontalWheel => _info.Mouse().fHasHorizontalWheel != 0;

        public int Id => _info.Mouse().dwId;

        public int NumberOfButtons => _info.Mouse().dwNumberOfButtons;

        public int SampleRate => _info.Mouse().dwSampleRate;


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

        protected virtual void oOnMouseChange(User32.MouseButtonFlags buttonsFlags, int x, int y, bool isPosAbsolute)
        {
            MouseChange?.Invoke(this, buttonsFlags, x, y, isPosAbsolute);
        }

        protected virtual void oOnMouseWheel(int dlt)
        {
            MouseWheel?.Invoke(this, dlt);
        }

        protected override bool oProcessInput(IntPtr handleInput)
        {
            base.oProcessInput(handleInput);

            // -- je soupçonne qu'il faille traiter tous les messages, même s'ils ne changent pas
            User32.RAWMOUSE mouse = _lastInput.Mouse();
            User32.MouseButtonFlags btns = (User32.MouseButtonFlags)(mouse.usButtonFlags & (int)~User32.MouseButtonFlags.RI_MOUSE_WHEEL);

            if (btns != 0 || mouse.lLastX != 0 || mouse.lLastY != 0)
                oOnMouseChange(btns, mouse.lLastX, mouse.lLastY, (mouse.usFlags & (long)User32.MouseFlags.MOUSE_MOVE_ABSOLUTE) != 0L);

            if ((mouse.usButtonFlags & (int)User32.MouseButtonFlags.RI_MOUSE_WHEEL) != 0)
                oOnMouseWheel(mouse.usButtonData);

            return true;
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