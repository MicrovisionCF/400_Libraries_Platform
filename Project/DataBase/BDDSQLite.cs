using System.Data;
using System.Data.SQLite;
using System.Text;

using Microvision.Types;

namespace Microvision.DataBase
{
    public class BDDSQLite : Citizen, IBDDEngine, IBDDCreator
    {
        // ***************************************************************************************************
        // 31.05.17 : (libs 2.1) Création
        // 06.09.17 : Ajout de SQLiteError.NoError, CreateBase retourne une erreur comme OpenBase si SQLite non installé
        // 07.09.17 : Restriction des types utilisables
        // 21.11.19 : (libs 2.2) Ajout de disposes oubliés
        // 28.04.20 : Correction de la suppression / renommage de colonne
        // 10.11.20 : Ajout de Flush
        // 24.11.20 : Ajout de la création de la BDD avec un script
        // 21.01.21 : Garbage collector à la fermeture de la base pour accélérer le relachement du fichier
        // 25.03.21 : Ajout d'un étage d'appel pour correctement remonter l'erreur d'installation defectueuse
        // 13.04.22 : (libs 3.0)
        // 03.10.22 : Définition de nouveaux types acceptés, transformés en types existants
        // 23.03.23 : Possibilité de supprimer un champ dans une table qui possède une clé primaire
        //            Ajout de la version utilisateur
        //            Ajout de valeurs par défaut à l'ajout de nouvelle colonne
        // 22.05.23 : Renommage de la table de test car "test" est un nom réellement utilisé et qui entre en conflit
        // 14.11.23 : Les patterns sont gérés (SQLite utilise _ pour 1 caractère et % pour n caractères)
        // ***************************************************************************************************

        public enum SQLiteError
        {
            NoError,
            BadPassword, // En vérité le mot de passe chiffre la base donc un fichier qui n'est pas une base ou une base chiffrée on sait pas faire la différence
            AlreadyOpen,
            ProviderNotInstalled,
            FailOpen
        }


