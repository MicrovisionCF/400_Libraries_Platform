using System.Data;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDataTable : Citizen
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private StdList<StdDataColumn> _columns;
        private DataTable _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDataTable(string name) : base()
        {
            _columns = [];
            _core = new DataTable(name);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public DataTable Core => _core;

        public int Count => _core.Rows.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected StdDataColumn<T> oAddColumn<T>(string name)
        {
            StdDataColumn<T> column = new StdDataColumn<T>(name);

            _columns.Add(column.GiveLife());
            _core.Columns.Add(column.Core);

            return column;
        }

        protected void oAddRow(params object[] values)
        {
            _core.Rows.Add(values);
        }

        protected void oDeleteRow(int rowNo)
        {
            _core.Rows[rowNo].Delete();
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_core is not null)
            {
                if (isExplicit) _core.Dispose();
                _core = null;
            }

            if (_columns is not null)
            {
                if (isExplicit) _columns.Dispose();
                _columns = null;
            }

            base.oDispose(isExplicit);
        }

        protected void oSetPrimaryKey(params StdDataColumn[] columns)
        {
            _core.PrimaryKey = StdDataColumn.CreatePrimaryKey(columns);
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