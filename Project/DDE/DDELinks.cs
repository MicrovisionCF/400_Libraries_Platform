using Microvision.Types;

namespace Microvision.DDE
{
    internal class DDELinks : BaseList<DDELink>
    {
        // ***************************************************************************************************
        // 21.03.12 : création, par scission de DDEData.
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 19.09.16 : héritage BaseList
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Ajout de SetEncoder + SetValue pour écriture de valeur sur le serveur
        // 13.04.22 : (libs 3.0)
        // 04.10.23 : Ajout FindShortName
        // ***************************************************************************************************

        public delegate void LinkChangeEventHandler(int no, bool isLinkEtablished);
        public delegate void ValueChangeEventHandler(int no, bool isValid, float value);

        public event LinkChangeEventHandler LinkChange;
        public event ValueChangeEventHandler ValueChange;

        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DDELinks() : base()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        internal int FindShortName(string name)
        {
            return _items.FindIndex(o => o.ShortName == name);
        }

        internal string GetLongName(int no)
        {
            return _items[no].LongName;
        }

        internal string GetShortName(int no)
        {
            return _items[no].ShortName;
        }

        internal string GetUnit(int no)
        {
            return _items[no].Unit;
        }

        internal float? GetValue(int no)
        {
            return _items[no].GetValue();
        }

        internal bool IsConnected(int no)
        {
            return _items[no].IsConnected;      // -- liaison demandée
        }

        internal bool IsLinked(int no)
        {
            return _items[no].IsLinked;         // -- liaison établie
        }

        internal void SetDecoder(int no, DDELink.DecodeHandler function)
        {
            _items[no].SetDecoder(function);
        }

        internal void SetEncoder(int no, DDELink.EncodeHandler function)
        {
            _items[no].SetEncoder(function);
        }

        internal bool SetValue(int no, float v)
        {
            return _items[no].SetValue(v);
        }

        internal bool StartConnection(int no, WinDDEManager manager)
        {
            return _items[no].StartConnection(manager);
        }

        internal void StopConnection(int no)
        {
            _items[no].StopConnection();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected virtual void oOnLinkChange(int no, bool isLinkEtablished)
        {
            LinkChange?.Invoke(no, isLinkEtablished);
        }

        protected virtual void oOnValueChange(int no, bool isValid, float value)
        {
            ValueChange?.Invoke(no, isValid, value);
        }

        protected override void oSetHandlers(DDELink link, bool status)
        {
            if (status)
            {
                link.LinkChange += _item_LinkChange;
                link.ValueChange += _item_ValueChange;
            }
            else
            {
                link.LinkChange -= _item_LinkChange;
                link.ValueChange -= _item_ValueChange;
            }

            base.oSetHandlers(link, status);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _item_LinkChange(DDELink sender, bool isLinkEtablished)
        {
            oOnLinkChange(_items.IndexOf(sender), isLinkEtablished);
        }

        private void _item_ValueChange(DDELink sender, bool isValid, float value)
        {
            oOnValueChange(_items.IndexOf(sender), isValid, value);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}