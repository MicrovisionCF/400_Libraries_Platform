using System.Data.SqlClient;

namespace Microvision.ADO.MicrosoftSqlServer
{
    public class StdMsDbDataTable : StdDbDataTable<SqlDataAdapter, SqlCommand, SqlConnection, SqlTransaction>
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdMsDbDataTable(string name, StdMsDbDataAdapter dataAdapter) : base(name, dataAdapter)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Fill(StdMsDbConnection connection)
        {
            oFill(connection.Core);
        }

        public void Update(StdMsDbConnection connection, StdMsDbTransaction transaction)
        {
            oUpdate(connection.Core, transaction.Core);
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


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}