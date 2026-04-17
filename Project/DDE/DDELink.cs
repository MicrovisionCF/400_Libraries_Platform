using Microvision.Types;

namespace Microvision.DDE
{
    public class DDELink : Citizen
    {
        // ***************************************************************************************************
        // 03.03.11 : (création) un lien DDE.
        // 07.03.11 : traduction VBNet (à cause la dde est interdite dans une librarie ActiveX), via
        //            implémentation VBNet de la DDE via DDEManager créé pour l'occasion.
        // 20.03.12 : libs 1.8, héritage Citizen, événement LinkChange.
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 18.03.21 : Ajout de SetValue, pour écriture de valeur vers le serveur
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate bool DecodeHandler(string caption, out float value);
        public delegate bool EncodeHandler(float value, out string caption);
        public delegate void LinkChangeEventHandler(DDELink sender, bool isLinkEtablished);
        public delegate void ValueChangeEventHandler(DDELink sender, bool isValid, float value);

        public event LinkChangeEventHandler LinkChange;
        public event ValueChangeEventHandler ValueChange;

        // ***************************************************************************************************

        private WinDDEManager.xDDEItem _item;
        private string _longName;
        private string _shortName;
        private string _unit;
        private DecodeHandler _decoder;
        private EncodeHandler _encoder;

        private bool _valid;
        private float _value;

        private WinDDEManager _ddeManager;  // -- connexion demandée
        private bool _linked;           // -- connexion établie


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DDELink() : base()
        {
            _linked = false;
            _valid = false;
            _decoder = zGetValue;
            _encoder = zGetString;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool IsConnected => _ddeManager is not null;

        public bool IsLinked => _linked;

        public string LinkItem => _item.itemName;

        public string LinkServer => _item.serverName;

        public string LinkTopic => _item.topic;

        public string LongName => _longName;

        public string ShortName => _shortName;

        public string Unit => _unit;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        [Obsolete("17.10.23 Utiliser la version avec retour nullable")]
        public bool GetValue(ref float value)
        {
            if (!_linked)
            {
                // -- serveur arrivé après le client, ceci va connecter la conversation (Connect) et établir les liens déjà demandés
                // si la source a balancé un événement XTYP_REGISTER (ce qui est le cas de Falcon, mais pas d'Excel, par exemple).
                // -- MAIS : ceci ne fonctionne que si GetValue est appelé de la thread qui a initialisé la conversation.
                _valid = _decoder.Invoke(_ddeManager.RequestItemData(_item), out _value);
            }

            if (_valid) value = _value;

            return _valid;
        }

        public float? GetValue()
        {
            if (!_linked)
            {
                // -- serveur arrivé après le client, ceci va connecter la conversation (Connect) et établir les liens déjà demandés
                // si la source a balancé un événement XTYP_REGISTER (ce qui est le cas de Falcon, mais pas d'Excel, par exemple).
                // -- MAIS : ceci ne fonctionne que si GetValue est appelé de la thread qui a initialisé la conversation.
                _valid = _decoder.Invoke(_ddeManager.RequestItemData(_item), out _value);
            }

            return _valid ? _value : null;
        }

        public void SetDecoder(DecodeHandler function)
        {
            _decoder = function;
        }

        public void SetEncoder(EncodeHandler function)
        {
            _encoder = function;
        }

        public void SetLink(string serverName, string topic, string itemName)
        {
            _item = new WinDDEManager.xDDEItem(serverName, topic, itemName);
        }

        public void SetNames(string longName, string shortName, string unit)
        {
            _longName = longName;
            _shortName = shortName;
            _unit = unit;
        }

        public bool SetValue(float value)
        {
            bool ok = false;

            if (_encoder.Invoke(value, out string caption))
            {
                ok = _ddeManager.PokeItemData(_item, caption);
                _value = value;
            }

            return ok;
        }

        public bool StartConnection(WinDDEManager manager)
        {
            _ddeManager = manager;
            _ddeManager_Attach(true);
            _ddeManager.RegisterItem(_item.serverName, _item.topic, _item.itemName);

            if (_ddeManager.LinkItem(_item))
            {
                _linked = true;
                _valid = _decoder.Invoke(_ddeManager.RequestItemData(_item), out _value);
            }

            return (_ddeManager is not null);
        }

        public void StopConnection()
        {
            _valid = false;

            if (_linked)
            {
                _ddeManager.UnlinkItem(_item);
                _linked = false;
            }

            _ddeManager.UnregisterItem(ref _item);
            _ddeManager_Attach(false);
            _ddeManager = null;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected virtual void oOnLinkChange(bool isLinkEtablished)
        {
            LinkChange?.Invoke(this, isLinkEtablished);
        }

        protected virtual void oOnValueChange(bool isValid, float value)
        {
            ValueChange?.Invoke(this, isValid, value);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private bool zGetString(float value, out string caption)
        {
            caption = value.ToString();

            return true;
        }

        private bool zGetValue(string caption, out float value)
        {
            bool ok;

            if (caption != "")
            {
                value = ConvertShop.ReadFloat(caption);
                ok = true;
            }
            else
            {
                value = -1;
                ok = false;
            }

            return ok;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _ddeManager_Attach(bool attach)
        {
            if (attach)
            {
                _ddeManager.ItemDataChange += _ddeManager_ItemDataChange;
                _ddeManager.ItemLinkStart += _ddeManager_ItemLinkStart;
                _ddeManager.ItemLinkStop += _dDEMng_ItemLinkStop;
            }
            else
            {
                _ddeManager.ItemDataChange -= _ddeManager_ItemDataChange;
                _ddeManager.ItemLinkStart -= _ddeManager_ItemLinkStart;
                _ddeManager.ItemLinkStop -= _dDEMng_ItemLinkStop;
            }
        }

        private void _ddeManager_ItemDataChange(WinDDEManager.xDDEItem item, string caption)
        {
            if (item == _item)
            {
                bool changed = false;
                bool ok = _decoder.Invoke(caption, out float newValue);

                if (ok != _valid)
                {
                    _valid = ok;
                    changed = true;
                }

                if (_valid && newValue != _value)
                {
                    _value = newValue;
                    changed = true;
                }

                if (changed) oOnValueChange(_valid, _value);
            }
        }

        private void _ddeManager_ItemLinkStart(WinDDEManager.xDDEItem item)
        {
            if (item == _item && !_linked)
            {
                _linked = true;
                _valid = _decoder.Invoke(_ddeManager.RequestItemData(_item), out _value);
                oOnLinkChange(_linked);
                oOnValueChange(_valid, _value);
            }
        }

        private void _dDEMng_ItemLinkStop(WinDDEManager.xDDEItem item)
        {
            if (item == _item && _linked)
            {
                _linked = false;
                if (_valid)
                {
                    _valid = false;
                    oOnValueChange(_valid, _value);
                }

                oOnLinkChange(_linked);
            }
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}