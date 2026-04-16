using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.Principal;
using System.Threading;

using Microvision.Types;

namespace Microvision.Platform
{
    public class IPCClient<T> : Citizen
    {
        // ***************************************************************************************************
        // 28.09.20 : Création
        // 27.01.22 : (libs 3.0)
        // 09.12.22 : Mutexage des démarrages / arrêts, parce qu'on a déjà eu des soucis avec la syncronicité
        //            de plusieurs démarrages simultanés (Vers Maestro et Integrity)
        //            Ajout des propriétés pour changer les noms manuellement.
        //            La récupération de l'objet distant se fait au premier appel de GetObject et plus à la connexion
        // ***************************************************************************************************

        private string _portName, _channelName, _objectUri;

        private IpcChannel _channel;
        private T _remoteObject;

        private Mutex _lockIpc;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public IPCClient(string name)
        {
            _portName = name;
            _channelName = _portName + "Channel";
            _objectUri = _portName + "Data";
            _lockIpc = new Mutex(false, "Microvision.IpcCreation");
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string ChannelName
        {
            get => _channelName;

            set
            {
                if (_channel is not null) throw new Exception("Le nom doit être définit avant d'ouvrir le canal.");
                _channelName = value;
            }
        }

        public string ObjectUri
        {
            get => _objectUri;

            set
            {
                if (_channel is not null) throw new Exception("Le nom doit être définit avant d'ouvrir le canal.");
                _objectUri = value;
            }
        }

        public bool Opened => _channel is not null;

        public string PortName
        {
            get => _portName;

            set
            {
                if (_channel is not null) throw new Exception("Le nom doit être définit avant d'ouvrir le canal.");
                _portName = value;
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Close()
        {
            if (_channel is not null)
            {
                _lockIpc.WaitOne();
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
                _remoteObject = default;
                _lockIpc.ReleaseMutex();
            }
        }

        public T GetObject()
        {
            _remoteObject ??= zGetRemoteObject(_channelName, _objectUri);

            return _remoteObject;
        }

        public bool Open()
        {
            bool ok;

            _lockIpc.WaitOne();

            try
            {
                _channel = zCreateIPC(_portName);
                ChannelServices.RegisterChannel(_channel, false);
                ok = true;
            }
            catch (RemotingException)
            {
                ok = false;
            }

            if (!ok) _channel = null;

            _lockIpc.ReleaseMutex();

            return ok;
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

        private IpcChannel zCreateIPC(string pnam)
        {
            IDictionary prps = new Hashtable();

            prps["portName"] = pnam;
            prps["name"] = pnam + "Ipc";
            prps["authorizedGroup"] = zGetNameFromSID(WellKnownSidType.WorldSid);     // -- 29.08.16 : cf en-tête
            prps["typeFilterLevel"] = "Full";

            BinaryClientFormatterSinkProvider cprov = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider sprov = new BinaryServerFormatterSinkProvider();
            sprov.TypeFilterLevel = TypeFilterLevel.Full;

            IpcChannel channel = new IpcChannel(prps, cprov, sprov);

            return channel;
        }

        private static string zGetNameFromSID(WellKnownSidType sid)
        {
            SecurityIdentifier s = new SecurityIdentifier(sid, null);
            string name = s.Translate(typeof(NTAccount)).Value;

            return name;
        }

        private T zGetRemoteObject(string channelName, string objectUri)
        {
            T obj = (T)Activator.GetObject(typeof(T), zURL(channelName, objectUri));

            try
            {
                // L'object est vraiment récupéré lors du premier accès, donc on force le destin...
                int hash = obj.GetHashCode();
            }
            catch (RemotingException)
            {
                obj = default;
            }

            return obj;
        }

        private string zURL(string ipcChnam, string objuri)
        {
            return "ipc://" + ipcChnam + "/" + objuri;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}