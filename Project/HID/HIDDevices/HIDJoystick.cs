using System;
using System.Collections.Generic;

using Microvision.NativeMethods;

namespace Microvision.HID
{
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
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public delegate void AxesValueChangeEventHandler(HIDJoystick js, List<int> v);
        public delegate void ButtonsPressedChangeEventHandler(HIDJoystick js, int which);

        public event AxesValueChangeEventHandler? AxesValueChange;
        public event ButtonsPressedChangeEventHandler? ButtonsPressedChange;

        // ***************************************************************************************************

        private readonly List<Hid.HIDP_BUTTON_CAPS> _buttonsCaps;
        private readonly List<Hid.HIDP_VALUE_CAPS> _valuesCaps;
        private readonly List<int> _axValues;

        private int _buttonsPressed;

        private bool _processAllButtonInputs;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDJoystick(IntPtr hdl) : base(hdl)
        {
            _buttonsCaps = HIDLib.GetButtonsCaps(_preparsedData, _caps);
            _valuesCaps = HIDLib.GetValuesCaps(_preparsedData, _caps);
            _axValues = [.. new List<int>().Resize(_valuesCaps.Count, 0)];

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

        public Hid.SomeUsage GetAxisID(int no)
        {
            return (Hid.SomeUsage)_valuesCaps[no].UsageMin;
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

        protected override bool oProcessInput(IntPtr handleInput)
        {
            bool changed = false;

            if (base.oProcessInput(handleInput) || _processAllButtonInputs)
            {
                bool buttonChanged = false;
                User32.RAWHID hid = _lastInput.Hid();
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

        private static int zButtonsCount(Hid.HIDP_BUTTON_CAPS buttonCaps)
        {
            return 1 + buttonCaps.UsageMax - buttonCaps.UsageMin;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }

}