using System.Runtime.InteropServices;

namespace Microvision.NativeMethods
{
    internal static class Winmm
    {
        // ***************************************************************************************************
        // 02.06.26 : Création avec les fonctions déjà utilisées
        // ***************************************************************************************************

        [DllImport(nameof(Winmm), EntryPoint = "mmioStringToFOURCCA")]
        public static extern int mmioStringToFOURCC(string sz, int uFlags); // returns fourcc
    }
}
