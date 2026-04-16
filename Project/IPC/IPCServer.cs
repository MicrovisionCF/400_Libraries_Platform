using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Principal;
using System.Threading;

using Microvision.Types;

namespace Microvision.Platform
{
    public class IPCServer<T> : Citizen where T : RemoteObject
    {
        // ***************************************************************************************************
        // 28.09.20 : Création
        // 27.01.22 : (libs 3.0)
        // 09.12.22 : Mutexage des démarrages / arrêts et ajout des propriétés pour changer les noms manuellement.
        // ***************************************************************************************************

        private string _name, _channelName, _objectUri;
        private T _publicData;
        private ObjRef _publicDataRef; //C'est peut être pas utilisé, mais on garde quand même une référence dessus
        private IpcChannel _channel;
        private Mutex _lockIpc;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public IPCServer(string name)
        {
            _name = name;
            _channelName = _name + "Channel";
            _objectUri = _name + "Data";
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


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Close()
        {
            _lockIpc.WaitOne();
            ChannelServices.UnregisterChannel(_channel);
            _publicData = null;
            _publicDataRef = null;
            _lockIpc.ReleaseMutex();
        }

        public bool Open()
        {
            bool ok;

            _lockIpc.WaitOne();

            try
            {
                _channel = zCreateIPC(_channelName);
                ChannelServices.RegisterChannel(_channel, false);
                ok = true;
            }
            catch (RemotingException)
            {
                ok = false;
            }

            _lockIpc.ReleaseMutex();

            return ok;
        }

        public bool SetObject(T obj)
        {
            bool ok;

            try
            {
                _publicData = obj;
                _publicDataRef = zRegisterChannel(_publicData, _objectUri);
                ok = _publicDataRef is not null;
            }
            catch (RemotingException)
            {
                _publicData = null;
                ok = false;
            }

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
            prps["authorizedGroup"] = zGetNameFromSID(WellKnownSidType.WorldSid);
            prps["typeFilterLevel"] = "Full";

            BinaryClientFormatterSinkProvider cprov = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider sprov = new BinaryServerFormatterSinkProvider();
            sprov.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IpcChannel channel = new IpcChannel(prps, cprov, sprov);

            return channel;
        }

        private static string zGetNameFromSID(WellKnownSidType sid)
        {
            SecurityIdentifier s = new SecurityIdentifier(sid, null);
            string name = s.Translate(typeof(NTAccount)).Value;

            return name;
        }

        private ObjRef zRegisterChannel(RemoteObject obj, string objuri)
        {
            ObjRef reference;

            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(obj.GetType(), objuri, WellKnownObjectMode.Singleton);

                // -- this makes the object acquired by clients to be the same instance that the one created by the server :
                reference = RemotingServices.Marshal(obj, objuri, obj.GetType());
            }
            catch
            {
                reference = null;
                // ...
            }

            return reference;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}