using System.Data.SqlClient;
using System.Security;

namespace Microvision.ADO.MicrosoftSqlServer
{
    public class StdMsDbConnection : StdDbConnection<SqlConnection>
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private SecureString _password;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdMsDbConnection(string connectionString, string userId, string password) : base(connectionString)
        {
            _password = zMakePassword(password);
            _core.Credential = new SqlCredential(userId, _password);
        }

        public StdMsDbConnection(string connectionString) : base(connectionString)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public StdMsDbTransaction BeginTransaction()
        {
            return new StdMsDbTransaction(this);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_password is not null)
            {
                if (isExplicit) _password.Dispose();
                _password = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static SecureString zMakePassword(string password)
        {
            SecureString output = new SecureString();
            foreach (char c in password) output.AppendChar(c);
            output.MakeReadOnly();

            return output;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}