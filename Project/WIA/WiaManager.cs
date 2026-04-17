using System.Diagnostics;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaManager : Citizen
    {
        // ***************************************************************************************************
        // 08.02.13 : (ébauche) architecture écrite pour être intégrée à µV.Platform, donc usage de µV.Types
        //            uniquement.
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private WIA.DeviceManager _manager;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public WiaManager() : base()
        {
            _manager = new WIA.DeviceManager();
            _manager_Attach(true);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int DevicesCount => _manager.DeviceInfos.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugManager(this, pfx);
        }

        public WiaDeviceInfo GetDeviceInfo(int no)
        {
            return new WiaDeviceInfo(_manager.DeviceInfos.ToList()[no]);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_manager is not null)
            {
                _manager_Attach(false);
                Marshal.ReleaseComObject(_manager);
                _manager = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugManager(WiaManager dmng, string pfx)
        {
            string s = "";

            for (int i = 0; i < dmng.DevicesCount; i++)
            {
                WiaDeviceInfo dinf = dmng.GetDeviceInfo(i);
                s = s + SpecialChars.NewLine + dinf.DebugString(pfx + SpecialChars.Tab);
                dinf.Dispose();
            }

            return s;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _manager_Attach(bool attach)
        {
            if (attach)
            {
                _manager.OnEvent += _manager_OnEvent;
            }
            else
            {
                _manager.OnEvent -= _manager_OnEvent;
            }
        }

        private void _manager_OnEvent(string EventID, string DeviceID, string ItemID)
        {
            Debug.Print(EventID, DeviceID, ItemID);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}