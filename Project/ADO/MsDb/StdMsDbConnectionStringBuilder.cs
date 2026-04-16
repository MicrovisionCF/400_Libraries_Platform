using System.Data.SqlClient;

namespace Microvision.ADO.MicrosoftSqlServer
{
    public class StdMsDbConnectionStringBuilder : StdDbConnectionStringBuilder<SqlConnectionStringBuilder>
    {
        // ***************************************************************************************************
        // 23.04.25 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdMsDbConnectionStringBuilder() : base()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string DataSource
        {
            get => _core.DataSource;
            set => _core.DataSource = value;
        }

        public string InitialCatalog
        {
            get => _core.InitialCatalog;
            set => _core.InitialCatalog = value;
        }

        public bool IntegratedSecurity
        {
            get => _core.IntegratedSecurity;
            set => _core.IntegratedSecurity = value;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


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