        private SQLiteConnection _sqlCon;
        private SQLiteError _lastError;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public BDDSQLite()
        {
            _lastError = SQLiteError.NoError;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected bool oCreateBase(string fileName)
        {
            bool success;

            _sqlCon?.Dispose();

            try
            {
                _sqlCon = new SQLiteConnection("Data Source=" + fileName + ";Version=3;New=True;Compress=True;");
                _sqlCon.Open();
                success = true;
            }
            catch
            {
                success = false;
                _sqlCon?.Dispose();
                _sqlCon = null;
            }

            return success;
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected bool oOpenBase(string baseName, string password, bool isExclusive)
        {
            bool success;

            try
            {
                _sqlCon = new SQLiteConnection("Data Source=" + "\"" + baseName + "\"" + ";Version=3;New=True;Compress=True;Foreign Keys=true;Password=" + zSQLQuote(password));
                _sqlCon.Open();
                _sqlCon.BusyTimeout = 1;
                _sqlCon.DefaultTimeout = 1;

                zExecute(_sqlCon, "BEGIN" + (isExclusive ? " EXCLUSIVE" : "")); // Débute une transaction
                zTestBase();

                success = true;
                _lastError = SQLiteError.NoError;
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode == (int)SQLiteErrorCode.NotADb)
                    _lastError = SQLiteError.BadPassword;
                else if (ex.ErrorCode == (int)SQLiteErrorCode.Busy)
                    _lastError = SQLiteError.AlreadyOpen;
                else
                    _lastError = SQLiteError.FailOpen;

                success = false;
            }

            return success;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static void zAddParameter(SQLiteCommand cmd, object o)
        {
            if (o is DateTime dat)
            {
                cmd.Parameters.AddWithValue("", (dat).Ticks); // Les dates sont sauvegardées en ticks
            }
            else if (o is byte[] btsArray)
            {
                Bytes tab = new Bytes(btsArray);
                cmd.Parameters.Add("", DbType.Binary, tab.Length).Value = tab.Array; // Conversion de Byte() en données binaires
            }
            else if (o is Bytes bts)
            {
                cmd.Parameters.Add("", DbType.Binary, bts.Length).Value = bts.Array; // Conversion de Bytes en données binaires
            }
            else
            {
                cmd.Parameters.AddWithValue("", o);
            }
        }

        private void zEnableForeignKeyConstrains(bool enable)
        {
            zExecute(_sqlCon, "END");
            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "PRAGMA foreign_keys = " + (enable ? "ON" : "OFF");
            cmd.ExecuteNonQuery();
            zExecute(_sqlCon, "BEGIN");
        }

        private static void zExecute(SQLiteConnection conn, string cmd)
        {
            SQLiteCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = cmd;
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
        }

        private static string zFormatPattern(string input)
        {
            return input.Replace("?", "_").Replace("*", "%");
        }

        private static DbType zGetDbType(string sqliteType)
        {
            DbType output = sqliteType switch
            {
                "TEXT" => DbType.String,
                "NUMERIC" => DbType.Double,
                "REAL" => DbType.Double,
                "INTEGER" => DbType.Int64,
                "BLOB" => DbType.Binary,
                _ => throw new ArgumentException("Type inconnu dans SQLite : " + sqliteType + " (Types autorisés : TEST, NUMERIC, REAL, INTEGER, BLOB")
            };

            return output;
        }

        private static string zGetSQLiteType(DbType type)
        {
            String output = type switch
            {
                DbType.Binary => "BLOB",
                DbType.Int32 => "INTEGER",
                DbType.Int64 => "INTEGER",
                DbType.Double => "NUMERIC",
                DbType.String => "TEXT",
                DbType.DateTime => "INTEGER",
                DbType.Boolean => "INTEGER",
                DbType.Single => "NUMERIC",
                _ => throw new ArgumentException("Type équivalent inconnu par SQLite : " + type.ToNameString() + " (Types natif : Binary, Int64, Double, String)")
            };

            return output;
        }

        private static void zSQLCopyTable(SQLiteConnection con, string tbSrc, List<string> colsSrc, string tbDst, List<string> colsDst)
        {
            string src = "";
            string dst = "";

            for (int i = 0; i < colsSrc.Count; i++)
            {
                src += zSQLQuote(colsSrc[i]);
                src += i < colsSrc.Count - 1 ? ", " : "";
                dst += zSQLQuote(colsDst[i]);
                dst += i < colsDst.Count - 1 ? ", " : "";
            }

            string query = "INSERT INTO " + zSQLQuote(tbDst) + " (" + dst + ") SELECT " + src + " FROM " + zSQLQuote(tbSrc) + ";";

            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static int zSQLCount(SQLiteConnection con, string tbnam)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT COUNT(*) FROM " + zSQLQuote(tbnam);
            int count = ConvertShop.ReadInt(cmd.ExecuteScalar());
            cmd.Dispose();

            return count;
        }

        private static void zSQLCreate(SQLiteConnection con, string tbnam, List<string> colsName, List<DbType> colsType, List<bool> colsKey)
        {
            string query = "CREATE TABLE " + zSQLQuote(tbnam) + " (";
            for (int i = 0; i < colsName.Count; i++)
            {
                query += zSQLQuote(colsName[i]) + " ";
                query += zGetSQLiteType(colsType[i]);
                if (colsKey[i])
                    query += " PRIMARY KEY AUTOINCREMENT UNIQUE";
                query += i < colsName.Count - 1 ? "," + SpecialChars.NewLine : ");";
            }

            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static int zSQLDeleteRow(SQLiteConnection con, string tbnam, string idnam, int idval)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE FROM " + zSQLQuote(tbnam) + " WHERE " + zSQLQuote(idnam) + " = ?";
            cmd.Parameters.AddWithValue("", idval);
            int deletedCount = cmd.ExecuteNonQuery();
            cmd.Dispose();

            return deletedCount;
        }

        private (List<string> names, List<DbType> types, List<bool> isKey) zSQLFieldsList(SQLiteConnection con, string tbnam, string idnam)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "PRAGMA table_info(" + zSQLQuote(tbnam) + ");";
            SQLiteDataReader reader = cmd.ExecuteReader();

            List<string> nams = new List<string>();
            List<DbType> types = new List<DbType>();
            List<bool> isKey = new List<bool>();

            while (reader.Read())
            {
                string nam = reader.GetString(1);
                if (!nam.Equals(idnam))
                {
                    nams.Add(nam);
                    types.Add(zGetDbType(reader.GetString(2)));
                    isKey.Add(reader.GetBoolean(5));
                }
            }

            reader.Close();
            cmd.Dispose();

            return (nams, types, isKey);
        }

        private static List<int> zSQLIDsWhere(SQLiteConnection con, string req, List<object> vals)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = req;

            vals?.ForEach(o =>
            {
                if (o is string s)
                    zAddParameter(cmd, zFormatPattern(s));
                else
                    zAddParameter(cmd, o);
            });

            vals?.ForEach(v => zAddParameter(cmd, v));

            SQLiteDataReader reader = cmd.ExecuteReader();

            List<int> ids = new List<int>();
            while (reader.Read())
                ids.Add(reader.GetInt32(0));

            reader.Close();
            cmd.Dispose();

            return ids;
        }

