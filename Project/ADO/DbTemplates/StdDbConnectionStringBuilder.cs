using System.Data.Common;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDbConnectionStringBuilder<TBuilder> : Citizen where TBuilder : DbConnectionStringBuilder, new()
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        protected TBuilder _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDbConnectionStringBuilder() : base()
        {
            _core = new TBuilder();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string ConnectionString
        {
            get => _core.ConnectionString;
            set => _core.ConnectionString = value;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _core = null;

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