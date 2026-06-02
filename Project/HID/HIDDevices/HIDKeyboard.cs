using System;
using System.Windows.Forms;

using Microvision.NativeMethods;

namespace Microvision.HID
{
    public class HIDKeyboard : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) ébauche pour claviers HID
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public delegate void KeyDownEventHandler(HIDKeyboard sender, Keys key);
        public delegate void KeyUpEventHandler(HIDKeyboard sender, Keys key);
        public delegate void SysKeyDownEventHandler(HIDKeyboard sender, Keys key);
        public delegate void SysKeyUpEventHandler(HIDKeyboard sender, Keys key);

        public event KeyDownEventHandler? KeyDown;
        public event KeyUpEventHandler? KeyUp;
        public event SysKeyDownEventHandler? SysKeyDown;
        public event SysKeyUpEventHandler? SysKeyUp;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDKeyboard(IntPtr handle) : base(User32.RIM.RIM_TYPEKEYBOARDField, handle)
        {
            oSetUsage(Hid.SomeUsagePage.GenericDesktopControls, Hid.SomeUsage.Keyboard);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int KeyboardMode => _info.Keyboard().dwKeyboardMode;

        public int NumberOfFunctionKeys => _info.Keyboard().dwNumberOfFunctionKeys;

        public int NumberOfIndicators => _info.Keyboard().dwNumberOfIndicators;

        public int NumberOfKeysTotal => _info.Keyboard().dwNumberOfKeysTotal;

        public int SubType => _info.Keyboard().dwSubType;

        public int Type => _info.Keyboard().dwType;


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

        protected virtual void oOnKeyDown(Keys key)
        {
            KeyDown?.Invoke(this, key);
        }

        protected virtual void oOnKeyUp(Keys key)
        {
            KeyUp?.Invoke(this, key);
        }

        protected virtual void oOnSysKeyDown(Keys key)
        {
            SysKeyDown?.Invoke(this, key);
        }

        protected virtual void oOnSysKeyUp(Keys key)
        {
            SysKeyUp?.Invoke(this, key);
        }

        protected override bool oProcessInput(IntPtr handleInput)
        {
            bool changed = false;

            if (base.oProcessInput(handleInput))
            {
                User32.RAWKEYBOARD kb = _lastInput.Keyboard();

                switch ((User32.RawKeyboardMsg)kb.Message)
                {
                    case User32.RawKeyboardMsg.WM_KEYDOWN: oOnKeyDown((Keys)kb.VKey); break;
                    case User32.RawKeyboardMsg.WM_KEYUP: oOnKeyUp((Keys)kb.VKey); break;
                    case User32.RawKeyboardMsg.WM_SYSKEYDOWN: oOnSysKeyDown((Keys)kb.VKey); break;
                    case User32.RawKeyboardMsg.WM_SYSKEYUP: oOnSysKeyUp((Keys)kb.VKey); break;
                }

                changed = true;
            }

            return changed;
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