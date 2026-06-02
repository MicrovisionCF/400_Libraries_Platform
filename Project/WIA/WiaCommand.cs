using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaCommand : Citizen
    {
        // ***************************************************************************************************
        // 11.02.13 : création. Sur Epson 1680, Description et Name sont identiques.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly WIA.DeviceCommand _command;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaCommand(WIA.DeviceCommand cmd) : base()
        {
            _command = cmd;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string CommandID => _command.CommandID;

        public string Description => _command.Description;

        public string Name => _command.Name;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return $"{pfx}{GetType().Name} = {zDebugCommand(this)}";
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            Marshal.ReleaseComObject(_command);

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugCommand(WiaCommand cmd)
        {
            return cmd.Name;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}