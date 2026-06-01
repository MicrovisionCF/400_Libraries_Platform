using System;
using System.Runtime.InteropServices;

using Microvision.NativeMethods;

namespace Microvision.Authenticode
{
    public class SignShop
    {
        // ***************************************************************************************************
        // 11.10.23 : Création
        // 13.05.26 : (libs 4.0) Envoi des bases vers Platform
        // ***************************************************************************************************


        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static bool IsSigned(string filePath)
        {
            Wintrust.WINTRUST_FILE_INFO file = new Wintrust.WINTRUST_FILE_INFO();
            file.cbStruct = Marshal.SizeOf(typeof(Wintrust.WINTRUST_FILE_INFO));
            file.pcwszFilePath = filePath;

            Wintrust.WINTRUST_DATA data = new Wintrust.WINTRUST_DATA();
            data.cbStruct = Marshal.SizeOf(typeof(Wintrust.WINTRUST_DATA));
            data.dwUIChoice = Wintrust.WTD_UI_NONE;
            data.dwUnionChoice = Wintrust.WTD_CHOICE_FILE;
            data.fdwRevocationChecks = Wintrust.WTD_REVOKE_NONE;
            data.pFile = Marshal.AllocHGlobal(file.cbStruct);
            Marshal.StructureToPtr(file, data.pFile, false);

            int hr;

            try
            {
                hr = Wintrust.WinVerifyTrust(Wintrust.INVALID_HANDLE_VALUE, Wintrust.WINTRUST_ACTION_GENERIC_VERIFY_V2, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(data.pFile);
            }

            return hr == 0;
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