        private static void zSQLInsertID(SQLiteConnection con, string tbnam, string idnam)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO " + zSQLQuote(tbnam) + " (" + zSQLQuote(idnam) + ") VALUES(NULL)";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static int zSQLLastInsertedID(SQLiteConnection con)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT last_insert_rowid()";
            int id = ConvertShop.ReadInt(cmd.ExecuteScalar());
            cmd.Dispose();

            return id;
        }

        private static string zSQLQuote(string s)
        {
            // Entoure de guillemets et supprime ceux dans la chaine : pas de guillemet dans les noms de table / champs

            string output = "";

            if (s is not null)
                output = "\"" + s.Replace("\"", "") + "\"";

            return output;
        }

        private static List<object> zSQLReadRecord(SQLiteConnection con, string tbnam, string idnam, int recid, List<string> flds)
        {
            StringBuilder req = new StringBuilder("SELECT ");

            flds.ForEach(f => req.Append(zSQLQuote(f) + ", "));
            req.Remove(req.Length - 2, 2);

            req.Append(" FROM " + zSQLQuote(tbnam));
            req.Append(" WHERE " + zSQLQuote(idnam) + " = ?");

            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = req.ToString();
            cmd.Parameters.AddWithValue("", recid);

            SQLiteDataReader reader = cmd.ExecuteReader();
            reader.Read();
            object[] vals = new object[flds.Count];
            reader.GetValues(vals);

            reader.Close();
            cmd.Dispose();

            return vals.ToList();
        }

