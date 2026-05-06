using System.Runtime.InteropServices;

namespace Microvision.NativeMethods
{
    internal static class Winmm
    {

        [DllImport(nameof(Winmm), EntryPoint = "mmioStringToFOURCCA")]
        public static extern int mmioStringToFOURCC(string sz, int uFlags); // returns fourcc
    }
}
