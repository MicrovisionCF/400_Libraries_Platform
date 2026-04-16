using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microvision.ADO.MicrosoftSqlServer
{
    public abstract class StdMsDbTableBinder
    {
        // ***************************************************************************************************
        // 28.04.25 : Création, pour binder des tables de façon classiques. Hériter et surcharger oMake pour
        //            modifier le comportement des requêtes (contraintes etc)
        // ***************************************************************************************************

        private string _tableName;
        private string _idName;
        private List<(string name, SqlDbType type, int length)> _parameters;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdMsDbTableBinder(string tableName, string idName = "id")
        {
            _parameters = new List<(string name, SqlDbType type, int length)>();
            _tableName = tableName;
            _idName = idName;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public StdMsDbDataAdapter Make()
        {
            return oMake();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected void oAddColumn(string name, SqlDbType type, int length = 0)
        {
            _parameters.Add((name, type, length));
        }

        protected virtual StdMsDbDataAdapter oMake()
        {
            StdMsDbDataAdapter adapter = new StdMsDbDataAdapter();

            adapter.SelectCommand.CommandText = $"SELECT * FROM {_tableName}";

            adapter.InsertCommand.CommandText = $"INSERT INTO #Tabl# (#Cols#) OUTPUT INSERTED.{_idName} VALUES (#Vals#)"
                .Replace("#Tabl#", _tableName.SurroundBracket())
                .Replace("#Cols#", string.Join(", ", _parameters.Select(o => o.name.SurroundBracket())))
                .Replace("#Vals#", string.Join(", ", _parameters.Select(o => "@" + o.name)));
            adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

            adapter.InsertCommand.Parameters.AddRange([.. _parameters.Select(o => new SqlParameter("@" + o.name, o.type, o.length, o.name))]);

            adapter.UpdateCommand.CommandText = $"UPDATE #Tabl# SET #Sets# WHERE [{_idName}] = @{_idName}"
                .Replace("#Tabl#", _tableName.SurroundBracket())
                .Replace("#Sets#", string.Join(", ", _parameters.Select(o => $"{o.name.SurroundBracket()} = @{o.name}")));
            adapter.UpdateCommand.Parameters.Add(new SqlParameter("@" + _idName, SqlDbType.Int, 0, _idName));
            adapter.UpdateCommand.Parameters.AddRange([.. _parameters.Select(o => new SqlParameter("@" + o.name, o.type, o.length, o.name))]);

            adapter.DeleteCommand.CommandText = $"DELETE FROM [{_tableName}] WHERE [{_idName}] = @{_idName}";
            adapter.DeleteCommand.Parameters.Add(new SqlParameter("@" + _idName, SqlDbType.Int, 0, _idName));

            return adapter;
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