using System;
using System.Text;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.DDE
{
    public class WinDDELibrary : Citizen
    {
        // ***************************************************************************************************
        // 04.03.11 : (création) une "instance" de ddeml, avec les fonctions requises par les clients DDE.
        // 21.03.12 : libs 1.8, héritage Citizen.
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Identifiant en Integer et pas en IntPtr, Ajout d'envoi de valeur par Poke
        // 13.04.22 : (libs 3.0)
        // 23.11.22 : Utilisation de StringBuilder pour relire les chaines en mémoire (non fonctionnel en string)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private static int KCodePage = User32.CP_WINANSI; // -- pas encore décidé...
        private int _identifier;



        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public WinDDELibrary() : base()
        {
            _identifier = -1;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public IntPtr ClientTransaction(IntPtr handleConnection, IntPtr hitem, User32.XType typ)
        {
            int pdwResult = 0;
            return User32.DdeClientTransaction(IntPtr.Zero, 0, handleConnection, hitem, 1, (uint)typ, 1000, ref pdwResult);
        }

        public IntPtr ClientTransactionData(IntPtr dataHandle, IntPtr handleConnection, IntPtr hitem, User32.XType typ)
        {
            int pdwResult = 0;
            return User32.DdeClientTransaction(dataHandle, -1, handleConnection, hitem, 1, (uint)typ, 1000, ref pdwResult);
        }

        public IntPtr Connect(IntPtr handleStringServer, IntPtr handleStringTopic)
        {
            return User32.DdeConnect(_identifier, handleStringServer, handleStringTopic, IntPtr.Zero);
        }

        public IntPtr CreateDataHandle(IntPtr hitem, Bytes data, int format)
        {
            IntPtr output;

            if (KCodePage == User32.CP_WINANSI)
                output = User32.DdeCreateDataHandleAnsi(_identifier, data.Array, data.Length, 0, hitem, format, 0);
            else
                output = User32.DdeCreateDataHandleUnicode(_identifier, data.Array, data.Length, 0, hitem, format, 0);

            return output;
        }

        public IntPtr CreateStringHandle(string data)
        {
            IntPtr output;

            if (KCodePage == User32.CP_WINANSI)
                output = User32.DdeCreateStringHandleAnsi(_identifier, data, KCodePage);
            else
                output = User32.DdeCreateStringHandleUnicode(_identifier, data, KCodePage);

            return output;
        }

        public void Disconnect(IntPtr handleConnection)
        {
            User32.DdeDisconnect(handleConnection);
        }

        public void FreeDataHandle(IntPtr hdata)
        {
            User32.DdeFreeDataHandle(hdata);
        }

        public void FreeStringHandle(IntPtr handleString)
        {
            User32.DdeFreeStringHandle(_identifier, handleString);
        }

        public bool GetData(IntPtr hdata, out Bytes bytes)
        {
            int lg = User32.DdeGetData(hdata, IntPtr.Zero, 0, 0);

            bytes = new Bytes(lg);
            LockTable<byte> hbts = new LockTable<byte>(bytes.Array, bytes.Length);
            lg = User32.DdeGetData(hdata, hbts.Address(0), lg, 0); // TODOC# A tester... Réécrit car la traduction VB était louche (ref sur un octet)
            hbts.Free();

            return lg > 0;
        }

        public User32.DMLERR GetLastError()
        {
            return (User32.DMLERR)User32.DdeGetLastError(_identifier);
        }

        public bool Initialize(User32.FNCALLBACK callback)
        {
            int id = 0;
            bool output = false;

            int cmd = User32.APPCLASS_STANDARD | User32.APPCMD_CLIENTONLY;
            int erc = User32.DdeInitializeW(ref id, callback, cmd, 0);

            if (erc == 0)
            {
                _identifier = id;
                output = true;
            }

            return output;
        }

        public string QueryString(IntPtr handleString)
        {
            // -- la doc n'est pas claire du tout sur la signification des longueurs de chaines...
            string output = "";

            if (handleString != IntPtr.Zero)
            {
                if (KCodePage == User32.CP_WINANSI)
                {
                    int length = User32.DdeQueryStringAnsi(_identifier, handleString, IntPtr.Zero, 0, KCodePage);
                    StringBuilder sb = new StringBuilder(length);
                    _ = User32.DdeQueryStringAnsi(_identifier, handleString, sb, length, KCodePage);
                    output = sb.ToString();
                }
                else
                {
                    int length = User32.DdeQueryStringUnicode(_identifier, handleString, IntPtr.Zero, 0, KCodePage);
                    length++;
                    StringBuilder sb = new StringBuilder(length);
                    _ = User32.DdeQueryStringUnicode(_identifier, handleString, sb, length, KCodePage);
                    output = sb.ToString();
                }
            }

            return output;
        }

        public void Uninitialize()
        {
            User32.DdeUninitialize(_identifier);
            _identifier = -1;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }


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