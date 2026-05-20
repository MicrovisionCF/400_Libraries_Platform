using System;
using System.Collections.Generic;

using Microvision.Collections;
using Microvision.NativeMethods;

namespace Microvision.HID
{
    internal class HIDDevices : BaseList<HIDDevice>
    {
        // ***************************************************************************************************
        // 28.10.14 : (création)
        // 19.09.16 : héritage BaseList
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void InputChangeEventHandler(int no, User32.RAWINPUT inpt);

        public event InputChangeEventHandler? InputChange;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDDevices() : base()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public int Find(IntPtr hdev)
        {
            return zFind(hdev, _items);
        }

        public IntPtr GetHandle(int no)
        {
            return _items[no].Handle;
        }

        public HIDDevice GetItem(int no)
        {
            return _items[no];
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oSetHandlers(HIDDevice obj, bool status)
        {
            if (status)
                obj.InputChange += _item_InputChange;
            else
                obj.InputChange -= _item_InputChange;

            base.oSetHandlers(obj, status);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static int zFind(IntPtr hdev, IReadOnlyList<HIDDevice> lst)
        {
            return lst.FindIndex(d => hdev == d.Handle);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _item_InputChange(HIDDevice sender, User32.RAWINPUT inpt)
        {
            InputChange?.Invoke(_items.IndexOf(sender), inpt);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}