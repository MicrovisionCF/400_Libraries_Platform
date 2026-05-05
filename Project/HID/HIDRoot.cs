using Microvision.Types;

namespace Microvision.HID
{
    public class HIDRoot : Citizen
    {
        // ***************************************************************************************************
        // 27.10.14 : (création) liste de devices HID installés sur le poste, racine pour trouver un device
        //            donné.
        // 19.09.16 : _devices et _regUUPs as list(of)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void DeviceAddEventHandler(IntPtr hdev);
        public delegate void DeviceRemoveEventHandler(IntPtr hdev);
        public delegate void InputChangeEventHandler(int no, RawInputLib.RAWINPUT inpt);

        public event DeviceAddEventHandler? DeviceAdd;
        public event DeviceRemoveEventHandler? DeviceRemove;
        public event InputChangeEventHandler? InputChange;

        // ***************************************************************************************************

        private readonly List<RawInputLib.RAWINPUTDEVICELIST> _devices;
        private readonly List<HIDLib.USAGE_AND_PAGE> _regUUPs;
        private readonly HIDDevices _regDevs;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public HIDRoot() : base()
        {
            _devices = RawInputLib.GetDevicesList();
            _regUUPs = [];
            _regDevs = new HIDDevices();
            _regDevs_Attach(true);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int Count => _devices.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public int Find(IntPtr hdev)
        {
            return zFindHdl(hdev, _devices);
        }

        public int Find(HIDLib.SomeUsage us, HIDLib.SomeUsagePage uspg)
        {
            return zFindUUP(new HIDLib.USAGE_AND_PAGE(us, uspg), _devices);
        }

        public int Find(int vendorId, int productId)
        {
            return zFindVendorIdProductId(vendorId, productId, _devices);
        }

        public HIDDevice? GetDevice(int no)
        {
            HIDDevice? output = null;

            int rno = _regDevs.Find(_devices[no].hDevice);
            if (rno >= 0) output = _regDevs.GetItem(rno).AddLife();

            return output;
        }

        public IntPtr GetHandle(int no)
        {
            return _devices[no].hDevice;
        }

        public RawInputLib.RIM GetType(int no)
        {
            return _devices[no].dwType;
        }

        public void ProcessMessage(ref Message m)
        {
            switch ((RawInputLib.RawInputMsg)m.Msg)
            {
                case RawInputLib.RawInputMsg.WM_INPUT:
                    if (oProcessInput(m.LParam))
                        m.Result = IntPtr.Zero;
                    break;

                case RawInputLib.RawInputMsg.WM_INPUT_DEVICE_CHANGE:
                    _devices.Clear();
                    _devices.AddRange(RawInputLib.GetDevicesList());
                    IntPtr hdevice = m.LParam;

                    switch ((RawInputLib.DevChgParam)m.WParam)
                    {
                        case RawInputLib.DevChgParam.GIDC_ARRIVAL:
                            if (oProcessArrival(hdevice)) oOnDeviceAdd(hdevice);
                            m.Result = IntPtr.Zero;
                            break;

                        case RawInputLib.DevChgParam.GIDC_REMOVAL:
                            if (oProcessRemoval(hdevice)) oOnDeviceRemove(hdevice);
                            m.Result = IntPtr.Zero;
                            break;
                    }

                    break;
            }
        }

        public bool Register(IntPtr hwnd, HIDLib.SomeUsage us, HIDLib.SomeUsagePage uspg)
        {
            _regUUPs.Add(new HIDLib.USAGE_AND_PAGE(us, uspg));

            for (int i = 0; i < _devices.Count; i++)
                if (_regUUPs.IndexOf(zGetUsageAndPage(_devices[i].dwType, _devices[i].hDevice)) >= 0)
                    if (_regDevs.Find(_devices[i].hDevice) < 0)
                        _regDevs.Add(oCreateDevice(_devices[i].dwType, _devices[i].hDevice).GiveLife());

            return RawInputLib.RegisterDevice(_regUUPs, hwnd);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected virtual HIDDevice oCreateDevice(RawInputLib.RIM rim, IntPtr hdev)
        {
            HIDDevice dev;

            switch (rim)
            {
                case RawInputLib.RIM.RIM_TYPEMOUSEField:
                    dev = new HIDMouse(hdev);
                    break;

                case RawInputLib.RIM.RIM_TYPEKEYBOARDField:
                    dev = new HIDKeyboard(hdev);
                    break;

                case RawInputLib.RIM.RIM_TYPEHIDField:
                    HIDLib.USAGE_AND_PAGE uup = zGetUsageAndPage(rim, hdev);
                    dev = uup.Usage switch
                    {
                        HIDLib.SomeUsage.Joystick => new HIDJoystick(hdev),
                        HIDLib.SomeUsage.GamePad => new HIDJoystick(hdev),
                        _ => new HIDOther(hdev),
                    };
                    break;

                default:
                    throw new NotImplementedException(rim.ToString());
            }

            return dev;
        }

        protected override void oDispose(bool isExplicit)
        {
            _regDevs_Attach(false);
            if (isExplicit) _regDevs.Dispose();

            base.oDispose(isExplicit);
        }

        protected virtual void oOnDeviceAdd(IntPtr hdev)
        {
            DeviceAdd?.Invoke(hdev);
        }

        protected virtual void oOnDeviceRemove(IntPtr hdev)
        {
            DeviceRemove?.Invoke(hdev);
        }

        protected virtual bool oProcessArrival(IntPtr hdevice)
        {
            bool done = false;

            RawInputLib.RID_DEVICE_INFO inf = RawInputLib.GetDeviceInfo(hdevice);
            if (_regUUPs.IndexOf(zGetUsageAndPage((RawInputLib.RIM)inf.dwType, hdevice)) >= 0)
            {
                if (_regDevs.Find(hdevice) < 0)
                {
                    _regDevs.Add(oCreateDevice((RawInputLib.RIM)inf.dwType, hdevice).GiveLife());
                    done = true;
                }
            }

            return done;
        }

        protected bool oProcessInput(IntPtr hinput)
        {
            bool done = false;
            RawInputLib.RAWINPUTHEADER hdr = RawInputLib.GetRawInputHeader(hinput);
            int no = _regDevs.Find(hdr.hDevice);

            if (no >= 0)
                done = _regDevs.GetItem(no).ProcessInput(hinput);

            return done;
        }

        protected virtual bool oProcessRemoval(IntPtr hdevice)
        {
            bool done = false;
            int rno = _regDevs.Find(hdevice);

            if (rno >= 0)
            {
                _regDevs.Remove(rno);
                done = true;
            }

            return done;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static int zFindHdl(IntPtr hdev, List<RawInputLib.RAWINPUTDEVICELIST> lst)
        {
            return lst.FindIndex(d => hdev == d.hDevice);
        }

        private static int zFindUUP(HIDLib.USAGE_AND_PAGE uup, List<RawInputLib.RAWINPUTDEVICELIST> lst)
        {
            return lst.FindIndex(u => uup == zGetUsageAndPage(u.dwType, u.hDevice));
        }

        private static int zFindVendorIdProductId(int vendorId, int productId, List<RawInputLib.RAWINPUTDEVICELIST> lst)
        {
            return lst.FindIndex(device => zGetVendorIdProductId(device.hDevice) == (vendorId, productId));
        }

        private static HIDLib.USAGE_AND_PAGE zGetUsageAndPage(RawInputLib.RIM rim, IntPtr hdev)
        {
            HIDLib.USAGE_AND_PAGE output;

            switch (rim)
            {
                case RawInputLib.RIM.RIM_TYPEMOUSEField:
                    output = new HIDLib.USAGE_AND_PAGE(HIDLib.SomeUsage.Mouse, HIDLib.SomeUsagePage.GenericDesktopControls);
                    break;

                case RawInputLib.RIM.RIM_TYPEKEYBOARDField:
                    output = new HIDLib.USAGE_AND_PAGE(HIDLib.SomeUsage.Keyboard, HIDLib.SomeUsagePage.GenericDesktopControls);
                    break;

                case RawInputLib.RIM.RIM_TYPEHIDField:
                    RawInputLib.RID_DEVICE_INFO_HID inf = RawInputLib.GetDeviceInfo(hdev).hid();
                    output = new HIDLib.USAGE_AND_PAGE((HIDLib.SomeUsage)inf.usUsage, (HIDLib.SomeUsagePage)inf.usUsagePage);
                    break;

                default:
                    output = default;
                    break;
            }

            return output;
        }

        private static (int vendorId, int productId) zGetVendorIdProductId(IntPtr hdev)
        {
            RawInputLib.RID_DEVICE_INFO_HID inf = RawInputLib.GetDeviceInfo(hdev).hid();

            return (inf.dwVendorId, inf.dwProductId);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _regDevs_Attach(bool attach)
        {
            if (attach)
            {
                _regDevs.InputChange += _regDevs_InputChange;
            }
            else
            {
                _regDevs.InputChange -= _regDevs_InputChange;
            }
        }

        private void _regDevs_InputChange(int no, RawInputLib.RAWINPUT inpt)
        {
            InputChange?.Invoke(zFindHdl(_regDevs.GetHandle(no), _devices), inpt);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}