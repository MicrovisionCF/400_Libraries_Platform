using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly WIA.DeviceManager _manager;


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
            return $"{pfx}{GetType().Name} = {zDebugManager(this, pfx)}";
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
            _manager_Attach(false);
            Marshal.ReleaseComObject(_manager);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugManager(WiaManager dmng, string pfx)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < dmng.DevicesCount; i++)
            {
                using WiaDeviceInfo dinf = dmng.GetDeviceInfo(i);
                s.AppendLine(dinf.DebugString(pfx + SpecialChars.Tab));
            }

            return s.ToString();
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