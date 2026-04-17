using System.Data;

using Microvision.Types;

namespace Microvision.DataBase
{
    public interface IBDDRecord
    {
        // ***************************************************************************************************
        //            Création : (iVariantArray) interface servant à entrer / sortir des enregistrements de bases à données.
        // 12.11.08 : renommé iBDDRecord et intégration à nouvelle librairie MVDBase, pour VBNet.
        // 21.06.11 : libs 1.8
        // 21.01.14 : libs 2.0, transmission des noms de champs pour rester indépendant de leur ordre, très
        //            changeant, et suppression de ValsCount, inutilisé.
        // 19.09.16 : (iBDDRecordL) variante pour listes
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        List<object> GetVals(List<string> fieldsName);
        void SetVals(List<string> fieldsName, List<object> values);
    }

    public class StdBDDTable : Citizen
    {
        // ***************************************************************************************************
        //            création : abstraction d'une table de bdd, avec qq règles µV :
        //            - ouvre et ferme la base à chaque accès
        //            - avec un moteur défini de l'extérieur
        //            - ya toujours un champ ID, indexé (non rendu dans la liste des champs)
        //            - enregistrements définis par iBDDRecord.
        // 04.10.06 : nom du champ ID et nom de l'index rendus variables, pour lire les bases Leica IM1000.
        // 21.06.11 : libs 1.8
        // 21.01.14 : libs 2.0, modif de la création, qq subs déplacées vers StdBDD.
        // 05.07.16 : _fldNames et _fldTypes as list(of), des surcharges.
        // 19.09.16 : exploitation du iBDDEngine à listes et surcharges pour iBDDRecordL à listes.
        // 07.12.16 : FindRecordsByDate ne marche plus depuis loooooongtemps (VBNet / libs 2.0), pour cause
        //            de conversions en texte mal foutues et inutiles.
        // 13.12.16 : FindRecordsByWord marche pas non plus ==> changement de caractère sauvage de Like.
        // 12.05.17 : (libs 2.1)
        // 07.09.17 : Plus de DISTINCTROW dans les requêtes parce que 1/ c'est propre à Access 2/ on sélectionne les ID,
        //            qui sont déjà uniques par définition
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private struct xField
        {
            public string name;
            public DbType type;

            public xField(string nam, DbType typ)
            {
                this.name = nam;
                this.type = typ;
            }
        }


        private string _fileName;
        private string _password;
        private IBDDEngine _engine;

        private string _name;
        private List<string> _fieldsName;
        private List<DbType> _fieldsType;
        private string _idFieldName;
        private string _indexName;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdBDDTable() : this("ID", "clef") // -- valeurs éternelles, jusqu'au 04.10.06
        {
        }

