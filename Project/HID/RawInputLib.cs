using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.HID
{
    public sealed class RawInputLib
    {
        // ***************************************************************************************************
        // 24.10.14 : (création) accès à l'API Raw Input (incluse dans User32), qui permet de lire les données
        //            de tout périphérique HID, y compris souris et clavier. Sauf que seuls les joysticks
        //            m'intéressent.
        // 19.09.16 : GetDevicesList et RegisterDevice à list
        // 06.04.17 : des bytes en remplacement de tableaux. C'est risqué en principe car les structures sont
        //            publiques, mais ça a l'air de passer avec le seul utilisateur de ces structures, JoystickHID.plg.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct RAWHID
        {
            public int dwSizeHid;
            public int dwCount;
            public Bytes bRawData;      // -- dwSizeHid * dwCount

            public RAWHID(int bnb)
            {
                dwSizeHid = 0;
                dwCount = 0;
                bRawData = new Bytes(bnb - MarshShop.SizeOf(dwSizeHid) - MarshShop.SizeOf(dwCount));
            }

            public int rawlen
            {
                get
                {
                    return dwSizeHid * dwCount;
                }
            }
        }

        public struct RAWINPUT
        {
            public RAWINPUTHEADER header;
            public Bytes data;  // -- RAWMOUSE ou RAWKEYBOARD ou RAWHID

            public RAWINPUT(int bnb)
            {
                header = new RAWINPUTHEADER();
                data = new Bytes(bnb - MarshShop.SizeOf(header));
            }

            public int datalen
            {
                get
                {
                    return data.Length;
                }
            }

            public RAWHID hid()
            {
                RAWHID h = new RAWHID(datalen);
                int bfpos = 0;
                bfpos += MarshShop.BufferToStruct(data, bfpos, out h.dwSizeHid);
                bfpos += MarshShop.BufferToStruct(data, bfpos, out h.dwCount);
                bfpos += MarshShop.BufferToBytes(data, bfpos, h.rawlen, h.bRawData);

                return h;
            }

            public RAWKEYBOARD keyboard()
            {
                MarshShop.BufferToStruct(data, 0, out RAWKEYBOARD k);
                return k;
            }

            public RAWMOUSE mouse()
            {
                MarshShop.BufferToStruct(data, 0, out RAWMOUSE m);
                return m;
            }

            public static bool operator ==(RAWINPUT a, RAWINPUT b)
            {
                return a.header == b.header && zBytesEquals(a.data, b.data);
            }

            public static bool operator !=(RAWINPUT a, RAWINPUT b)
            {
                return a.header != b.header || !zBytesEquals(a.data, b.data);
            }

            private static bool zBytesEquals(Bytes a, Bytes b)
            {
                bool output = false;

                if (a.Array is null && b.Array is null)
                    output = true;
                else if (a.Array is not null && b.Array is not null && a.Length == b.Length)
                    output = Enumerable.SequenceEqual(a.Array, b.Array);

                return output;
            }

            public override bool Equals(object obj)
            {
                return obj is RAWINPUT objT && this == objT;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + header.GetHashCode();
                    hash = hash * 23 + data.GetHashCode();
                    return hash;
                }
            }
        }

        public struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            public RIM dwType;
        }

        public struct RAWINPUTHEADER
        {
            public RIM dwType;
            public int dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;

            public static bool operator ==(RAWINPUTHEADER a, RAWINPUTHEADER b)
            {
                return a.dwType == b.dwType && a.dwSize == b.dwSize && a.hDevice == b.hDevice && a.wParam == b.wParam;
            }

            public static bool operator !=(RAWINPUTHEADER a, RAWINPUTHEADER b)
            {
                return a.dwType != b.dwType || a.dwSize != b.dwSize || a.hDevice != b.hDevice || a.wParam != b.wParam;
            }

            public override bool Equals(object obj)
            {
                return obj is RAWINPUTHEADER objT && this == objT;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + dwType.GetHashCode();
                    hash = hash * 23 + dwSize.GetHashCode();
                    hash = hash * 23 + hDevice.GetHashCode();
                    hash = hash * 23 + wParam.GetHashCode();
                    return hash;
                }
            }
        }

        public struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;         // -- KeyboardFlags
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct RAWMOUSE
        {
            public uint usFlags;     // -- MouseFlags
            public ushort usButtonFlags; // -- MouseButtonFlags
            public short usButtonData;
            public uint ulRawButtons;
            public int lLastX;
            public int lLastY;
            public uint ulExtraInformation;
        }

        public struct RID_DEVICE_INFO
        {
            public int cbSize;
            public int dwType;   // -- RIM
            public Bytes info;      // -- RID_DEVICE_INFO_MOUSE ou RID_DEVICE_INFO_KEYBOARD ou RID_DEVICE_INFO_HID

            public RID_DEVICE_INFO(int cbsize)
            {
                cbSize = cbsize;
                dwType = 0;
                info = new Bytes(cbsize - MarshShop.SizeOf(cbSize) - MarshShop.SizeOf(dwType));
            }

            public int infolen
            {
                get
                {
                    return info.Length;
                }
            }

            public RID_DEVICE_INFO_HID hid()
            {
                MarshShop.BufferToStruct(info, 0, out RID_DEVICE_INFO_HID h);
                return h;
            }

            public RID_DEVICE_INFO_KEYBOARD keyboard()
            {
                MarshShop.BufferToStruct(info, 0, out RID_DEVICE_INFO_KEYBOARD k);
                return k;
            }

            public RID_DEVICE_INFO_MOUSE mouse()
            {
                MarshShop.BufferToStruct(info, 0, out RID_DEVICE_INFO_MOUSE m);
                return m;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct RID_DEVICE_INFO_HID
        {
            public int dwVendorId;
            public int dwProductId;
            public int dwVersionNumber;
            public ushort usUsagePage;
            public ushort usUsage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct RID_DEVICE_INFO_KEYBOARD
        {
            public int dwType;
            public int dwSubType;
            public int dwKeyboardMode;
            public int dwNumberOfFunctionKeys;
            public int dwNumberOfIndicators;
            public int dwNumberOfKeysTotal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct RID_DEVICE_INFO_MOUSE
        {
            public int dwId;
            public int dwNumberOfButtons;
            public int dwSampleRate;
            public short fHasHorizontalWheel;    // -- Boolean
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        private struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public RIDEVType dwFlags;
            public IntPtr hwndTarget;

            public RAWINPUTDEVICE(ushort upage, ushort usage, IntPtr hwnd)
            {
                usUsagePage = upage;
                usUsage = usage;
                dwFlags = RIDEVType.RIDEV_INPUTSINKField | RIDEVType.RIDEV_DEVNOTIFYField;
                hwndTarget = hwnd;
            }
        }


        public enum DevChgParam
        {
            GIDC_ARRIVAL = 1,        // -- A new device has been added to the system.
            GIDC_REMOVAL = 2        // -- A device has been removed from the system.
        }

        public enum InputParam
        {
            RIM_INPUT = 0,           // -- Input occurred while the application was in the foreground. The application must call DefWindowProc so the system can perform cleanup.
            RIM_INPUTSINK = 1       // -- Input occurred while the application was not in the foreground. The application must call DefWindowProc so the system can perform the cleanup.
        }

        public enum KeyboardFlags
        {
            RI_KEY_BREAK = 1,    // The key is up.
            RI_KEY_E0 = 2,       // This is the left version of the key.
            RI_KEY_E1 = 4,       // This is the right version of the key.
            RI_KEY_MAKE = 0     // The key is down.
        }

        [Flags]
        public enum MouseButtonFlags
        {
            RI_MOUSE_LEFT_BUTTON_DOWN = 0x1, // Left button changed to down.
            RI_MOUSE_LEFT_BUTTON_UP = 0x2, // Left button changed to up.
            RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x10, // Middle button changed to down.
            RI_MOUSE_MIDDLE_BUTTON_UP = 0x20, // Middle button changed to up.
            RI_MOUSE_RIGHT_BUTTON_DOWN = 0x4, // Right button changed to down.
            RI_MOUSE_RIGHT_BUTTON_UP = 0x8,    // Right button changed to up.
            RI_MOUSE_BUTTON_1_DOWN = 0x1, // RI_MOUSE_LEFT_BUTTON_DOWN
            RI_MOUSE_BUTTON_1_UP = 0x2, // RI_MOUSE_LEFT_BUTTON_UP
            RI_MOUSE_BUTTON_2_DOWN = 0x4, // RI_MOUSE_RIGHT_BUTTON_DOWN
            RI_MOUSE_BUTTON_2_UP = 0x8, // RI_MOUSE_RIGHT_BUTTON_UP
            RI_MOUSE_BUTTON_3_DOWN = 0x10, // RI_MOUSE_MIDDLE_BUTTON_DOWN
            RI_MOUSE_BUTTON_3_UP = 0x20, // RI_MOUSE_MIDDLE_BUTTON_UP
            RI_MOUSE_BUTTON_4_DOWN = 0x40, // XBUTTON1 changed to down.
            RI_MOUSE_BUTTON_4_UP = 0x80, // XBUTTON1 changed to up.
            RI_MOUSE_BUTTON_5_DOWN = 0x100, // XBUTTON2 changed to down.
            RI_MOUSE_BUTTON_5_UP = 0x200, // XBUTTON2 changed to up.
            RI_MOUSE_WHEEL = 0x400 // Raw input comes from a mouse wheel. The wheel delta is stored in usButtonData.
        }

        public enum MouseFlags
        {
            MOUSE_ATTRIBUTES_CHANGED = 0x4,  // Mouse attributes changed; application needs to query the mouse attributes.
            MOUSE_MOVE_RELATIVE = 0x0,       // Mouse movement data is relative to the last mouse position.
            MOUSE_MOVE_ABSOLUTE = 0x1,       // Mouse movement data is based on absolute position.
            MOUSE_VIRTUAL_DESKTOP = 0x2     // Mouse coordinates are mapped to the virtual desktop (for a multiple monitor system).
        }

        public enum RawInputMsg
        {
            WM_INPUT = 0xFF,
            WM_INPUT_DEVICE_CHANGE = 0xFE
        }

        public enum RawKeyboardMsg
        {
            WM_KEYDOWN = 0x100,      // Posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
            WM_KEYUP = 0x101,        // Posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus.
            WM_SYSKEYDOWN = 0x104,   // Posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
            WM_SYSKEYUP = 0x105     // Posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
            // -- etc...
        }

        public enum RIM
        {
            RIM_TYPEMOUSEField = 0,
            RIM_TYPEKEYBOARDField = 1,    // The device is a keyboard.
            RIM_TYPEHIDField = 2         // The device is an HID that is not a keyboard and not a mouse.
        }

        private enum RIDType
        {
            RID_HEADER = 0x10000005,     // -- Get the header information from the RAWINPUT structure.
            RID_INPUTField = 0x10000003      // -- Get the raw data from the RAWINPUT structure.
        }

        private enum RIDEVType
        {
            RIDEV_APPKEYS = 0x400,       // -- If set, the application command keys are handled. RIDEV_APPKEYS can be specified only if RIDEV_NOLEGACY is specified for a keyboard device.
            RIDEV_CAPTUREMOUSEField = 0x200,  // -- If set, the mouse button click does not activate the other window.
            RIDEV_DEVNOTIFYField = 0x2000,    // -- If set, this enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.
            // Windows XP:This flag is not supported until Windows Vista
            RIDEV_EXCLUDEField = 0x10,        // -- If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with RIDEV_PAGEONLY.
            RIDEV_EXINPUTSINKField = 0x1000,  // -- If set, this enables the caller to receive input in the background only if the foreground application does not process it. In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
            // Windows XP:This flag is not supported until Windows Vista
            RIDEV_INPUTSINKField = 0x100,     // -- If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that hwndTarget must be specified.
            RIDEV_NOHOTKEYSField = 0x200,     // -- If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. RIDEV_NOHOTKEYS can be specified even if RIDEV_NOLEGACY is not specified and hwndTarget is NULL.
            RIDEV_NOLEGACYField = 0x30,       // -- If set, this prevents any devices specified by usUsagePage or usUsage from generating legacy messages. This is only for the mouse and keyboard. See Remarks.
            RIDEV_PAGEONLYField = 0x20,       // -- If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that usUsage must be zero. To exclude a particular top level collection, use RIDEV_EXCLUDE.
            RIDEV_REMOVEField = 0x1          // -- If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.
        }

        private enum RIDIType
        {
            RIDI_DEVICENAME = 0x20000007,    // -- pData points to a string that contains the device name.
            // For this uiCommand only, the value in pcbSize is the character count (not the byte count).
            RIDI_DEVICEINFOField = 0x2000000B,    // -- pData points to an RID_DEVICE_INFO structure.
            RIDI_PREPARSEDDATAField = 0x20000005 // -- pData points to the previously parsed data.
        }


        [DllImport("User32.dll")] private static extern uint GetRawInputData(IntPtr hRawInput, RIDType uiCommand, IntPtr pData, ref uint pcbSize, int cbSizeHeader);
        [DllImport("User32.dll")] private static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref int puiNumDevices, uint cbSize);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)] private static extern bool RegisterRawInputDevices(IntPtr pRawInputDevices, uint uiNumDevices, uint cbSize);
        [DllImport("User32.dll", EntryPoint = "GetRawInputDeviceInfoW", CharSet = CharSet.Unicode)] private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RIDIType uiCommand, IntPtr pData, ref uint pcbSize);
        [DllImport("User32.dll", EntryPoint = "GetRawInputDeviceInfoW", CharSet = CharSet.Unicode)] private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RIDIType uiCommand, string ch, ref uint pcbSize);


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        private RawInputLib()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static RID_DEVICE_INFO GetDeviceInfo(IntPtr hdevice)
        {
            RID_DEVICE_INFO infos = default;

            uint lng = 0;
            uint erc = GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_DEVICEINFOField, IntPtr.Zero, ref lng);
            if (erc == 0)
            {
                Bytes bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                MarshShop.StructToBuffer(lng, bf, 0);
                GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_DEVICEINFOField, hbf.Address(0), ref lng);
                hbf.Free();

                infos = new RID_DEVICE_INFO((int)lng);

                int bfpos = 0;
                bfpos += MarshShop.BufferToStruct(bf, bfpos, out infos.cbSize);
                bfpos += MarshShop.BufferToStruct(bf, bfpos, out infos.dwType);
                bfpos += MarshShop.BufferToBytes(bf, bfpos, infos.infolen, infos.info);
            }

            return infos;
        }

        public static string GetDeviceName(IntPtr hdevice)
        {
            // TODOC# à tester
            uint strsize = 0;
            GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_DEVICENAME, IntPtr.Zero, ref strsize);
            IntPtr pData = Marshal.AllocHGlobal(((int)strsize) * 2);
            RawInputLib.GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_DEVICENAME, pData, ref strsize);

            string name = Marshal.PtrToStringAuto(pData);

            return name.TrimEnd('\0');
        }

        public static List<RAWINPUTDEVICELIST> GetDevicesList()
        {
            int devicesCount = 0;

            List<RAWINPUTDEVICELIST> lst = null;
            if (GetRawInputDeviceList(IntPtr.Zero, ref devicesCount, (uint)MarshShop.SizeOf<RAWINPUTDEVICELIST>()) == 0)
            {
                lst = new List<RAWINPUTDEVICELIST>().Resize(devicesCount);
                LockList<RAWINPUTDEVICELIST> hlst = new LockList<RAWINPUTDEVICELIST>(lst);
                GetRawInputDeviceList(hlst.Address(0), ref devicesCount, checked((uint)MarshShop.SizeOf<RawInputLib.RAWINPUTDEVICELIST>()));
                hlst.Free();
            }

            return lst;
        }

        public static Bytes GetPreparsedData(IntPtr hdevice)
        {
            Bytes bf = new Bytes();
            uint lng = 0;
            uint erc = GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_PREPARSEDDATAField, IntPtr.Zero, ref lng);

            if (erc == 0 && lng > 0)
            {
                bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                GetRawInputDeviceInfo(hdevice, RIDIType.RIDI_PREPARSEDDATAField, hbf.Address(0), ref lng);
                hbf.Free();
            }

            return bf;
        }

        public static RAWINPUT GetRawInput(IntPtr hinput)
        {
            RAWINPUT output = default;
            uint lng = 0;
            uint erc = GetRawInputData(hinput, RIDType.RID_INPUTField, IntPtr.Zero, ref lng, MarshShop.SizeOf<RAWINPUTHEADER>());

            if (erc == 0)
            {
                Bytes bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                GetRawInputData(hinput, RIDType.RID_INPUTField, hbf.Address(0), ref lng, MarshShop.SizeOf<RAWINPUTHEADER>());
                hbf.Free();

                output = new RAWINPUT((int)lng);

                int bfpos = 0;
                bfpos = (bfpos + MarshShop.BufferToStruct(bf, bfpos, out output.header));
                bfpos = (bfpos + MarshShop.BufferToBytes(bf, bfpos, output.datalen, output.data));
            }

            return output;
        }

        public static RAWINPUTHEADER GetRawInputHeader(IntPtr hinput)
        {
            uint lng = 0;
            RAWINPUTHEADER output = default;
            uint erc = GetRawInputData(hinput, RIDType.RID_HEADER, IntPtr.Zero, ref lng, MarshShop.SizeOf<RAWINPUTHEADER>());

            if (erc == 0 && lng == MarshShop.SizeOf<RAWINPUTHEADER>())
            {
                IntPtr hhdr = MarshShop.LockStruct(output);
                GetRawInputData(hinput, RIDType.RID_HEADER, hhdr, ref lng, MarshShop.SizeOf<RAWINPUTHEADER>());
                output = MarshShop.UnlockStruct<RAWINPUTHEADER>(hhdr);
            }

            return output;
        }

        public static bool RegisterDevice(List<HIDLib.USAGE_AND_PAGE> uups, IntPtr hwnd)
        {
            int nb = uups.Count;
            List<RAWINPUTDEVICE> lst = new List<RAWINPUTDEVICE>();
            for (int i = 0; i < nb; i++)
                lst.Add(new RAWINPUTDEVICE((ushort)uups[i].UsagePage, (ushort)uups[i].Usage, hwnd));

            LockList<RAWINPUTDEVICE> hlst = new LockList<RAWINPUTDEVICE>(lst);
            bool ok = RegisterRawInputDevices(hlst.Address(0), (uint)hlst.Count, (uint)MarshShop.SizeOf<RAWINPUTDEVICE>());
            hlst.Free();

            return ok;
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