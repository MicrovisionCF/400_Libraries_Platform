using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaDeviceInfo : Citizen
    {
        // ***************************************************************************************************
        // 08.02.13 : création.
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private const string KPropName = "Name";

        // -- propriétés constatées sur Epson Expression 1680 :
        // Unique Device ID	16	{6BDD1FC6-810F-11D0-BEC7-08002BE2092F}\0000
        // Manufacturer	16	EPSON
        // Description	16	EPSON Expression 1680
        // Type	5	65537
        // Port	16	\\.\Usbscan1
        // Name	16	EPSON Expression 1680
        // Server	16	local
        // Remote Device ID	16	
        // UI Class ID	16	{00000000-0000-0000-0000-000000000000}
        // Hardware Configuration	5	0
        // BaudRate	16	
        // STI Generic Capabilities	5	19
        // WIA Version	16	2.0
        // Driver Version	16	2.0.1.1
        // PnP ID String	16	\\?\usb#vid_04b8&pid_010e#6&1ba46697&0&4#{6bdd1fc6-810f-11d0-bec7-08002be2092f}
        // STI Driver Version	5	2

        private readonly WIA.DeviceInfo _devInfo;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaDeviceInfo(WIA.DeviceInfo devinf) : base()
        {
            _devInfo = devinf;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string DeviceID => _devInfo.DeviceID;

        public string Name => _devInfo.Properties[KPropName].get_Value().ToString();

        public int PropertiesCount => _devInfo.Properties.Count;

        public WiaDevice.DeviceType Type => (WiaDevice.DeviceType)_devInfo.Type;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public WiaDevice Connect()
        {
            return new WiaDevice(_devInfo.Connect());
        }

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugDeviceInfo(this, pfx);
        }

        public int FindProperty(string pnam)
        {
            return zFindProperty(pnam, _devInfo.Properties.ToList());
        }

        public WiaProperty GetProperty(int no)
        {
            return new WiaProperty(_devInfo.Properties.ToList()[no]);
        }

        public bool HasProperty(string name)
        {
            return _devInfo.Properties.Exists(name);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_devInfo);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugDeviceInfo(WiaDeviceInfo dinf, string pfx)
        {
            string ch = dinf.Name + " (" + dinf.Type.ToNameString() + ")";

            for (int i = 0; i < dinf.PropertiesCount; i++)
            {
                WiaProperty prp = dinf.GetProperty(i);
                ch += SpecialChars.NewLine + prp.DebugString(pfx + SpecialChars.Tab);
                prp.Dispose();
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