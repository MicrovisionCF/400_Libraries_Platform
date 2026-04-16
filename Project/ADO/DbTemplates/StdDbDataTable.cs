using System.Data.Common;

namespace Microvision.ADO
{
    public class StdDbDataTable<TDataAdapter, TCommand, TConnection, TTransaction> : StdDataTable
        where TDataAdapter : DbDataAdapter, new()
        where TCommand : DbCommand, new()
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private StdDbDataAdapter<TDataAdapter, TCommand, TConnection, TTransaction> _dataAdapter;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDbDataTable(string name, StdDbDataAdapter<TDataAdapter, TCommand, TConnection, TTransaction> dataAdapter) : base(name)
        {
            _dataAdapter = dataAdapter.AddLife();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_dataAdapter is not null)
            {
                if (isExplicit) _dataAdapter.Dispose();
                _dataAdapter = null;
            }

            base.oDispose(isExplicit);
        }

        protected void oFill(TConnection connection)
        {
            _dataAdapter.Fill(this, connection);
        }

        protected void oUpdate(TConnection connection, TTransaction transaction)
        {
            _dataAdapter.Update(this, connection, transaction);
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