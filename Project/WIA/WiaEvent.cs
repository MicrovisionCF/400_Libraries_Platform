using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaEvent : Citizen
    {
        // ***************************************************************************************************
        // 19.02.13 : création.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public enum EventFlag
        {
            ActionEvent = WIA.WiaEventFlag.ActionEvent,
            NotificationEvent = WIA.WiaEventFlag.NotificationEvent
        }


        private readonly WIA.DeviceEvent _event;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaEvent(WIA.DeviceEvent evt) : base()
        {
            _event = evt;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string Description => _event.Description;

        public string EventID => _event.EventID;

        public string Name => _event.Name;

        public EventFlag Type => (EventFlag)_event.Type;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + this.GetType().Name + " = " + zDebugEvent(this);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_event);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugEvent(WiaEvent evt)
        {
            return evt.Name + " (" + evt.Type.ToNameString() + ")";
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}