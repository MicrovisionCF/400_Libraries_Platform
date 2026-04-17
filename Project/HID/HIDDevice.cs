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
        // ***************************************************************************************************

        public delegate void InputChangeEventHandler(HIDDevice sender, RawInputLib.RAWINPUT inpt);

        public event InputChangeEventHandler InputChange;

        // ***************************************************************************************************

        protected RawInputLib.RIM _rim;
        protected IntPtr _handle;

        protected string _name;
        protected RawInputLib.RID_DEVICE_INFO _info;

        protected HIDLib.SomeUsagePage _usagePage;
        protected HIDLib.SomeUsage _usage;

        protected RawInputLib.RAWINPUT _lastInput;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected HIDDevice(RawInputLib.RIM rim, IntPtr hdl) : base()
        {
            _rim = rim;
            _handle = hdl;

            _name = RawInputLib.GetDeviceName(_handle);
            _info = RawInputLib.GetDeviceInfo(_handle);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public IntPtr Handle => _handle;

        public string Name => _name;

        public HIDLib.SomeUsage Usage => _usage;

        public HIDLib.SomeUsagePage UsagePage => _usagePage;


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static string HexBytes(Bytes bf, int charsCount, int rowsCount)
        {
            return zHexBytes(bf, 0, bf.Length, charsCount, rowsCount);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool ProcessInput(IntPtr hinput)
        {
            if (oProcessInput(hinput)) oOnInputChange(_lastInput);

            return true;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected virtual void oOnInputChange(RawInputLib.RAWINPUT inpt)
        {
            InputChange?.Invoke(this, inpt);
        }

        protected virtual bool oProcessInput(IntPtr hinput)
        {
            bool changed = false;
            RawInputLib.RAWINPUT inpt = RawInputLib.GetRawInput(hinput);

            if (inpt != _lastInput)
            {
                _lastInput = inpt;
                changed = true;
            }

            return changed;
        }

        protected void oSetUsage(HIDLib.SomeUsagePage uspg, HIDLib.SomeUsage us)
        {
            _usagePage = uspg;
            _usage = us;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zHexBytes(Bytes bf, int bfpos, int buffLength, int charsCount, int linesCount)
        {
            int length = bfpos + charsCount * linesCount < buffLength ? charsCount * linesCount : buffLength - bfpos;
            string s = "";

            for (int i = 0; i < length; i++)
            {
                s += bf[bfpos + i].ToString("X2");
                if ((i + 1) % charsCount == 0)
                    s += SpecialChars.NewLine;
                else if ((i + 1) % 4 == 0)
                    s += "   ";
                else
                    s += " ";
            }

            if (s.EndsWith(SpecialChars.NewLine))
                s = s.Substring(0, s.Length - SpecialChars.NewLine.Length);
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

    public class HIDJoystick : HIDOther
    {
        // ***************************************************************************************************
        // 28.10.14 : (création) HIDOther avec décodage des inputs à l'aide de HID.dll, tout pompé sur
        //            article de Alexander Böcken, 2011, CodeProject.
        // 19.09.16 : _buttonsCaps, _valuesCaps et _axValues as list, ajout d'un événement à liste
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 15.09.25 : Ajout de ProcessAllButtonInputs pour gérer les joysticks qui n'envoient pas de front descendant
        //            lorsqu'on relâche un bouton (c'est le cas de l'endoscope "ENDO-CAM" de Foretec).
        // ***************************************************************************************************

        public delegate void AxesValueChangeEventHandler(HIDJoystick js, List<int> v);
        public delegate void ButtonsPressedChangeEventHandler(HIDJoystick js, int which);

        public event AxesValueChangeEventHandler AxesValueChange;
        public event ButtonsPressedChangeEventHandler ButtonsPressedChange;

        // ***************************************************************************************************

        private List<HIDLib.HIDP_BUTTON_CAPS> _buttonsCaps;
        private List<HIDLib.HIDP_VALUE_CAPS> _valuesCaps;

        private int _buttonsPressed;
        private List<int> _axValues;

        private bool _processAllButtonInputs;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDJoystick(IntPtr hdl) : base(hdl)
        {
            _buttonsCaps = HIDLib.GetButtonsCaps(_preparsedData, _caps);
            _valuesCaps = HIDLib.GetValuesCaps(_preparsedData, _caps);
            _axValues = new List<int>().Resize(_valuesCaps.Count, 0).ToList();

            _processAllButtonInputs = false;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int AxesCount => _valuesCaps.Count;

        public int ButtonsCount => zButtonsCount(_buttonsCaps[0]);

        public int ButtonsPressed => _buttonsPressed;

        public bool ProcessAllButtonInputs
        {
            get => _processAllButtonInputs;

            set
            {
                if (value != _processAllButtonInputs)
                {
                    _processAllButtonInputs = value;
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public HIDLib.SomeUsage GetAxisID(int no)
        {
            return (HIDLib.SomeUsage)_valuesCaps[no].UsageMin;
        }

        public int GetAxisMax(int no)
        {
            return _valuesCaps[no].LogicalMax;
        }

        public int GetAxisMin(int no)
        {
            return _valuesCaps[no].LogicalMin;
        }

        public int GetAxisValue(int no)
        {
            return _axValues[no];
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _buttonsCaps = null;
            _valuesCaps = null;
            _axValues = null;

            base.oDispose(isExplicit);
        }

        protected virtual void oOnAxesValueChange(List<int> values)
        {
            AxesValueChange?.Invoke(this, values);
        }

        protected virtual void oOnButtonsPressedChange(int which)
        {
            ButtonsPressedChange?.Invoke(this, which);
        }

        protected override bool oProcessInput(IntPtr hinput)
        {
            bool changed = false;

            if (base.oProcessInput(hinput) || _processAllButtonInputs)
            {
                bool buttonChanged = false;
                RawInputLib.RAWHID hid = _lastInput.hid();
                int btns = HIDLib.GetButtonsPressed(_preparsedData, _buttonsCaps[0], hid.bRawData, hid.dwSizeHid);
                if ((btns != _buttonsPressed) || _processAllButtonInputs)
                {
                    _buttonsPressed = btns;
                    buttonChanged = true;
                }

                bool valueChanged = false;
                for (int i = 0; i < _valuesCaps.Count; i++)
                {
                    int v = HIDLib.GetValueValue(_preparsedData, _valuesCaps[i], hid.bRawData, hid.dwSizeHid);
                    if (v != _axValues[i])
                    {
                        _axValues[i] = v;
                        valueChanged = true;
                    }
                }

                if (buttonChanged) oOnButtonsPressedChange(_buttonsPressed);
                if (valueChanged) oOnAxesValueChange(_axValues);

                changed = true;
            }

            return changed;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static int zButtonsCount(HIDLib.HIDP_BUTTON_CAPS bcps)
        {
            return 1 + bcps.UsageMax - bcps.UsageMin;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }

    public class HIDKeyboard : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) ébauche pour claviers HID
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void KeyDownEventHandler(HIDKeyboard sender, Keys k);
        public delegate void KeyUpEventHandler(HIDKeyboard sender, Keys k);
        public delegate void SysKeyDownEventHandler(HIDKeyboard sender, Keys k);
        public delegate void SysKeyUpEventHandler(HIDKeyboard sender, Keys k);

        public event KeyDownEventHandler KeyDown;
        public event KeyUpEventHandler KeyUp;
        public event SysKeyDownEventHandler SysKeyDown;
        public event SysKeyUpEventHandler SysKeyUp;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDKeyboard(IntPtr hdl) : base(RawInputLib.RIM.RIM_TYPEKEYBOARDField, hdl)
        {
            oSetUsage(HIDLib.SomeUsagePage.GenericDesktopControls, HIDLib.SomeUsage.Keyboard);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int KeyboardMode => _info.keyboard().dwKeyboardMode;

        public int NumberOfFunctionKeys => _info.keyboard().dwNumberOfFunctionKeys;

        public int NumberOfIndicators => _info.keyboard().dwNumberOfIndicators;

        public int NumberOfKeysTotal => _info.keyboard().dwNumberOfKeysTotal;

        public int SubType => _info.keyboard().dwSubType;

        public int Type => _info.keyboard().dwType;


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

        protected virtual void oOnKeyDown(Keys k)
        {
            KeyDown?.Invoke(this, k);
        }

        protected virtual void oOnKeyUp(Keys k)
        {
            KeyUp?.Invoke(this, k);
        }

        protected virtual void oOnSysKeyDown(Keys k)
        {
            SysKeyDown?.Invoke(this, k);
        }

        protected virtual void oOnSysKeyUp(Keys k)
        {
            SysKeyUp?.Invoke(this, k);
        }

        protected override bool oProcessInput(IntPtr hinput)
        {
            bool changed = false;

            if (base.oProcessInput(hinput))
            {
                RawInputLib.RAWKEYBOARD kb = _lastInput.keyboard();

                switch ((RawInputLib.RawKeyboardMsg)kb.Message)
                {
                    case RawInputLib.RawKeyboardMsg.WM_KEYDOWN: oOnKeyDown((Keys)kb.VKey); break;
                    case RawInputLib.RawKeyboardMsg.WM_KEYUP: oOnKeyUp((Keys)kb.VKey); break;
                    case RawInputLib.RawKeyboardMsg.WM_SYSKEYDOWN: oOnSysKeyDown((Keys)kb.VKey); break;
                    case RawInputLib.RawKeyboardMsg.WM_SYSKEYUP: oOnSysKeyUp((Keys)kb.VKey); break;
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

    public class HIDMouse : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) ébauche pour souris HID
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void MouseChangeEventHandler(HIDMouse sender, RawInputLib.MouseButtonFlags btns, int x, int y, bool fabsolute);
        public delegate void MouseWheelEventHandler(HIDMouse sender, int dy);

        public event MouseChangeEventHandler MouseChange;
        public event MouseWheelEventHandler MouseWheel;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDMouse(IntPtr hdl) : base(RawInputLib.RIM.RIM_TYPEMOUSEField, hdl)
        {
            oSetUsage(HIDLib.SomeUsagePage.GenericDesktopControls, HIDLib.SomeUsage.Mouse);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool HasHorizontalWheel => _info.mouse().fHasHorizontalWheel != 0;

        public int Id => _info.mouse().dwId;

        public int NumberOfButtons => _info.mouse().dwNumberOfButtons;

        public int SampleRate => _info.mouse().dwSampleRate;


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

        protected virtual void oOnMouseChange(RawInputLib.MouseButtonFlags btns, int x, int y, bool fabs)
        {
            MouseChange?.Invoke(this, btns, x, y, fabs);
        }

        protected virtual void oOnMouseWheel(int dlt)
        {
            MouseWheel?.Invoke(this, dlt);
        }

        protected override bool oProcessInput(IntPtr hinput)
        {
            base.oProcessInput(hinput);

            // -- je soupçonne qu'il faille traiter tous les messages, même s'ils ne changent pas
            RawInputLib.RAWMOUSE m = _lastInput.mouse();
            RawInputLib.MouseButtonFlags btns = (RawInputLib.MouseButtonFlags)(m.usButtonFlags & (int)~RawInputLib.MouseButtonFlags.RI_MOUSE_WHEEL);

            if (btns != 0 || m.lLastX != 0 || m.lLastY != 0)
                oOnMouseChange(btns, m.lLastX, m.lLastY, (m.usFlags & (long)RawInputLib.MouseFlags.MOUSE_MOVE_ABSOLUTE) != 0L);

            if ((m.usButtonFlags & (int)RawInputLib.MouseButtonFlags.RI_MOUSE_WHEEL) != 0)
                oOnMouseWheel(m.usButtonData);

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

    public class HIDOther : HIDDevice
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) périphérique HID ni clavier ni souris
        // 20.09.16 : _preparsedData as list
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected Bytes _preparsedData;
        protected HIDLib.HIDP_CAPS _caps;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDOther(IntPtr hdl) : base(RawInputLib.RIM.RIM_TYPEHIDField, hdl)
        {
            _preparsedData = RawInputLib.GetPreparsedData(_handle);
            _caps = HIDLib.GetCaps(_preparsedData);
            oSetUsage((HIDLib.SomeUsagePage)_info.hid().usUsagePage, (HIDLib.SomeUsage)_info.hid().usUsage);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int ProductId => _info.hid().dwProductId;

        public int VendorId => _info.hid().dwVendorId;

        public int VersionNumber => _info.hid().dwVersionNumber;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _preparsedData = default;
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