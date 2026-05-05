using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaDevice : Citizen
    {
        // ***************************************************************************************************
        // 08.02.13 : ébauche
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0) Les constantes Interop ne sont plus référencables
        // ***************************************************************************************************

        public enum DeviceType // -- enum dupliquée pour ne pas imposer de référence à WIA aux utilisateurs de la librairie
        {
            UnspecifiedDeviceType = WIA.WiaDeviceType.UnspecifiedDeviceType,
            ScannerDeviceType = WIA.WiaDeviceType.ScannerDeviceType,
            CameraDeviceType = WIA.WiaDeviceType.CameraDeviceType,
            VideoDeviceType = WIA.WiaDeviceType.VideoDeviceType
        }


        public static string wiaCommandChangeDocument = "{04E725B0-ACAE-11D2-A093-00C04F72DC3C}"; // WIA.CommandID.wiaCommandChangeDocument;
        public static string wiaCommandDeleteAllItems = "{E208C170-ACAD-11D2-A093-00C04F72DC3C}"; // WIA.CommandID.wiaCommandDeleteAllItems;
        public static string wiaCommandSynchronize = "{9B26B7B2-ACAD-11D2-A093-00C04F72DC3C}"; // WIA.CommandID.wiaCommandSynchronize;
        public static string wiaCommandTakePicture = "{AF933CAC-ACAD-11D2-A093-00C04F72DC3C}"; //  WIA.CommandID.wiaCommandTakePicture;
        public static string wiaCommandUnloadDocument = "{1F3B3D8E-ACAE-11D2-A093-00C04F72DC3C}"; // WIA.CommandID.wiaCommandUnloadDocument;
        public static string wiaEventDeviceConnected = "{A28BBADE-64B6-11D2-A231-00C04FA31809}"; //  WIA.EventID.wiaEventDeviceConnected;
        public static string wiaEventDeviceDisconnected = "{143E4E83-6497-11D2-A231-00C04FA31809}"; //  WIA.EventID.wiaEventDeviceDisconnected;
        public static string wiaEventItemCreated = "{4C8F4EF5-E14F-11D2-B326-00C04F68CE61}"; // WIA.EventID.wiaEventItemCreated;
        public static string wiaEventItemDeleted = "{1D22A559-E14F-11D2-B326-00C04F68CE61}"; // WIA.EventID.wiaEventItemDeleted;
        public static string wiaEventScanEmailImage = "{C686DCEE-54F2-419E-9A27-2FC7F2E98F9E}"; // WIA.EventID.wiaEventScanEmailImage;
        public static string wiaEventScanFaxImage = "{C00EB793-8C6E-11D2-977A-0000F87A926F}"; // WIA.EventID.wiaEventScanFaxImage;
        public static string wiaEventScanFilmImage = "{9B2B662C-6185-438C-B68B-E39EE25E71CB}"; // WIA.EventID.wiaEventScanFilmImage;
        public static string wiaEventScanImage = "{A6C5A715-8C6E-11D2-977A-0000F87A926F}"; // WIA.EventID.wiaEventScanImage;
        public static string wiaEventScanImage2 = "{FC4767C1-C8B3-48A2-9CFA-2E90CB3D3590}"; // WIA.EventID.wiaEventScanImage2;
        public static string wiaEventScanImage3 = "{154E27BE-B617-4653-ACC5-0FD7BD4C65CE}"; // WIA.EventID.wiaEventScanImage3;
        public static string wiaEventScanImage4 = "{A65B704A-7F3C-4447-A75D-8A26DFCA1FDF}"; // WIA.EventID.wiaEventScanImage4;
        public static string wiaEventScanOCRImage = "{9D095B89-37D6-4877-AFED-62A297DC6DBE}"; // WIA.EventID.wiaEventScanOCRImage;
        public static string wiaEventScanPrintImage = "{B441F425-8C6E-11D2-977A-0000F87A926F}"; // WIA.EventID.wiaEventScanPrintImage;


        private const string KPropBedSizeX = "Horizontal Bed Size";
        private const string KPropBedSizeY = "Vertical Bed Size";
        private const string KPropName = "Name";
        private const string KPropOptResolX = "Horizontal Optical Resolution";
        private const string KPropOptResolY = "Vertical Optical Resolution";
        private const string KPropPreview = "Preview";
        private const float KMMPerBedUnit = 25.4f / 1000;

        // -- commands constatées sur Epson Expression 1680 :
        // Synchronize	
        // Delete device tree	
        // Build device tree	

        // -- items constatés sur Epson Expression 1680 :
        // 0000\Root\Top

        // -- propriétés constatées sur Epson Expression 1680 :
        // Item Name	String	Root
        // Full Item Name	String	0000\Root
        // Item Flags	Integer	76

        // (idem WiaDeviceInfo)
        // Unique Device ID	String	{6BDD1FC6-810F-11D0-BEC7-08002BE2092F}\0000
        // Manufacturer	String	EPSON
        // Description	String	EPSON Expression 1680
        // Type	Integer	65537
        // Port	String	\\.\Usbscan1
        // Name	String	EPSON Expression 1680
        // Server	String	local
        // Remote Device ID	String	
        // UI Class ID	String	{00000000-0000-0000-0000-000000000000}
        // Hardware Configuration	Integer	0
        // BaudRate	String	
        // STI Generic Capabilities	Integer	19
        // WIA Version	String	2.0
        // Driver Version	String	2.0.1.1
        // PnP ID String	String	\\?\usb#vid_04b8&pid_010e#6&1ba46697&0&4#{6bdd1fc6-810f-11d0-bec7-08002be2092f}
        // STI Driver Version	Integer	2

        // Horizontal Bed Size	Integer	8500
        // Vertical Bed Size	Integer	11700
        // Access Rights	6	3
        // Horizontal Optical Resolution	Integer	1600
        // Vertical Optical Resolution	Integer	1600
        // Firmware Version	String	1.00
        // Max Scan Time	Integer	1800000

        private readonly WIA.Device _device;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaDevice(WIA.Device dev) : base()
        {
            _device = dev;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public SizeF BedSize // mm
        {
            get
            {
                return new SizeF(ConvertShop.ReadFloat(_device.Properties[KPropBedSizeX].get_Value()) * KMMPerBedUnit,
                                ConvertShop.ReadFloat(_device.Properties[KPropBedSizeY].get_Value()) * KMMPerBedUnit);
            }
        }

        public int CommandsCount => _device.Commands.Count;

        internal WIA.Device Core => _device;

        public string DeviceID => _device.DeviceID;

        public int EventsCount => _device.Events.Count;

        public bool HasPreview => _device.Properties.Exists(KPropPreview);

        public int ItemsCount => _device.Items.Count;

        public string Name => _device.Properties[KPropName].get_Value().ToString();

        public PointF OpticalResolution => new PointF(ConvertShop.ReadFloat(_device.Properties[KPropOptResolX].get_Value()), ConvertShop.ReadFloat(_device.Properties[KPropOptResolY].get_Value()));

        public bool PreviewMode
        {
            get
            {
                bool output = false;

                if (_device.Properties.Exists(KPropPreview))
                    output = ConvertShop.ReadInt(_device.Properties[KPropPreview].get_Value()) != 0;

                return output;
            }

            set => _device.Properties[KPropPreview].set_Value(value ? 1 : 0);
        }

        public int PropertiesCount => _device.Properties.Count;

        public DeviceType Type => (DeviceType)_device.Type;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugDevice(this, pfx);
        }

        public WiaItem ExecuteCommand(WiaCommand cmd)
        {
            return new WiaItem(_device.ExecuteCommand(cmd.CommandID));
        }

        public int FindProperty(string pnam)
        {
            return zFindProperty(pnam, _device.Properties.ToList());
        }

        public WiaCommand GetCommand(int no)
        {
            return new WiaCommand(_device.Commands.ToList()[no]);
        }

        public WiaEvent GetEvent(int no)
        {
            return new WiaEvent(_device.Events.ToList()[no]);
        }

        public WiaItem GetItem(int no)
        {
            return new WiaItem(_device.Items.ToList()[no]);
        }

        public WiaProperty GetProperty(int no)
        {
            return new WiaProperty(_device.Properties.ToList()[no]);
        }

        public bool HasProperty(string nam)
        {
            return _device.Properties.Exists(nam);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_device);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugDevice(WiaDevice dev, string pfx)
        {
            string ch = dev.Name + " (" + dev.Type.ToNameString() + ") " + dev.BedSize.ToString() + ", " + dev.OpticalResolution.ToString();

            for (int i = 0; i < dev.PropertiesCount; i++)
            {
                WiaProperty prp = dev.GetProperty(i);
                ch = ch + SpecialChars.NewLine + prp.DebugString(pfx + SpecialChars.Tab);
                prp.Dispose();
            }

            for (int i = 0; i < dev.CommandsCount; i++)
            {
                WiaCommand cmd = dev.GetCommand(i);
                ch = ch + SpecialChars.NewLine + cmd.DebugString(pfx + SpecialChars.Tab);
                cmd.Dispose();
            }

            for (int i = 0; i < dev.EventsCount; i++)
            {
                WiaEvent evt = dev.GetEvent(i);
                ch = ch + SpecialChars.NewLine + evt.DebugString(pfx + SpecialChars.Tab);
                evt.Dispose();
            }

            for (int i = 0; i < dev.ItemsCount; i++)
            {
                WiaItem itm = dev.GetItem(i);
                ch = ch + SpecialChars.NewLine + itm.DebugString(pfx + SpecialChars.Tab);
                itm.Dispose();
            }

            return ch;
        }

        private static int zFindProperty(string nam, List<WIA.Property> prps)
        {
            return prps.FindIndex(p => nam.EqualsWithoutCase(p.Name));
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}