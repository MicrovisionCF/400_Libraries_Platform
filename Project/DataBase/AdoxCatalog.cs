using System.Runtime.InteropServices;

using Microvision.Types;

using ADOX;

namespace Microvision.DataBase
{
    public class AdoxCatalog : Citizen
    {
        // ***************************************************************************************************
        // 22.01.14 : (création) encapsulation de Catalog, objet racine de ADOX, seul moyen que j'ai trouvé
        //            de créer une base à données Access. Je cite :
        //
        //            "Microsoft® ActiveX® Data Objects Extensions for Data Definition Language and Security
        //            (ADOX) is an extension to the ADO objects and programming model. ADOX includes objects
        //            for schema creation and modification, as well as security. Because it is an object-based
        //            approach to schema manipulation, you can write code that will work against various data
        //            sources regardless of differences in their native syntaxes."
        //
        //            ADOX est une librairie Com :
        //            - Msadox.dll, apparemment installée avec Windows,
        //            - dans "C:\Program Files (x86)\Common Files\System\ado\"
        //            - à référencer dans le projet ("Microsoft ADO Ext. for DDL and Security.")
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private Catalog _core;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AdoxCatalog() : base()
        {
            _core = new ADOX.Catalog();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void AddField(string tableName, string fieldName, DataTypeEnum fieldType, bool isAutoIncrement = false)
        {
            Table tb = _core.Tables[tableName];
            Column fld = new Column();
            fld.Name = fieldName;
            fld.Type = fieldType;

            if (isAutoIncrement)
            {
                fld.ParentCatalog = _core;
                fld.Properties["Autoincrement"].Value = true;
            }
            else if (zIsNullable(fieldType))
            {
                fld.Attributes = ColumnAttributesEnum.adColNullable;
            }

            tb.Columns.Append(fld);
        }

        public void AddIndex(string tableName, string fieldName, string indexName)
        {
            Table tb = _core.Tables[tableName];
            Index idx = new Index();
            idx.Name = indexName;
            idx.PrimaryKey = true;
            idx.Unique = true;
            idx.Columns.Append(fieldName);

            tb.Indexes.Append(idx);
        }

        public void AddTable(string tableName)
        {
            Table tb = new Table();
            tb.Name = tableName;

            _core.Tables.Append(tb);
        }

        public void CloseDatabase()
        {
            // -- _core.ActiveConnection est de type ADODB.Connection. L'appel à Close corrige un bug avec lequel
            // tout appel à Create empêche les ouvertures ultérieures en mode exclusif, donc (entre autres) le
            // changement de motdpasse.

            ((dynamic)_core.ActiveConnection).Close();
            _core.ActiveConnection = null;
        }

        public void CreateDatabase(string connectionString)
        {
            _core.Create(connectionString);
        }

        public void DeleteField(string tableName, string fieldName)
        {
            Table tb = _core.Tables[tableName];
            tb.Columns.Delete(fieldName);
        }

        public void DeleteTable(string tableName)
        {
            _core.Tables.Delete(tableName);
        }

        public void OpenDatabase(string connectionString)
        {
            _core.let_ActiveConnection(connectionString);
        }

        public void RenameField(string tableName, string fieldName, string newFieldName)
        {
            Table tb = _core.Tables[tableName];
            tb.Columns[fieldName].Name = newFieldName;
        }

        public void RenameTable(string tableName, string newTableName)
        {
            _core.Tables[tableName].Name = newTableName;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_core is not null)
            {
                Marshal.ReleaseComObject(_core);
                _core = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static bool zIsNullable(DataTypeEnum dtyp)
        {
            return dtyp != DataTypeEnum.adBoolean;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}