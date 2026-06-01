using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class TwainThread : Citizen
    {
        // ***************************************************************************************************
        // 17.03.23 : Création. Lorsque le scanner est "enabled", il envoie des messages à l'application
        //            via la message loop. Ce thread crée une message loop destinée à recevoir ces messages.
        // ***************************************************************************************************

        private readonly Thread _thread;

        private ApplicationContext? _appContext;
        private ISynchronizeInvoke? _sync;
        private IntPtr _hWnd;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainThread() : base()
        {
            _thread = new Thread(zThreadCallback);

            using Semaphore sem = new Semaphore(0, 1);
            _thread.Start(sem);
            sem.WaitOne();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public IntPtr HWnd => _hWnd;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void RunInUIThread(Action action)
        {
            oRunInUIThread(action);
        }

        public void SetMessageFilter(bool status, IMessageFilter messageFilter)
        {
            if (status) oRunInUIThread(() => Application.AddMessageFilter(messageFilter));
            else oRunInUIThread(() => Application.RemoveMessageFilter(messageFilter));
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_appContext is not null)
            {
                _appContext.ExitThread();
                // le Dispose est appelé dans le callback du thread.
                _appContext = null;
            }

            if (_thread.ThreadState == ThreadState.Running)
            {
                _thread.Join();
            }

            base.oDispose(isExplicit);
        }

        protected void oRunInUIThread(Action action)
        {
            _sync.ThrowIfNull();

            if (_sync.InvokeRequired) _sync.Invoke(action, null);
            else action();
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private void zThreadCallback(object? arg)
        {
            Semaphore locker = (arg as Semaphore).ThrowIfNull();

            using ApplicationContext appContext = new ApplicationContext();
            using Form form = new Form();

            _appContext = appContext;
            _sync = form;
            _hWnd = form.Handle;

            locker.Release();

            Application.Run(appContext);

            _sync = null;
            _hWnd = IntPtr.Zero;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}