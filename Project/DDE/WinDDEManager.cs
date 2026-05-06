using System.Diagnostics;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.DDE
{
    public class WinDDEManager : Citizen
    {
        // ***************************************************************************************************
        // 04.03.11 : (création) une "instance" de ddeml, avec liste de conversations.
        //            WinDDEManager
        //            + WinDDELibrary
        //            + WinDDEConversations
        //            + WinDDEConversation
        // 21.03.12 : libs 1.8, héritage Citizen.
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Ajout de Poke pour envoi de valeur au serveur
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void ItemDataChangeEventHandler(xDDEItem item, string data);
        public delegate void ItemLinkStartEventHandler(xDDEItem item);
        public delegate void ItemLinkStopEventHandler(xDDEItem item);

        public event ItemDataChangeEventHandler? ItemDataChange;
        public event ItemLinkStartEventHandler? ItemLinkStart;
        public event ItemLinkStopEventHandler? ItemLinkStop;

        // ***************************************************************************************************

        public record struct xDDEItem
        {
            public string serverName;
            public string topic;
            public string itemName;

            public xDDEItem(string serverName, string topic, string itemName)
            {
                this.serverName = serverName;
                this.topic = topic;
                this.itemName = itemName;
            }
        }

        private readonly WinDDEConversations _convs;

        private WinDDELibrary? _lib;
        private User32.FNCALLBACK? _callBack;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public WinDDEManager() : base()
        {
            _convs = new WinDDEConversations();
            _convs_Attach(true);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool Initialize()
        {
            _lib = new WinDDELibrary();
            _callBack = zDDECallback;

            if (!_lib.Initialize(_callBack))
            {
                _lib.Dispose();
                _lib = null;
                _callBack = null;
            }

            return _lib is not null;
        }

        public bool LinkItem(xDDEItem item)
        {
            int cno = _convs.Find(item.serverName, item.topic);
            int ino = _convs.FindItem(cno, item.itemName);

            if (!_convs.Connected(cno)) _convs.Connect(cno, false);

            return _convs.LinkItem(cno, ino);
        }

        public bool PokeItemData(xDDEItem item, string data)
        {
            int cno = _convs.Find(item.serverName, item.topic);
            int ino = _convs.FindItem(cno, item.itemName);

            if (!_convs.Connected(cno)) _convs.Connect(cno, false);

            return _convs.PokeItemData(cno, ino, data);
        }

        public xDDEItem RegisterItem(string serverName, string topic, string itemName)
        {
            ArgumentNullException.Check(_lib);

            xDDEItem item = new xDDEItem(serverName, topic, itemName);

            int cno = _convs.Find(item.serverName, item.topic);
            if (cno < 0) cno = _convs.Add(new WinDDEConversation(_lib, item.serverName, item.topic).GiveLife());

            _convs.AddItem(cno, item.itemName);

            return item;
        }

        public string RequestItemData(xDDEItem item)
        {
            int cno = _convs.Find(item.serverName, item.topic);
            int ino = _convs.FindItem(cno, item.itemName);
            if (!_convs.Connected(cno)) _convs.Connect(cno, false);

            return _convs.RequestItemData(cno, ino);
        }

        public void Terminate()
        {
            ArgumentNullException.Check(_lib);

            _lib.Uninitialize();
            _lib.Dispose();
            _lib = null;
            _callBack = null;
        }

        public void UnlinkItem(xDDEItem item)
        {
            int cno = _convs.Find(item.serverName, item.topic);
            int ino = _convs.FindItem(cno, item.itemName);

            _convs.UnlinkItem(cno, ino);

            if (_convs.Connected(cno) && _convs.GetRequestedLinksCount(cno) == 0) _convs.Disconnect(cno, false);
        }

        public void UnregisterItem(ref xDDEItem item)
        {
            int cno = _convs.Find(item.serverName, item.topic);
            int ino = _convs.FindItem(cno, item.itemName);

            _convs.RemoveItem(cno, ino);

            if (_convs.GetItemsCount(cno) == 0) _convs.Remove(cno);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected bool oDDEAdviseData(IntPtr hconv, IntPtr hItem, IntPtr hData)
        {
            bool ok = false;
            int cno = _convs.Find(hconv);
            if (cno >= 0)
            {
                int ino = _convs.FindItem(cno, hItem);
                if (ino >= 0)
                {
                    _convs.AdviseItemData(cno, ino, hData);
                    ok = true;
                }
            }

            return ok;
        }

        protected virtual void oDDEASyncCompleted()
        {
            Debug.Print("==> " + "XTYP_XACT_COMPLETE");
        }

        protected void oDDEDisconnect(IntPtr hconv, int fsameinst)
        {
            int cno = _convs.Find(hconv);
            if (cno >= 0)
            {
                Debug.Print("==> " + "XTYP_DISCONNECT" + SpecialChars.Tab + _convs.GetConversation(cno).ServerName + ", " + _convs.GetConversation(cno).Topic + ", " + fsameinst);
                _convs.Disconnect(cno, true);
            }
        }

        protected void oDDEError(int hconv)
        {
            Debug.Print("==> " + "XTYP_ERROR");
        }

        protected void oDDERegisterServer(IntPtr hsrvnam)
        {
            ArgumentNullException.Check(_lib);

            string srvnam = _lib.QueryString(hsrvnam);
            Debug.Print("==> " + "XTYP_REGISTER" + SpecialChars.Tab + srvnam);

            for (int i = 0; i < _convs.Count; i++)
            {
                if (_convs.GetServerName(i) == srvnam && !_convs.Connected(i))
                {
                    _convs.Connect(i, true);
                }
            }
        }

        protected void oDDEUnregisterServer(IntPtr hsrvnam)
        {
            ArgumentNullException.Check(_lib);

            Debug.Print("==> " + "XTYP_UNREGISTER" + SpecialChars.Tab + _lib.QueryString(hsrvnam));
        }

        protected override void oDispose(bool isExplicit)
        {
            _convs_Attach(false);
            if (isExplicit) _convs.Dispose();

            base.oDispose(isExplicit);
        }

        protected virtual void oOnItemDataChange(xDDEItem item, string data)
        {
            ItemDataChange?.Invoke(item, data);
        }

        protected virtual void oOnItemLinkStart(xDDEItem item)
        {
            ItemLinkStart?.Invoke(item);
        }

        protected virtual void oOnItemLinkStop(xDDEItem item)
        {
            ItemLinkStop?.Invoke(item);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private IntPtr zDDECallback(User32.XType wType, int wFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
        {
            User32.DDEStatus output = default;

            switch (wType)       // -- ceux susceptibles d'être reçus par un client
            {
                case User32.XType.XTYP_ADVDATA:
                    bool adviseOk = oDDEAdviseData(hConv, hsz2, hData);
                    output = adviseOk ? User32.DDEStatus.DDE_FACK : User32.DDEStatus.DDE_FNOTPROCESSED;
                    break;

                case User32.XType.XTYP_DISCONNECT:// -- pas de réponse
                    oDDEDisconnect(hConv, dwData2.ToInt32());
                    break;

                case User32.XType.XTYP_ERROR:// -- pas de réponse
                    oDDEError((int)hConv);
                    break;

                case User32.XType.XTYP_REGISTER:// -- pas de réponse
                    oDDERegisterServer(hsz1);
                    break;

                case User32.XType.XTYP_UNREGISTER:// -- pas de réponse
                    oDDEUnregisterServer(hsz1);
                    break;

                case User32.XType.XTYP_XACT_COMPLETE:// -- pas de réponse
                    oDDEASyncCompleted();

                    Debug.Print("==> " + "XTYP_XACT_COMPLETE");
                    break;

                default:
                    Debug.Print("==> " + "inattendu");
                    break;
            }

            return (IntPtr)output;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _convs_Attach(bool attach)
        {
            if (attach)
            {
                _convs.ItemDataChange += _convs_ItemDataChange;
                _convs.ItemLinkStart += _convs_ItemLinkStart;
                _convs.ItemLinkStop += _convs_ItemLinkStop;
            }
            else
            {
                _convs.ItemDataChange -= _convs_ItemDataChange;
                _convs.ItemLinkStart -= _convs_ItemLinkStart;
                _convs.ItemLinkStop -= _convs_ItemLinkStop;
            }
        }

        private void _convs_ItemDataChange(WinDDEConversation conv, string itemName, string data)
        {
            oOnItemDataChange(new xDDEItem(conv.ServerName, conv.Topic, itemName), data);
        }

        private void _convs_ItemLinkStart(WinDDEConversation conv, string itemName)
        {
            oOnItemLinkStart(new xDDEItem(conv.ServerName, conv.Topic, itemName));
        }

        private void _convs_ItemLinkStop(WinDDEConversation conv, string itemName)
        {
            oOnItemLinkStop(new xDDEItem(conv.ServerName, conv.Topic, itemName));
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}