using System;

using Microvision.Types;

namespace Microvision.DDE
{
    public class DDEData : Citizen
    {
        // ***************************************************************************************************
        // 03.03.11 : (création) lecture de plusieurs canaux DDE et transfo en propriétés ActiveX (pour VBNet,
        //            au final).
        // 07.03.11 : traduction VBNet, à cause DDE interdite dans les librairies ActiveX, via WinDDEManager
        //            créé pour l'occasion.
        // 20.03.12 : traduction libs 1.8, mais :
        //            - objet privatisé
        //            - suppression de l'implémentation de iAnalogPlugin
        //            - suppression de DDEConfig
        //            - ajout des événements LinkChange et ValueChange.
        //            - scission en DDEData et DDELinks
        //            Et mise en base (la librairie DataLinks ne sera pas reconduite telle qu'elle).
        // 13.09.13 : libs 2.0, intégration à µV.Platform.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        //            DDEData
        //            + WinDDEManager
        //            + WinDDELibrary
        //            + WinDDEConversations
        //            + WinDDEConversation
        //            + DDELinks
        //            + DDELink
        // 18.03.21 : Ajout de SetValue, pour écrire sur le serveur
        // 13.04.22 : (libs 3.0)
        // 04.10.23 : Ajout FindShortName
        // ***************************************************************************************************

        public delegate void LinkChangeEventHandler(int linkNo, bool flnk);
        public delegate void ValueChangeEventHandler(int linkNo, bool isValid, float value);

        public event LinkChangeEventHandler LinkChange;
        public event ValueChangeEventHandler ValueChange;

        // ***************************************************************************************************

        private WinDDEManager _dde;
        private DDELinks _links;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public DDEData() : base()
        {
            _links = new DDELinks();
            _links_Attach(true);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int Count => _links.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public int AddLink(string serverName, string topic, string itemName, string longName, string shortName, string unit)
        {
            DDELink link = new DDELink();
            link.SetLink(serverName, topic, itemName);
            link.SetNames(longName, shortName, unit);

            int no = _links.Add(link.GiveLife());

            return no;
        }

        public bool Connect(int no)
        {
            return _links.StartConnection(no, _dde);
        }

        public void Disconnect(int no)
        {
            _links.StopConnection(no);
        }

        public int FintShortName(string name)
        {
            return _links.FindShortName(name);
        }

        public string GetLongName(int no)
        {
            return _links.GetLongName(no);
        }

        public string GetShortName(int no)
        {
            return _links.GetShortName(no);
        }

        public string GetUnit(int no)
        {
            return _links.GetUnit(no);
        }

        [Obsolete("17.10.23 Utiliser la version avec retour nullable")]
        public bool GetValue(int no, ref float v)
        {
            float? value = this.GetValue(no);

            if (value.HasValue) v = value.Value;

            return value.HasValue;
        }

        public float? GetValue(int no)
        {
            return _links.GetValue(no);
        }

        public bool IsConnected(int no)
        {
            return _links.IsConnected(no); // -- liaison demandée
        }

        public bool IsLinked(int no)
        {
            return _links.IsLinked(no); // -- liaison établie
        }

        public bool IsStarted()
        {
            return _dde is not null;
        }

        public void SetDecoder(int no, DDELink.DecodeHandler fct)
        {
            _links.SetDecoder(no, fct);
        }

        public void SetEncoder(int no, DDELink.EncodeHandler fct)
        {
            _links.SetEncoder(no, fct);
        }

        public bool SetValue(int no, float v)
        {
            return _links.SetValue(no, v);
        }

        public bool StartManager()
        {
            WinDDEManager wmng = new WinDDEManager();

            if (wmng.Initialize())
                _dde = wmng;
            else
                wmng.Dispose();

            return (_dde is not null);
        }

        public void StopManager()
        {
            for (int i = 0; i < _links.Count; i++)
                if (_links.IsConnected(i))
                    _links.StopConnection(i);

            _dde.Terminate();
            _dde.Dispose();
            _dde = null;

            _links.Clear();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_links is not null)
            {
                _links_Attach(false);
                if (isExplicit) _links.Dispose();
                _links = null;
            }

            if (_dde is not null)
            {
                if (isExplicit) _dde.Dispose();
                _dde = null;
            }

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


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _links_Attach(bool attach)
        {
            if (attach)
            {
                _links.LinkChange += _links_LinkChange;
                _links.ValueChange += _links_ValueChange;
            }
            else
            {
                _links.LinkChange -= _links_LinkChange;
                _links.ValueChange -= _links_ValueChange;
            }
        }

        private void _links_LinkChange(int no, bool isLinkEtablishes)
        {
            oOnLinkChange(no, isLinkEtablishes);
        }

        private void _links_ValueChange(int no, bool isValid, float value)
        {
            oOnValueChange(no, isValid, value);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}