        public StdBDDTable(string idFieldName, string indexName) : base()
        {
            _password = "";
            _fileName = "";
            _name = "";

            _fieldsName = new List<string>();
            _fieldsType = new List<DbType>();
            _idFieldName = idFieldName;
            _indexName = indexName;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public string DataSourceName => _fileName;

        public IBDDEngine Engine => _engine;

        public int FieldsCount => _fieldsName.Count;

        public string IDFieldName => _idFieldName;

        public string IndexName => _indexName;

        public string Name
        {
            get => _name;

            internal set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        public string Password
        {
            get => _password;

            set
            {
                if (_password != value)
                {
                    _password = value;
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool AddField(string fieldName, DbType fieldType)
        {
            bool ok = false;

            if (_engine is IBDDCreator mng && _engine.OpenBase(_fileName, _password))
            {
                ok = mng.AddField(_name, fieldName, fieldType);
                _engine.CloseBase();

                if (ok)
                {
                    _fieldsName.Add(fieldName);
                    _fieldsType.Add(fieldType);
                }
            }

            return ok;
        }

        public int AddRecord(IBDDRecord record)
        {
            int id = -1;

            List<object> v = record.GetVals(_fieldsName);
            if (_engine.OpenBase(_fileName, _password))
            {
                id = _engine.AddRecord(_name, _idFieldName);
                if (id > 0) _engine.WriteRecord(_name, _idFieldName, id, _fieldsName, v);
                _engine.CloseBase();
            }

            return id;
        }

        internal void Close()
        {
            _engine.Dispose();
            _engine = null;
            _fileName = "";
            _name = "";
            _fieldsName.Clear();
            _fieldsType.Clear();
        }

        public int FindField(string fieldName)
        {
            return _fieldsName.IndexOf(fieldName);
        }

        public List<int> FindRecordsAll(string orderByField = "")
        {
            List<int> ids = null;

            if (_engine.OpenBase(_fileName, _password))
            {
                ids = _engine.GetRecordIds(zSQLFindAll(_name, _idFieldName, orderByField), null);
                _engine.CloseBase();
            }

            return ids;
        }

        public List<int> FindRecordsByDate(string dateFielName, object fromDate, object toDate)
        {
            List<int> ids = null;
            List<object> vals = new List<object>();

            if (fromDate is not null) vals.Add(fromDate);
            if (toDate is not null) vals.Add((TimeSpan)toDate + new TimeSpan(1, 0, 0, 0));

            if (_engine.OpenBase(_fileName, _password))
            {
                ids = _engine.GetRecordIds(zSQLFindDate(_name, _idFieldName, dateFielName, fromDate is not null, toDate is not null), vals);
                _engine.CloseBase();
            }

            return ids;
        }

        public List<int> FindRecordsByWord(string field, string word, string orderByField = "")
        {
            List<int> ids = null;

            if (_engine.OpenBase(_fileName, _password))
            {
                word = word.Replace("'", "''").Surround("%");
                ids = _engine.GetRecordIds(zSQLFindWord(_name, _idFieldName, field, orderByField), new List<object> { word });
                _engine.CloseBase();
            }

            return ids;
        }

        public List<int> FindRecordsSQL(string fieldName, string operation, object value, string orderByField = "")
        {
            List<int> ids = null;
            string where = zSQLOneLine(fieldName, operation);

            if (_engine.OpenBase(_fileName, _password))
            {
                ids = _engine.GetRecordIds(zSQLGeneric(_name, _idFieldName, where, orderByField), new List<object> { value });
                _engine.CloseBase();
            }

            return ids;
        }

        public List<int> FindRecordsSQL(List<string> fields, List<string> operations, List<object> values, string logoper = "AND", string orderByField = "")
        {
            List<int> ids = null;
            string where = "";

            for (int i = 0; i < fields.Count; i++)
            {
                where += zSQLOneLine(fields[i], operations[i]).SurroundParenthesis();
                if (i < fields.Count - 1) where += logoper.Surround(" ");
            }

            if (_engine.OpenBase(_fileName, _password))
            {
                ids = _engine.GetRecordIds(zSQLGeneric(_name, _idFieldName, where, orderByField), values);
                _engine.CloseBase();
            }

            return ids;
        }

        public string GetFieldName(int no)
        {
            return _fieldsName[no];
        }

        public (List<string> fieldsName, List<DbType> fieldsType) GetFields()
        {
            return (new List<string>(_fieldsName), new List<DbType>(_fieldsType));
        }

        public DbType GetFieldType(int no)
        {
            return _fieldsType[no];
        }

        public IBDDRecord GetRecord(int id, IBDDRecord record)
        {
            IBDDRecord output = null;

            if (_engine.OpenBase(_fileName, _password))
            {
                List<object> v = _engine.ReadRecord(_name, _idFieldName, id, _fieldsName);
                _engine.CloseBase();
                output = record;
                output.SetVals(_fieldsName, v);
            }

            return output;
        }

        public bool GetRecords(List<int> ids, List<IBDDRecord> records)
        {
            bool ok = false;

            if (_engine.OpenBase(_fileName, _password))
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    List<object> v = _engine.ReadRecord(_name, _idFieldName, ids[i], _fieldsName);
                    records[i].SetVals(_fieldsName, v);
                }

                _engine.CloseBase();
                ok = true;
            }

            return ok;
        }

        public bool HasField(string fieldName)
        {
            return _fieldsName.IndexOf(fieldName) >= 0;
        }

        public void KillField(int no)
        {
            if (_engine is IBDDCreator eng && _engine.OpenBase(_fileName, _password))
            {
                eng.KillField(_name, _fieldsName[no]);
                _engine.CloseBase();
                _fieldsName.RemoveAt(no);
                _fieldsType.RemoveAt(no);
            }
        }

        public void KillRecord(int id)
        {
            if (_engine.OpenBase(_fileName, _password))
            {
                _engine.KillRecord(_name, _idFieldName, id);
                _engine.CloseBase();
            }
        }

        internal void Open(string fileName, string password, string tableName, IBDDEngine engine)
        {
            _fileName = fileName;
            _password = password;
            _name = tableName;
            _engine = engine.AddLife();

            if (_engine.OpenBase(_fileName, _password))
            {
                (_fieldsName, _fieldsType) = _engine.ReadFields(_name, _idFieldName);
                _engine.CloseBase();
            }
        }

        public int RecordsCount()
        {
            int count = 0;

            if (_engine.OpenBase(_fileName, _password))
            {
                count = _engine.RecordsCount(_name);
                _engine.CloseBase();
            }

            return count;
        }

        public void RenameField(int fieldNo, string newFieldName)
        {
            if (_engine is IBDDCreator eng && _engine.OpenBase(_fileName, _password))
            {
                eng.RenameField(_name, _fieldsName[fieldNo], newFieldName);
                _engine.CloseBase();
                _fieldsName[fieldNo] = newFieldName;
            }
        }

        public void SetRecord(int id, IBDDRecord record)
        {
            List<object> v = record.GetVals(_fieldsName);

            if (_engine.OpenBase(_fileName, _password))
            {
                _engine.WriteRecord(_name, _idFieldName, id, _fieldsName, v);
                _engine.CloseBase();
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _fieldsName = null;
            _fieldsType = null;

            if (_engine is not null)
            {
                if (isExplicit) _engine.Dispose();
                _engine = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zSQLFindAll(string tableName, string indexName, string orderByField)
        {
            string sql = "SELECT [¤TAB]." + indexName;
            sql += " FROM [¤TAB]";

            if (orderByField != "")
                sql += " ORDER BY [¤TAB].[¤ORD];";
            else
                sql += ";";

            sql = sql.Replace("¤TAB", tableName);

            if (orderByField != "") sql = sql.Replace("¤ORD", orderByField);

            return sql;
        }

        private static string zSQLFindDate(string tableName, string indexName, string fieldName, bool hasDateFrom, bool hasDateTo)
        {
            string sql = "SELECT [¤TAB]." + indexName + ", [¤TAB].[¤FLD]";
            sql += " FROM [¤TAB]";

            if (!hasDateTo)
                sql += " WHERE (([¤TAB].[¤FLD] >= ?))";
            else if (!hasDateFrom)
                sql += " WHERE (([¤TAB].[¤FLD] < ?))";
            else
                sql += " WHERE (([¤TAB].[¤FLD] >= ? And [¤TAB].[¤FLD] < ?))";

            sql += " ORDER BY [¤TAB].[¤FLD];";
            sql = sql.Replace("¤TAB", tableName);
            sql = sql.Replace("¤FLD", fieldName);

            return sql;
        }

        private static string zSQLFindWord(string tableName, string indexName, string fieldName, string orderByField)
        {
            string sql = "SELECT [¤TAB]." + indexName + ", [¤TAB].[¤FLD]";
            sql += " FROM [¤TAB]";
            sql += " WHERE (([¤TAB].[¤FLD] Like ?))";

            if (orderByField != "")
                sql += " ORDER BY [¤TAB].[¤ORD];";
            else
                sql += ";";

            sql = sql.Replace("¤TAB", tableName);
            sql = sql.Replace("¤FLD", fieldName);
            if (orderByField != "") sql = sql.Replace("¤ORD", orderByField);

            return sql;
        }

        private static string zSQLGeneric(string tableName, string indexName, string where, string orderByField)
        {
            string sql = "SELECT [¤TAB]." + indexName;
            sql += " FROM [¤TAB]";
            sql += " WHERE (" + where + ")";

            if (orderByField != "")
                sql += " ORDER BY [¤TAB].[¤ORD];";
            else
                sql += ";";

            sql = sql.Replace("¤TAB", tableName);
            if (orderByField != "") sql = sql.Replace("¤ORD", orderByField);

            return sql;
        }

        private static string zSQLOneLine(string fieldName, string oper)
        {
            return "[¤TAB].[" + fieldName + "] " + oper + " ?";
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}