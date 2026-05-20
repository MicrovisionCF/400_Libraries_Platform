using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microvision.NativeMethods;
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

        public static User32.RID_DEVICE_INFO GetDeviceInfo(IntPtr hdevice)
        {
            User32.RID_DEVICE_INFO infos = default;

            uint lng = 0;
            uint erc = User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_DEVICEINFOField, IntPtr.Zero, ref lng);
            if (erc == 0)
            {
                Bytes bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                MarshShop.StructToBuffer(lng, bf, 0);
                User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_DEVICEINFOField, hbf.Address(0), ref lng);
                hbf.Free();

                infos = new User32.RID_DEVICE_INFO((int)lng);

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
            User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_DEVICENAME, IntPtr.Zero, ref strsize);
            IntPtr pData = Marshal.AllocHGlobal(((int)strsize) * 2);
            User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_DEVICENAME, pData, ref strsize);

            string name = Marshal.PtrToStringAuto(pData)?.TrimEnd('\0') ?? "Unidentified";

            return name;
        }

        public static List<User32.RAWINPUTDEVICELIST> GetDevicesList()
        {
            int devicesCount = 0;

            List<User32.RAWINPUTDEVICELIST> lst = [];

            if (User32.GetRawInputDeviceList(IntPtr.Zero, ref devicesCount, (uint)MarshShop.SizeOf<User32.RAWINPUTDEVICELIST>()) == 0)
            {
                lst.Resize(devicesCount);
                LockList<User32.RAWINPUTDEVICELIST> hlst = new LockList<User32.RAWINPUTDEVICELIST>(lst);
                User32.GetRawInputDeviceList(hlst.Address(0), ref devicesCount, checked((uint)MarshShop.SizeOf<User32.RAWINPUTDEVICELIST>()));
                hlst.Free();
            }

            return lst;
        }

        public static Bytes GetPreparsedData(IntPtr hdevice)
        {
            Bytes bf = new Bytes();
            uint lng = 0;
            uint erc = User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_PREPARSEDDATAField, IntPtr.Zero, ref lng);

            if (erc == 0 && lng > 0)
            {
                bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                User32.GetRawInputDeviceInfo(hdevice, User32.RIDIType.RIDI_PREPARSEDDATAField, hbf.Address(0), ref lng);
                hbf.Free();
            }

            return bf;
        }

        public static User32.RAWINPUT GetRawInput(IntPtr hinput)
        {
            User32.RAWINPUT output = default;
            uint lng = 0;
            uint erc = User32.GetRawInputData(hinput, User32.RIDType.RID_INPUTField, IntPtr.Zero, ref lng, MarshShop.SizeOf<User32.RAWINPUTHEADER>());

            if (erc == 0)
            {
                Bytes bf = new Bytes((int)lng);
                LockTable<byte> hbf = new LockTable<byte>(bf.Array, bf.Length);
                User32.GetRawInputData(hinput, User32.RIDType.RID_INPUTField, hbf.Address(0), ref lng, MarshShop.SizeOf<User32.RAWINPUTHEADER>());
                hbf.Free();

                output = new User32.RAWINPUT((int)lng);

                int bfpos = 0;
                bfpos = (bfpos + MarshShop.BufferToStruct(bf, bfpos, out output.header));
                bfpos = (bfpos + MarshShop.BufferToBytes(bf, bfpos, output.datalen, output.data));
            }

            return output;
        }

        public static User32.RAWINPUTHEADER GetRawInputHeader(IntPtr hinput)
        {
            uint lng = 0;
            User32.RAWINPUTHEADER output = default;
            uint erc = User32.GetRawInputData(hinput, User32.RIDType.RID_HEADER, IntPtr.Zero, ref lng, MarshShop.SizeOf<User32.RAWINPUTHEADER>());

            if (erc == 0 && lng == MarshShop.SizeOf<User32.RAWINPUTHEADER>())
            {
                IntPtr hhdr = MarshShop.LockStruct(output);
                User32.GetRawInputData(hinput, User32.RIDType.RID_HEADER, hhdr, ref lng, MarshShop.SizeOf<User32.RAWINPUTHEADER>());
                output = MarshShop.UnlockStruct<User32.RAWINPUTHEADER>(hhdr);
            }

            return output;
        }

        public static bool RegisterDevice(List<Hid.USAGE_AND_PAGE> uups, IntPtr hwnd)
        {
            int nb = uups.Count;
            List<User32.RAWINPUTDEVICE> lst = new List<User32.RAWINPUTDEVICE>();
            for (int i = 0; i < nb; i++)
                lst.Add(new User32.RAWINPUTDEVICE((ushort)uups[i].UsagePage, (ushort)uups[i].Usage, hwnd));

            LockList<User32.RAWINPUTDEVICE> hlst = new LockList<User32.RAWINPUTDEVICE>(lst);
            bool ok = User32.RegisterRawInputDevices(hlst.Address(0), (uint)hlst.Count, (uint)MarshShop.SizeOf<User32.RAWINPUTDEVICE>());
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