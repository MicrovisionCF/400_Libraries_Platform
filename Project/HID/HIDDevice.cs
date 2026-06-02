using System;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.HID
{
    public abstract class HIDDevice : Citizen
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) classe de base pour HIDMouse, HIDKeyboard et HIDOther
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public delegate void InputChangeEventHandler(HIDDevice sender, User32.RAWINPUT inpt);

        public event InputChangeEventHandler? InputChange;

        // ***************************************************************************************************

        protected readonly User32.RIM _rim;
        protected readonly IntPtr _handle;

        protected readonly string _name;
        protected readonly User32.RID_DEVICE_INFO _info;

        protected Hid.SomeUsagePage _usagePage;
        protected Hid.SomeUsage _usage;

        protected User32.RAWINPUT _lastInput;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected HIDDevice(User32.RIM rim, IntPtr handle) : base()
        {
            _rim = rim;
            _handle = handle;

            _name = RawInputLib.GetDeviceName(_handle);
            _info = RawInputLib.GetDeviceInfo(_handle);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public IntPtr Handle => _handle;

        public string Name => _name;

        public Hid.SomeUsage Usage => _usage;

        public Hid.SomeUsagePage UsagePage => _usagePage;


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static string HexBytes(Bytes buffer, int charsCount, int rowsCount)
        {
            return zHexBytes(buffer, 0, buffer.Length, charsCount, rowsCount);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool ProcessInput(IntPtr handleInput)
        {
            if (oProcessInput(handleInput)) oOnInputChange(_lastInput);

            return true;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected virtual void oOnInputChange(User32.RAWINPUT input)
        {
            InputChange?.Invoke(this, input);
        }

        protected virtual bool oProcessInput(IntPtr handleInput)
        {
            bool changed = false;
            User32.RAWINPUT inpt = RawInputLib.GetRawInput(handleInput);

            if (inpt != _lastInput)
            {
                _lastInput = inpt;
                changed = true;
            }

            return changed;
        }

        protected void oSetUsage(Hid.SomeUsagePage usagePage, Hid.SomeUsage usage)
        {
            _usagePage = usagePage;
            _usage = usage;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zHexBytes(Bytes src, int srcOffset, int buffLength, int charsCount, int linesCount)
        {
            int length = srcOffset + charsCount * linesCount < buffLength ? charsCount * linesCount : buffLength - srcOffset;
            string s = "";

            for (int i = 0; i < length; i++)
            {
                s += src[srcOffset + i].ToString("X2");
                if ((i + 1) % charsCount == 0)
                    s += SpecialChars.NewLine;
                else if ((i + 1) % 4 == 0)
                    s += "   ";
                else
                    s += " ";
            }

            if (s.EndsWith(SpecialChars.NewLine))
                s = s[..^SpecialChars.NewLine.Length];
            if (length >= charsCount * linesCount)
                s += "   ......";

            return s;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}