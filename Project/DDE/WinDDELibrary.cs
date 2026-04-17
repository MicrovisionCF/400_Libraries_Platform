using System;
using System.Runtime.InteropServices;
using System.Text;

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
        // ***************************************************************************************************

        private static int KCodePage = CP_WINANSI; // -- pas encore décidé...
        private int _identifier;

        // -- ddeml.h 
        // -- Application command flags
        private const int APPCMD_CLIENTONLY = 0x10;
        private const int APPCMD_FILTERINITS = 0x20;
        private const int APPCMD_MASK = 0xFF0;

        // -- Application classification flags
        private const int APPCLASS_STANDARD = 0x0;
        private const int APPCLASS_MASK = 0xF;

        // /***** codepage constants ****/
        private const int CP_WINANSI = 1004;    // /* default codepage for windows & old DDE convs. */
        private const int CP_WINUNICODE = 1200;

        // /***** transaction types *****/
        private const int XTYPF_NOBLOCK = 0x2;  // /* CBR_BLOCK will not work */
        private const int XTYPF_NODATA = 0x4;  // /* DDE_FDEFERUPD */
        private const int XTYPF_ACKREQ = 0x8;  // /* DDE_FACKREQ */
        private const int XCLASS_MASK = 0xFC00;
        private const int XCLASS_BOOL = 0x1000;
        private const int XCLASS_DATA = 0x2000;
        private const int XCLASS_FLAGS = 0x4000;
        private const int XCLASS_NOTIFICATION = 0x8000;

        public enum XType
        {
            XTYP_ERROR = 0x0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK,
            XTYP_ADVDATA = 0x10 | XCLASS_FLAGS,
            XTYP_ADVREQ = 0x20 | XCLASS_DATA | XTYPF_NOBLOCK,
            XTYP_ADVSTART = 0x30 | XCLASS_BOOL,
            XTYP_ADVSTOP = 0x40 | XCLASS_NOTIFICATION,
            XTYP_EXECUTE = 0x50 | XCLASS_FLAGS,
            XTYP_CONNECT = 0x60 | XCLASS_BOOL | XTYPF_NOBLOCK,
            XTYP_CONNECT_CONFIRM = 0x70 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK,
            XTYP_XACT_COMPLETE = 0x80 | XCLASS_NOTIFICATION,
            XTYP_POKE = 0x90 | XCLASS_FLAGS,
            XTYP_REGISTER = 0xA0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK,
            XTYP_REQUEST = 0xB0 | XCLASS_DATA,
            XTYP_DISCONNECT = 0xC0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK,
            XTYP_UNREGISTER = 0xD0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK,
            XTYP_WILDCONNECT = 0xE0 | XCLASS_DATA | XTYPF_NOBLOCK,
            XTYP_MASK = 0xF0,
            XTYP_SHIFT = 0x4  // /* shift to turn XTYP_ into an index */
        }

        // -- /* DDE constants for wStatus field */

        public enum DDEStatus
        {
            DDE_FACK = 0x8000,
            DDE_FBUSY = 0x4000,
            DDE_FDEFERUPD = 0x4000,
            DDE_FACKREQ = 0x8000,
            DDE_FRELEASE = 0x2000,
            DDE_FREQUESTED = 0x1000,
            DDE_FAPPSTATUS = 0xFF,
            DDE_FNOTPROCESSED = 0x0,
            DDE_FACKRESERVED = ~(DDE_FACK | DDE_FBUSY | DDE_FAPPSTATUS),
            DDE_FADVRESERVED = ~(DDE_FACKREQ | DDE_FDEFERUPD),
            DDE_FDATRESERVED = ~(DDE_FACKREQ | DDE_FRELEASE | DDE_FREQUESTED),
            DDE_FPOKRESERVED = ~DDE_FRELEASE
        }

        public enum DMLERR
        {
            DMLERR_NO_ERRORField = 0x0,       // /* must be 0 */
            DMLERR_FIRSTField = 0x4000,
            DMLERR_ADVACKTIMEOUTField = 0x4000,
            DMLERR_BUSYField = 0x4001,
            DMLERR_DATAACKTIMEOUTField = 0x4002,
            DMLERR_DLL_NOT_INITIALIZEDField = 0x4003,
            DMLERR_DLL_USAGEField = 0x4004,
            DMLERR_EXECACKTIMEOUTField = 0x4005,
            DMLERR_INVALIDPARAMETERField = 0x4006,
            DMLERR_LOW_MEMORYField = 0x4007,
            DMLERR_MEMORY_ERRORField = 0x4008,
            DMLERR_NOTPROCESSEDField = 0x4009,
            DMLERR_NO_CONV_ESTABLISHEDField = 0x400A,
            DMLERR_POKEACKTIMEOUTField = 0x400B,
            DMLERR_POSTMSG_FAILEDField = 0x400C,
            DMLERR_REENTRANCYField = 0x400D,
            DMLERR_SERVER_DIEDField = 0x400E,
            DMLERR_SYS_ERRORField = 0x400F,
            DMLERR_UNADVACKTIMEOUTField = 0x4010,
            DMLERR_UNFOUND_QUEUE_IDField = 0x4011,
            DMLERR_LASTField = 0x4011
        }

        public delegate IntPtr FNCALLBACK(XType wType, int wFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DdeClientTransaction(IntPtr pDataAd, int cbData, IntPtr hConv, IntPtr hszItem, int wFmt, uint wType, int dwTimeout, ref int pdwResult);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DdeConnect(int idInst, IntPtr hszService, IntPtr hszTopic, IntPtr pCC);
        [DllImport("user32.dll", EntryPoint = "DdeCreateDataHandle", CharSet = CharSet.Ansi)] private static extern IntPtr DdeCreateDataHandleAnsi(int idInst, byte[] pSrc, int cb, int cbOff, IntPtr hszItem, int wFmt, int afCmd);
        [DllImport("user32.dll", EntryPoint = "DdeCreateDataHandle", CharSet = CharSet.Unicode)] private static extern IntPtr DdeCreateDataHandleUnicode(int idInst, byte[] pSrc, int cb, int cbOff, IntPtr hszItem, int wFmt, int afCmd);
        [DllImport("user32.dll", EntryPoint = "DdeCreateStringHandleW", CharSet = CharSet.Ansi)] private static extern IntPtr DdeCreateStringHandleAnsi(int idInst, string psz, int iCodePage);
        [DllImport("user32.dll", EntryPoint = "DdeCreateStringHandleW", CharSet = CharSet.Unicode)] private static extern IntPtr DdeCreateStringHandleUnicode(int idInst, string psz, int iCodePage);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeDisconnect(IntPtr hConv);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeFreeDataHandle(IntPtr hData);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeFreeStringHandle(int idInst, IntPtr hsz);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeGetData(IntPtr hData, IntPtr pDstad, int cbMax, int cbOff);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeGetLastError(int idInst);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeInitializeW(ref int pidInst, FNCALLBACK pfnCallback, int afCmd, int ulRes);
        [DllImport("user32.dll", EntryPoint = "DdeQueryStringW", CharSet = CharSet.Ansi)] private static extern int DdeQueryStringAnsi(int idInst, IntPtr hsz, IntPtr psz, int cchMax, int iCodePage);
        [DllImport("user32.dll", EntryPoint = "DdeQueryStringW", CharSet = CharSet.Ansi)] private static extern int DdeQueryStringAnsi(int idInst, IntPtr hsz, StringBuilder psz, int cchMax, int iCodePage);
        [DllImport("user32.dll", EntryPoint = "DdeQueryStringW", CharSet = CharSet.Unicode)] private static extern int DdeQueryStringUnicode(int idInst, IntPtr hsz, IntPtr psz, int cchMax, int iCodePage);
        [DllImport("user32.dll", EntryPoint = "DdeQueryStringW", CharSet = CharSet.Unicode)] private static extern int DdeQueryStringUnicode(int idInst, IntPtr hsz, StringBuilder psz, int cchMax, int iCodePage);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DdeUninitialize(int idInst);

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

        public IntPtr ClientTransaction(IntPtr hconv, IntPtr hitem, XType typ)
        {
            int pdwResult = 0;
            return DdeClientTransaction(IntPtr.Zero, 0, hconv, hitem, 1, (uint)typ, 1000, ref pdwResult);
        }

        public IntPtr ClientTransactionData(IntPtr dataHandle, IntPtr hconv, IntPtr hitem, XType typ)
        {
            int pdwResult = 0;
            return DdeClientTransaction(dataHandle, -1, hconv, hitem, 1, (uint)typ, 1000, ref pdwResult);
        }

        public IntPtr Connect(IntPtr hsrv, IntPtr htopic)
        {
            return DdeConnect(_identifier, hsrv, htopic, IntPtr.Zero);
        }

        public IntPtr CreateDataHandle(IntPtr hitem, Bytes data, int format)
        {
            IntPtr output;

            if (KCodePage == CP_WINANSI)
                output = DdeCreateDataHandleAnsi(_identifier, data.Array, data.Length, 0, hitem, format, 0);
            else
                output = DdeCreateDataHandleUnicode(_identifier, data.Array, data.Length, 0, hitem, format, 0);

            return output;
        }

        public IntPtr CreateStringHandle(string data)
        {
            IntPtr output;

            if (KCodePage == CP_WINANSI)
                output = WinDDELibrary.DdeCreateStringHandleAnsi(_identifier, data, KCodePage);
            else
                output = WinDDELibrary.DdeCreateStringHandleUnicode(_identifier, data, KCodePage);

            return output;
        }

        public void Disconnect(IntPtr hconv)
        {
            DdeDisconnect(hconv);
        }

        public void FreeDataHandle(IntPtr hdata)
        {
            DdeFreeDataHandle(hdata);
        }

        public void FreeStringHandle(IntPtr hsz)
        {
            DdeFreeStringHandle(_identifier, hsz);
        }

        public bool GetData(IntPtr hdata, out Bytes bf)
        {
            int lg = DdeGetData(hdata, IntPtr.Zero, 0, 0);

            bf = new Bytes(lg);
            LockTable<byte> hbts = new LockTable<byte>(bf.Array, bf.Length);
            lg = DdeGetData(hdata, hbts.Address(0), lg, 0); // TODOC# A tester... Réécrit car la traduction VB était louche (ref sur un octet)
            hbts.Free();

            return lg > 0;
        }

        public DMLERR GetLastError()
        {
            return (DMLERR)DdeGetLastError(_identifier);
        }

        public bool Initialize(FNCALLBACK cbck)
        {
            int id = 0;
            bool output = false;

            int cmd = APPCLASS_STANDARD | APPCMD_CLIENTONLY;
            int erc = DdeInitializeW(ref id, cbck, cmd, 0);

            if (erc == 0)
            {
                _identifier = id;
                output = true;
            }

            return output;
        }

        public string QueryString(IntPtr hsz)
        {
            // -- la doc n'est pas claire du tout sur la signification des longueurs de chaines...
            string output = "";

            if (hsz != IntPtr.Zero)
            {
                if (KCodePage == CP_WINANSI)
                {
                    int lg = DdeQueryStringAnsi(_identifier, hsz, IntPtr.Zero, 0, KCodePage);
                    StringBuilder sb = new StringBuilder(lg);
                    lg = WinDDELibrary.DdeQueryStringAnsi(_identifier, hsz, sb, lg, KCodePage);
                    output = sb.ToString();
                }
                else
                {
                    int lg = DdeQueryStringUnicode(_identifier, hsz, IntPtr.Zero, 0, KCodePage);
                    lg++;
                    StringBuilder sb = new StringBuilder(lg);
                    lg = WinDDELibrary.DdeQueryStringUnicode(_identifier, hsz, sb, lg, KCodePage);
                    output = sb.ToString();
                }
            }

            return output;
        }

        public void Uninitialize()
        {
            DdeUninitialize(_identifier);
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