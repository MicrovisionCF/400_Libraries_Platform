using System.Diagnostics;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.DDE
{
    public class WinDDEConversation : Citizen
    {
        // ***************************************************************************************************
        // 04.03.11 : (création) Une "conversation" DDE, définie par ServerName + Topic
        //            WinDDEManager
        //            + WinDDELibrary
        //            + WinDDEConversations
        //            + WinDDEConversation
        // 21.03.12 : libs 1.8, héritage Citizen.
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 19.06.16 : _items as list(of)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Ajout de Poke pour envoi de valeur au serveur
        // 12.04.21 : Ajout d'un caractère terminal nul à l'envoi de chaines
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void ItemDataChangeEventHandler(WinDDEConversation sender, string itemName, string value);
        public delegate void ItemLinkStartEventHandler(WinDDEConversation sender, string itemName);
        public delegate void ItemLinkStopEventHandler(WinDDEConversation sender, string itemName);

        public event ItemDataChangeEventHandler? ItemDataChange;
        public event ItemLinkStartEventHandler? ItemLinkStart;
        public event ItemLinkStopEventHandler? ItemLinkStop;

        // ***************************************************************************************************

        private struct xItem
        {
            public IntPtr hitem;
            public bool isLinkRequired;
            public bool isLinkEtablished;

            public xItem(IntPtr h)
            {
                hitem = h;
                isLinkRequired = false;
                isLinkEtablished = false;
            }

            public xItem SetLinkEtablished(bool etablished)
            {
                isLinkEtablished = etablished;
                return this;
            }

            public xItem SetLinkRequired(bool required)
            {
                isLinkRequired = required;
                return this;
            }
        }


        private readonly WinDDELibrary _lib;
        private readonly IntPtr _hServer;
        private readonly IntPtr _hTopic;

        private readonly List<xItem> _items;

        private bool _connectable;
        private IntPtr _hConv;



        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public WinDDEConversation(WinDDELibrary lib, string serverName, string topic) : base()
        {
            _lib = lib;

            _hServer = _lib.CreateStringHandle(serverName);
            _hTopic = _lib.CreateStringHandle(topic);

            _connectable = true;
            _items = [];
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public IntPtr HConv => _hConv;

        public int ItemsCount => _items.Count;

        public int RequestedLinksCount => zLinkReqsCount(_items);

        public string ServerName => _lib.QueryString(_hServer);

        public string Topic => _lib.QueryString(_hTopic);


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public int AddItem(string itemName)
        {
            IntPtr h = _lib.CreateStringHandle(itemName);
            _items.Add(new xItem(h));

            return _items.Count - 1;
        }

        public void AdviseItemData(int itemNo, IntPtr hdata)
        {
            _lib.GetData(hdata, out Bytes bf);
            oOnItemDataChange(_lib.QueryString(_items[itemNo].hitem), zCString(bf));
        }

        public bool Connect(bool fnotify)
        {
            // -- fnotify  = le serveur vient d'arriver... mais n'est pas forcément complètement prêt (Falcon).
            // Je pourrais exploiter la situation si je savais rétablir la connexion de façon asynchrone, 
            // mais la DDE n'aime pas les threads, et je ne sais pas faire d'invoke dans la même thread, donc dommage.
            // Donc je me contente de signaler qu'on peut retenter la connexion.


            if (_hConv == IntPtr.Zero)
            {
                if (fnotify)
                {
                    _connectable = true;
                }
                else if (_connectable)
                {
                    _hConv = _lib.Connect(_hServer, _hTopic);
                    if (_hConv != IntPtr.Zero)
                    {
                        for (int i = 0; i < _items.Count; i++)
                        {
                            if (_items[i].isLinkRequired && !_items[i].isLinkEtablished)      // -- LinkItem a déjà été appelé, sans succés, donc on réessaie.
                            {
                                IntPtr h = _lib.ClientTransaction(_hConv, _items[i].hitem, User32.XType.XTYP_ADVSTART);
                                if (h != IntPtr.Zero)
                                {
                                    _items[i] = _items[i].SetLinkEtablished(true);
                                    oOnItemLinkStart(_lib.QueryString(_items[i].hitem));
                                    // -- 13.09.13 : quand faut-il libérer h ?
                                }
                            }
                        }
                    }

                    _connectable = false;
                }
            }

            return _hConv != IntPtr.Zero;
        }

        public bool Connected()
        {
            return _hConv != IntPtr.Zero;
        }

        public void Disconnect(bool fnotify)
        {
            // -- fnotify = Disconnect issu du server, donc déjà fait


            if (_hConv != IntPtr.Zero)
            {
                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    if (_items[i].isLinkEtablished)
                    {
                        if (!fnotify) _lib.ClientTransaction(_hConv, _items[i].hitem, User32.XType.XTYP_ADVSTOP);
                        _items[i] = _items[i].SetLinkEtablished(false);
                        oOnItemLinkStop(_lib.QueryString(_items[i].hitem));
                        // -- 13.09.13 : faut-il libérer h ?
                    }
                }

                if (!fnotify)
                {
                    _lib.Disconnect(_hConv);
                    _connectable = true;
                }

                _hConv = IntPtr.Zero;
            }
        }

        public int FindItem(IntPtr hitem)
        {
            return zFindHItem(hitem, _items);
        }

        public int FindItem(string itemName)
        {
            return zFindName(itemName, _lib, _items);
        }

        public bool LinkItem(int itemNo)
        {
            _items[itemNo] = _items[itemNo].SetLinkRequired(true);
            if (_hConv != IntPtr.Zero)
            {
                IntPtr h = _lib.ClientTransaction(_hConv, _items[itemNo].hitem, User32.XType.XTYP_ADVSTART);
                _items[itemNo] = _items[itemNo].SetLinkEtablished(h != IntPtr.Zero);
                // -- 13.09.13 : quand faut-il libérer h ?

                User32.DMLERR erc = _lib.GetLastError();  // -- des fois ya des erreurs mais ça marche quand même...
                if (erc != 0)
                    Debug.Print(_lib.QueryString(_items[itemNo].hitem) + SpecialChars.Tab + h.ToString() + SpecialChars.Tab + erc.ToNameString());
            }

            return _items[itemNo].isLinkEtablished;
        }

        public bool PokeItemData(int itemNo, string data)
        {
            bool ok = false;
            data += SpecialChars.Null;

            Bytes bf = new Bytes(System.Text.Encoding.ASCII.GetBytes(data));
            IntPtr dataHandle = _lib.CreateDataHandle(_items[itemNo].hitem, bf, 1);
            if (dataHandle != IntPtr.Zero)
            {
                IntPtr h = _lib.ClientTransactionData(dataHandle, _hConv, _items[itemNo].hitem, User32.XType.XTYP_POKE);
                ok = h != IntPtr.Zero;
            }

            return ok;
        }

        public void RemoveItem(int itemNo)
        {
            _lib.FreeStringHandle(_items[itemNo].hitem);
            _items.RemoveAt(itemNo);
        }

        public string RequestItemData(int itemNo)
        {
            string s = "";

            if (_hConv != IntPtr.Zero)
            {
                IntPtr h = _lib.ClientTransaction(_hConv, _items[itemNo].hitem, User32.XType.XTYP_REQUEST);
                if (h != IntPtr.Zero)
                {
                    _lib.GetData(h, out Bytes bts);
                    s = zCString(bts);
                    _lib.FreeDataHandle(h);
                }
            }

            return s;
        }

        public void UnlinkItem(int itemNo)
        {
            if (_hConv != IntPtr.Zero)
            {
                _lib.ClientTransaction(_hConv, _items[itemNo].hitem, User32.XType.XTYP_ADVSTOP); // -- 13.09.13 : faut-il libérer h ?
                _items[itemNo] = _items[itemNo].SetLinkEtablished(false);

                User32.DMLERR erc = _lib.GetLastError();
                if (erc != 0)
                    Debug.Print(_lib.QueryString(_items[itemNo].hitem) + SpecialChars.Tab + erc.ToNameString());
            }

            _items[itemNo] = _items[itemNo].SetLinkRequired(false);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _items.ForEach(o => _lib.FreeStringHandle(o.hitem));
            _items.Clear();
            _items.TrimExcess();

            _lib.FreeStringHandle(_hServer);
            _lib.FreeStringHandle(_hTopic);

            base.oDispose(isExplicit);
        }

        protected virtual void oOnItemDataChange(string itemName, string value)
        {
            ItemDataChange?.Invoke(this, itemName, value);
        }

        protected virtual void oOnItemLinkStart(string itemName)
        {
            ItemLinkStart?.Invoke(this, itemName);
        }

        protected virtual void oOnItemLinkStop(string itemName)
        {
            ItemLinkStop?.Invoke(this, itemName);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zCString(Bytes bf)
        {
            int i = 0;
            string s = "";

            while (bf[i] != 0)
            {
                s += Convert.ToChar(bf[i]);
                i++;
            }

            return s;
        }

        private static int zFindHItem(IntPtr hitem, List<xItem> lst)
        {
            return lst.FindIndex(o => hitem == o.hitem);
        }

        private static int zFindName(string itemName, WinDDELibrary lb, List<xItem> lst)
        {
            return lst.FindIndex(o => itemName == lb.QueryString(o.hitem));
        }

        private static int zLinkReqsCount(List<xItem> lst)
        {
            return lst.Count(o => o.isLinkRequired);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}