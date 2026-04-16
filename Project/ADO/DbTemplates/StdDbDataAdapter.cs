using System.Data.Common;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDbDataAdapter<TDataAdapter, TCommand, TConnection, TTransaction> : Citizen
        where TDataAdapter : DbDataAdapter, new()
        where TCommand : DbCommand, new()
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private TCommand _selectCommand;
        private TCommand _insertCommand;
        private TCommand _updateCommand;
        private TCommand _deleteCommand;

        private TDataAdapter _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDbDataAdapter() : base()
        {
            _selectCommand = new TCommand();
            _insertCommand = new TCommand();
            _updateCommand = new TCommand();
            _deleteCommand = new TCommand();

            _core = new TDataAdapter();
            _core.SelectCommand = _selectCommand;
            _core.InsertCommand = _insertCommand;
            _core.UpdateCommand = _updateCommand;
            _core.DeleteCommand = _deleteCommand;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public TCommand DeleteCommand => _deleteCommand;

        public TCommand InsertCommand => _insertCommand;

        public TCommand SelectCommand => _selectCommand;

        public TCommand UpdateCommand => _updateCommand;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Fill(StdDataTable table, TConnection connection)
        {
            _core.SelectCommand.Connection = connection;
            _core.Fill(table.Core);
            _core.SelectCommand.Connection = null;
        }

        public void Update(StdDataTable table, TConnection connection, TTransaction transaction)
        {
            TCommand[] commands = [_insertCommand, _updateCommand, _deleteCommand];

            foreach (TCommand command in commands)
            {
                command.Connection = connection;
                command.Transaction = transaction;
            }

            _core.Update(table.Core);

            foreach (TCommand command in commands)
            {
                command.Connection = null;
                command.Transaction = null;
            }
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

            if (_deleteCommand is not null)
            {
                if (isExplicit) _deleteCommand.Dispose();
                _deleteCommand = null;
            }

            if (_updateCommand is not null)
            {
                if (isExplicit) _updateCommand.Dispose();
                _updateCommand = null;
            }

            if (_insertCommand is not null)
            {
                if (isExplicit) _insertCommand.Dispose();
                _insertCommand = null;
            }

            if (_selectCommand is not null)
            {
                if (isExplicit) _selectCommand.Dispose();
                _selectCommand = null;
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