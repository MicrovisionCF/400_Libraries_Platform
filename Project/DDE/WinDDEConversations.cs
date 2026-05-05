using Microvision.Collections;

namespace Microvision.DDE
{
    internal class WinDDEConversations : BaseList<WinDDEConversation>
    {
        // ***************************************************************************************************
        // 04.03.11 : création
        //            WinDDEManager
        //            + WinDDELibrary
        //            + WinDDEConversations
        //            + WinDDEConversation
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 19.09.16 : héritage BaseList
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Ajout de Poke pour envoi de valeur au serveur
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void ItemDataChangeEventHandler(WinDDEConversation conv, string itemName, string value);
        public delegate void ItemLinkStartEventHandler(WinDDEConversation conv, string itemName);
        public delegate void ItemLinkStopEventHandler(WinDDEConversation conv, string itemName);

        public event ItemDataChangeEventHandler? ItemDataChange;
        public event ItemLinkStartEventHandler? ItemLinkStart;
        public event ItemLinkStopEventHandler? ItemLinkStop;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public int AddItem(int convNo, string itemName)
        {
            return _items[convNo].AddItem(itemName);
        }

        public void AdviseItemData(int convNo, int itemNo, IntPtr hdata)
        {
            _items[convNo].AdviseItemData(itemNo, hdata);
        }

        public bool Connect(int convNo, bool fnotify)
        {
            return _items[convNo].Connect(fnotify);
        }

        public bool Connected(int convNo)
        {
            return _items[convNo].Connected();
        }

        public void Disconnect(int convNo, bool fnotify)
        {
            _items[convNo].Disconnect(fnotify);
        }

        public int Find(IntPtr hconv)
        {
            return zFindHandle(hconv, _items);
        }

        public int Find(string serverName, string topic)
        {
            return zFindName(serverName, topic, _items);
        }

        public int FindItem(int convNo, IntPtr hitem)
        {
            return _items[convNo].FindItem(hitem);
        }

        public int FindItem(int convNo, string itemName)
        {
            return _items[convNo].FindItem(itemName);
        }

        public WinDDEConversation GetConversation(int convNo)
        {
            return _items[convNo];
        }

        public int GetItemsCount(int convNo)
        {
            return _items[convNo].ItemsCount;
        }

        public int GetRequestedLinksCount(int convNo)
        {
            return _items[convNo].RequestedLinksCount;
        }

        public string GetServerName(int convNo)
        {
            return _items[convNo].ServerName;
        }

        public bool LinkItem(int convNo, int itemNo)
        {
            return _items[convNo].LinkItem(itemNo);
        }

        public bool PokeItemData(int convNo, int itemNo, string data)
        {
            return _items[convNo].PokeItemData(itemNo, data);
        }

        public void RemoveItem(int convNo, int itemNo)
        {
            _items[convNo].RemoveItem(itemNo);
        }

        public string RequestItemData(int convNo, int itemNo)
        {
            return _items[convNo].RequestItemData(itemNo);
        }

        public void UnlinkItem(int convNo, int itemNo)
        {
            _items[convNo].UnlinkItem(itemNo);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected void oOnItemDataChange(WinDDEConversation sender, string itemName, string value)
        {
            ItemDataChange?.Invoke(sender, itemName, value);
        }

        protected void oOnItemLinkStart(WinDDEConversation sender, string itemName)
        {
            ItemLinkStart?.Invoke(sender, itemName);
        }

        protected void oOnItemLinkStop(WinDDEConversation sender, string itemName)
        {
            ItemLinkStop?.Invoke(sender, itemName);
        }

        protected override void oSetHandlers(WinDDEConversation obj, bool status)
        {
            if (status)
            {
                obj.ItemDataChange += oOnItemDataChange;
                obj.ItemLinkStart += oOnItemLinkStart;
                obj.ItemLinkStop += oOnItemLinkStop;
            }
            else
            {
                obj.ItemLinkStop -= oOnItemLinkStop;
                obj.ItemLinkStart -= oOnItemLinkStart;
                obj.ItemDataChange -= oOnItemDataChange;
            }

            base.oSetHandlers(obj, status);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static int zFindHandle(IntPtr hconv, IReadOnlyList<WinDDEConversation> lst)
        {
            return lst.FindIndex(o => hconv == o.HConv);
        }

        private static int zFindName(string srvnam, string topic, IReadOnlyList<WinDDEConversation> lst)
        {
            return lst.FindIndex(o => srvnam == o.ServerName && topic == o.Topic);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}