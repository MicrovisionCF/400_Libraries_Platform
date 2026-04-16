using System.Data.Common;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDbConnection<TConnection> : Citizen where TConnection : DbConnection, new()
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        protected TConnection _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDbConnection(string connectionString) : base()
        {
            _core = new TConnection();
            _core.ConnectionString = connectionString;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public TConnection Core => _core;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Close()
        {
            _core.Close();
        }

        public void Open()
        {
            _core.Open();
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