using System;
using System.Data;
using System.Linq;

using Microvision.Types;

namespace Microvision.ADO
{
    public class StdDataColumn : Citizen
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        private DataColumn _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected StdDataColumn(string name, Type type) : base()
        {
            _core = new DataColumn(name, type);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public DataColumn Core => _core;


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static DataColumn[] CreatePrimaryKey(params StdDataColumn[] columns)
        {
            return [.. columns.Select(o => o._core)];
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


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

        protected object oGetValue(int rowNo)
        {
            return _core.Table.Rows[rowNo][_core];
        }

        protected void oSetValue(int rowNo, object value)
        {
            _core.Table.Rows[rowNo][_core] = value;
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

    public class StdDataColumn<T> : StdDataColumn
    {
        // ***************************************************************************************************
        // 25.04.25 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdDataColumn(string name) : base(name, typeof(T))
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public T GetValue(int rowNo)
        {
            object result = oGetValue(rowNo);

            return result is not DBNull ? (T)result : default;
        }

        public void SetValue(int rowNo, T value)
        {
            oSetValue(rowNo, value);
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