        private static List<string> zSQLTablesList(SQLiteConnection con)
        {
            SQLiteCommand cmd = new SQLiteCommand(con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = \"table\"";
            SQLiteDataReader reader = cmd.ExecuteReader();

            List<string> tables = new List<string>();
            while (reader.Read())
            {
                string nam = reader.GetString(0);
                if (!nam.StartsWith("sqlite")) tables.Add(nam); // Les tables commencant par sqlite sont des tables systeme
            }

            reader.Close();
            cmd.Dispose();

            return tables;
        }

        private static void zSQLUpdateRow(SQLiteConnection con, string tbnam, string idnam, int idval, List<string> flds, List<object> values)
        {
            if (flds.Count > 0)
            {
                StringBuilder qry = new StringBuilder("UPDATE " + zSQLQuote(tbnam) + " SET ");
                flds.ForEach(f => qry.Append(zSQLQuote(f) + " = ?, "));
                qry.Remove(qry.Length - 2, 2);
                qry.Append(" WHERE " + zSQLQuote(idnam) + " = " + idval);

                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = qry.ToString();

                values.ForEach(v => zAddParameter(cmd, v));

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void zTestBase()
        {
            // Sert uniquement à tester si la base répond correctement ou si des choses sont verrouillées auquel cas les exceptions correspondantes seront levées

            ((IBDDCreator)this).CreateTable("MicrovisionTestDatabaseTable", null, "id");
            ((IBDDEngine)this).ReadTables();
            ((IBDDCreator)this).KillTable("MicrovisionTestDatabaseTable");
        }

        private bool zTryCreate(string filnam, string sqlscript)
        {
            bool success;

            try
            {
                success = oCreateBase(filnam);
                if (sqlscript is not null) zExecute(_sqlCon, sqlscript);
                zExecute(_sqlCon, "BEGIN");
                _lastError = SQLiteError.NoError;
            }
            catch
            {
                success = false;
                _lastError = SQLiteError.ProviderNotInstalled;
            }

            return success;
        }

        private static void zzRemoveColumn(string colNameToRemove, List<string> colsName, List<DbType> colsType, List<bool> colsKey)
        {
            int index = colsName.FindIndex(l => l.Equals(colNameToRemove));
            colsName.RemoveAt(index);
            colsType.RemoveAt(index);
            colsKey.RemoveAt(index);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

        // ####################################
        // As IBDDCreator
        // ####################################

        bool IBDDCreator.AddField(string tableName, string fieldName, DbType fieldType, bool isAutoIncrement)
        {
            // Pas d'AUTOINCREMENT, uniquement à la création de la table

            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "ALTER TABLE " + zSQLQuote(tableName) + " ADD " + zSQLQuote(fieldName) + " " + zGetSQLiteType(fieldType) + zDefaultSpecifier(fieldType);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            return true;
        }

        private string zDefaultSpecifier(DbType fieldType)
        {
            return fieldType switch
            {
                DbType.Binary => "",
                DbType.Int32 => " DEFAULT '0'",
                DbType.Int64 => " DEFAULT '0'",
                DbType.Double => " DEFAULT '0'",
                DbType.String => " DEFAULT ''",
                DbType.DateTime => " DEFAULT '0'",
                DbType.Boolean => " DEFAULT '0'",
                DbType.Single => " DEFAULT '0'",
                _ => ""
            };
        }

        bool IBDDCreator.CreateBase(string fileName)
        {
            // Ne pas appeler de code SQLite ici sinon l'appel plante directement en cas de mauvais installation
            // Il faut que ce soit le code dans le Try qui pète pour pouvoir retourner le code d'erreur

            bool success;

            try
            {
                success = zTryCreate(fileName, null);
            }
            catch
            {
                success = false;
                _lastError = SQLiteError.ProviderNotInstalled;
            }

            return success;
        }

        bool IBDDCreator.CreateBase(string fileName, string sqlScript)
        {
            // Ne pas appeler de code SQLite ici sinon l'appel plante directement en cas de mauvais installation
            // Il faut que ce soit le code dans le Try qui pète pour pouvoir retourner le code d'erreur

            bool success;

            try
            {
                success = zTryCreate(fileName, sqlScript);
            }
            catch
            {
                success = false;
                _lastError = SQLiteError.ProviderNotInstalled;
            }

            return success;
        }

        bool IBDDCreator.CreateTable(string tableName, string indexName, string indexField)
        {
            zSQLCreate(_sqlCon, tableName, new[] { indexField }.ToList(), new[] { DbType.Int64 }.ToList(), new[] { true }.ToList());

            // Création index
            if (indexName is not null)
            {
                SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "CREATE INDEX " + zSQLQuote(tableName + "_" + indexName) + " ON " + zSQLQuote(tableName) + " (" + zSQLQuote(indexField) + ")";

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            return true;
        }

        void IBDDCreator.KillField(string tableName, string fieldName)
        {
            zEnableForeignKeyConstrains(false);

            string oldName = tableName + "_old";
            (List<string> colsName, List<DbType> colsType, List<bool> colsKey) = zSQLFieldsList(_sqlCon, tableName, "");

            zzRemoveColumn(fieldName, colsName, colsType, colsKey);

            // Renommer la table en tbnam_old
            ((IBDDCreator)this).RenameTable(tableName, oldName);

            // Créer la table tbnam depuis les infos de structure sans fldnam
            zSQLCreate(_sqlCon, tableName, colsName, colsType, colsKey);

            // Copier tbnam_old dans tbnam
            zSQLCopyTable(_sqlCon, oldName, colsName, tableName, colsName);

            // Supprimer tbnam_old
            ((IBDDCreator)this).KillTable(oldName);

            zEnableForeignKeyConstrains(true);
        }

        void IBDDCreator.KillTable(string tableName)
        {
            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DROP TABLE " + zSQLQuote(tableName);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        void IBDDCreator.RenameField(string tableName, string fieldName, string newFieldName)
        {
            string oldName = tableName + "_old";
            (List<string> oldColsName, List<DbType> colsType, List<bool> colsKey) = zSQLFieldsList(_sqlCon, tableName, "");

            List<string> newColsName = new List<string>(oldColsName);
            newColsName[newColsName.IndexOf(fieldName)] = newFieldName;

            // Renommer la table en tbnam_old
            ((IBDDCreator)this).RenameTable(tableName, oldName);

            // Créer la table tbnam depuis les infos de structure sans fldnam
            zSQLCreate(_sqlCon, tableName, newColsName, colsType, colsKey);

            // Copier tbnam_old dans tbnam
            zSQLCopyTable(_sqlCon, oldName, oldColsName, tableName, newColsName);

            // Supprimer tbnam_old
            ((IBDDCreator)this).KillTable(oldName);
        }

        void IBDDCreator.RenameTable(string tableName, string newTableName)
        {
            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "ALTER TABLE " + zSQLQuote(tableName) + " RENAME TO " + zSQLQuote(newTableName);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        void IBDDCreator.SetUserVersion(int version)
        {
            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "PRAGMA user_version=" + version.ToString() + ";";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        // ####################################
        // As IBDDEngine
        // ####################################

        int IBDDEngine.AddRecord(string tableName, string indexFieldName)
        {
            zSQLInsertID(_sqlCon, tableName, indexFieldName);
            return zSQLLastInsertedID(_sqlCon);
        }

        string IBDDEngine.BaseProvider() => "SQLite";

        string IBDDEngine.BaseVersion() => _sqlCon.ServerVersion;

        void IBDDEngine.CloseBase()
        {
            zExecute(_sqlCon, "END"); // Mets fin à la transaction

            _sqlCon.Close();
            _sqlCon.Dispose();
            _sqlCon = null;

            GC.Collect();
        }

        void IBDDEngine.Flush()
        {
            // TODO : le fonctionnement avait été calqué sur la classe historique BDDAce12, mais ça serait bien mieux de gérer efficacement
            // avec des COMMIT par exemple...

            zExecute(_sqlCon, "END");
            zExecute(_sqlCon, "BEGIN");
        }

        List<int> IBDDEngine.GetRecordIds(string tabOrSql, List<object> parameters) => zSQLIDsWhere(_sqlCon, tabOrSql, parameters);

        bool IBDDEngine.KillRecord(string tableName, string indexName, int recordId) => zSQLDeleteRow(_sqlCon, tableName, indexName, recordId) == 1;

        int IBDDEngine.LastError() => (int)_lastError; // TODO

        bool IBDDEngine.NewPassword(string oldPassword, string newPassword)
        {
            _sqlCon.ChangePassword(newPassword);
            return true;
        }

        bool IBDDEngine.OpenBase(string baseName, string password, bool isExclusive)
        {
            bool success;

            // Ne pas appeler de code SQLite ici sinon l'appel plante directement en cas de mauvais installation
            // Il faut que ce soit le code dans le Try qui pète pour pouvoir retourner le code d'erreur

            try
            {
                success = oOpenBase(baseName, password, isExclusive);
            }
            catch
            {
                // Les erreurs SQLite étant gérées dans la privée, si ça pète jusqu'ici c'est surement une erreur d'installation
                success = false;
                _lastError = SQLiteError.ProviderNotInstalled;
            }

            return success;
        }

        (List<string> fieldsName, List<DbType> fieldsType) IBDDEngine.ReadFields(string tableName, string indexName)
        {
            (List<string> fieldsName, List<DbType> fieldsType, _) = zSQLFieldsList(_sqlCon, tableName, indexName);
            return (fieldsName, fieldsType);
        }

        List<object> IBDDEngine.ReadRecord(string tableName, string indexName, int recordId, List<string> fieldsName) => zSQLReadRecord(_sqlCon, tableName, indexName, recordId, fieldsName);

        List<string> IBDDEngine.ReadTables() => zSQLTablesList(_sqlCon);

        int IBDDEngine.RecordsCount(string tableName) => zSQLCount(_sqlCon, tableName);

        int IBDDEngine.UserVersion()
        {
            SQLiteCommand cmd = new SQLiteCommand(_sqlCon);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "PRAGMA user_version;";
            SQLiteDataReader reader = cmd.ExecuteReader();

            int id = -1;
            if (reader.Read()) id = reader.GetInt32(0);

            reader.Close();
            cmd.Dispose();

            return id;
        }

        void IBDDEngine.WriteRecord(string tableName, string indexName, int recordId, List<string> fieldsName, List<object> fieldsValue) => zSQLUpdateRow(_sqlCon, tableName, indexName, recordId, fieldsName, fieldsValue);


    }
}