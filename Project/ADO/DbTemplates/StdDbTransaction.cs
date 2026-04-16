using System.Data.Common;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDbTransaction<TTransaction> : Citizen where TTransaction : DbTransaction
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private TTransaction _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDbTransaction(TTransaction core) : base()
        {
            _core = core;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public TTransaction Core => _core;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Commit()
        {
            _core.Commit();
        }

        public void Rollback()
        {
            _core.Rollback();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_core is not null)
            {
                if (isExplicit) _core.Dispose();
                _core = null;
            }

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