using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ADOX;
using Microvision.Types;

namespace Microvision.DataBase
{
    public class BDDAce12 : Citizen, IBDDCreator, IBDDEngine
    {
        // ***************************************************************************************************
        // 22.01.14 : (création) ACE est apparemment la version 64 bits de Jet, officiellement abandonné mais
        //            ressuscité car utilisé par Office 64 bits. C'est le seul format que j'ai trouvé pour
        //            créer une bdd (via ADOX). Il permet également de lire les bases jet, donc les bases
        //            Archimed 32 bits.
        //
        //            Ce provider n'est pas inclus dans Windows :
        //            - il est dispo sur le site "Microsoft Access Database Engine 2010 redistributable"
        //            - il s'installe via AccessDataBaseEngine_x64.exe
        //            - dans le répertoire "C:\Program Files\Common Files\Microsoft Shared\OFFICE14"
        //            - et s'identifie par "ACE.OLEDB.12.0"
        //            Si.
        // 04.02.14 : énumération de qq erreurs identifiées, à vocation documentaire. Les codes restent
        //            dépendants de OleDB
        // 19.09.16 : implémentation des surcharges de iBDDEngine.
        // 12.12.16 : dans iBDDEngine_AddRecord, changement de commande pour récupérer le dernier ID créé,
        //            pour faire marcher la vieille MakeCD.mdb (sans faire merder les autres base, j'espère)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 10.11.20 : Ajout de Flush, pas testé
        // 24.11.20 : Ajout de la création de la BDD avec un script, pas testé
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private struct xColumn
        {
            public string name;
            public OleDbType typ;
            public object length;
            public int ord;

            public xColumn(string nam, OleDbType typ, object length, int ord)
            {
                this.name = nam;
                this.typ = typ;
                this.length = length;
                this.ord = ord;
            }
        }

        private struct xTable
        {
            public string nam;
            public DateTime dte;

            public xTable(string nam, DateTime dte)
            {
                this.nam = nam;
                this.dte = dte;
            }
        }


        public enum AceError
        {
            NoError = 0x0,
            InvalidPassword = 3031,          // &H80040E4D Not a valid password.
            // sqlstate = "3031"
            // nativeerror = &hF88Ff88F
            FileTooOld = 3041,               // &H80004005 Cannot open a database created with a previous version of your application.
            // sqlstate = "3041"
            // nativeerror = &hFC05FC05
            AlreadyOpen = 3704,              // &H80004005 You attempted to open a database that is already opened by user 'Admin' on machine 'CHB64'. Try again when the database is available.
                                             // sqlstate = "3704"
                                             // nativeerror = &hDD6EFC00

            ProviderNotInstalled = int.MinValue + 0x00131509,       // -- System.InvalidOperationException à l'ouverture
            ClassNotRegistered = int.MinValue + 0x00040154         // -- HResult de Interop.COMException
        }


        public static string ACEProvider = KACEProvider;


        private const string KProvider = "Provider=";
        private const string KDataSource = "Data Source=";
        private const string KPassword = "Jet OLEDB:Database Password=";
        private const string KEngine = "Jet OLEDB:Engine Type=";
        private const string KMode = "Mode=";

        private const string KACEProvider = "Microsoft.ACE.OLEDB.12.0";


        private OleDbConnection _bdd;
        private int _errCode;        // -- très bidon, pour l'instant


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public BDDAce12() : base()
        {
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

        protected override void oDispose(bool isExplicit)
        {
            if (_bdd is not null)
            {
                if (isExplicit) _bdd.Dispose();
                _bdd = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static DataTypeEnum zAdoxDataType(DbType dbType)
        {
            DataTypeEnum output = dbType switch
            {
                DbType.Boolean => DataTypeEnum.adBoolean,
                DbType.Int16 => DataTypeEnum.adSmallInt,
                DbType.Int32 => DataTypeEnum.adInteger,
                DbType.Int64 => DataTypeEnum.adBigInt,
                DbType.Single => DataTypeEnum.adSingle,
                DbType.DateTime => DataTypeEnum.adDate,
                DbType.String => DataTypeEnum.adWChar,
                DbType.Binary => DataTypeEnum.adLongVarBinary,// -- pas exactement équivalent. Ceci produit un OleDbType.Binary de longueur 0, c'est ainsi qu'apparaissent les vieilles bases.
                _ => default
            };

            return output;
        }

        private static void zCloseCatalog(AdoxCatalog cat)
        {
            cat.CloseDatabase();
            cat.Dispose();
        }

        private int zCompareCols(xColumn c1, xColumn c2)
        {
            return c1.ord.CompareTo(c2.ord);
        }

        private int zCompareTabs(xTable t1, xTable t2)
        {
            return t1.dte.CompareTo(t2.dte);
        }

        private static string zConnectionString(string provider, string fileName, string password, bool isExclusive)
        {
            string cnx = KProvider + provider + "; ";
            cnx = cnx + KDataSource + fileName + "; ";
            cnx = cnx + KPassword + password + "; ";
            // cnx = cnx & KEngine & 5 & "; "
            if (isExclusive) cnx = cnx + KMode + "Share Exclusive";

            return cnx;
        }

        private static DbType zDBType(OleDbType otyp)
        {
            DbType output = otyp switch
            {
                OleDbType.Boolean => DbType.Boolean,
                OleDbType.SmallInt => DbType.Int16,
                OleDbType.Integer => DbType.Int32,
                OleDbType.BigInt => DbType.Int64,
                OleDbType.Single => DbType.Single,
                OleDbType.Date => DbType.DateTime,
                OleDbType.WChar => DbType.String,
                OleDbType.Binary => DbType.Binary,
                _ => default
            };

            return output;
        }

        private static void zDisplayData(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                    Debug.Print("{0} = {1}", col.ColumnName, row[col]);
                Debug.Print("============================");
            }
        }

        private static int zFindColumn(string name, List<xColumn> lst)
        {
            return lst.FindIndex(o => name.EqualsWithoutCase(o.name));
        }

        private static AdoxCatalog zOpenCatalog(OleDbConnection cnx, ref int erc)
        {
            AdoxCatalog cat = null;

            try
            {
                cat = new AdoxCatalog();
                cat.OpenDatabase(cnx.ConnectionString);
                erc = 0;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (cat is not null)
                {
                    cat.Dispose();
                    cat = null;
                }

                erc = ex.ErrorCode;
            }

            return cat;
        }

        private static List<xColumn> zReadColumns(OleDbConnection cnx, string tableName)
        {
            DataTable sch = cnx.GetSchema(OleDbMetaDataCollectionNames.Columns);
            List<xColumn> cols = new List<xColumn>();
            for (int i = 0; i < sch.Rows.Count; i++)
            {
                DataRow row = sch.Rows[i];
                if (ConvertShop.ReadString(row["TABLE_NAME"]) == tableName)
                    cols.Add(new xColumn(ConvertShop.ReadString(row["COLUMN_NAME"]), (OleDbType)row["DATA_TYPE"], row["CHARACTER_MAXIMUM_LENGTH"], ConvertShop.ReadInt(row["ORDINAL_POSITION"])));
            }

            sch.Dispose();
            return cols;
        }

        private static List<xTable> zReadTables(OleDbConnection cnx, string tbtyp)
        {
            DataTable sch = cnx.GetSchema(OleDbMetaDataCollectionNames.Tables);
            List<xTable> tbs = new List<xTable>();

            for (int i = 0; i < sch.Rows.Count; i++)
            {
                DataRow row = sch.Rows[i];
                if (ConvertShop.ReadString(row["TABLE_TYPE"]) == tbtyp)
                    tbs.Add(new xTable(ConvertShop.ReadString(row["TABLE_NAME"]), ConvertShop.ReadDateTime(row["DATE_CREATED"])));
            }

            sch.Dispose();
            return tbs;
        }

        private static string zSQLCount(string tbnam)
        {
            return "SELECT Count(*) AS nb FROM " + tbnam.SurroundBracket() + "";
        }

        private static string zSQLDeleteRow(string tbnam, string idnam, int idval)
        {
            return "DELETE FROM " + tbnam.SurroundBracket() + " WHERE " + idnam.SurroundBracket() + " = " + idval.ToString();
        }

        private static string zSQLInsertID(string tbnam, string idnam)
        {
            return "Insert Into " + tbnam.SurroundBracket() + " (" + idnam.SurroundBracket() + ") Values (?)";
        }

        private static string zSQLPassword(string oldPassword, string newPassword)
        {
            if (oldPassword != "")
                oldPassword = oldPassword.SurroundBracket();
            else
                oldPassword = "NULL";

            if (newPassword != "")
                newPassword = newPassword.SurroundBracket();
            else
                newPassword = "NULL";

            return "ALTER DATABASE PASSWORD " + newPassword + " " + oldPassword;
        }

        private static string zSQLSelectIDs(string tbnam, string idnam)
        {
            return "Select " + idnam.SurroundBracket() + " From " + tbnam.SurroundBracket();
        }

        private static string zSQLSelectRow(string tbnam, string idnam, int idval)
        {
            string qry = "SELECT * FROM " + tbnam.SurroundBracket() + "";
            qry = qry + " WHERE " + tbnam.SurroundBracket() + "." + idnam.SurroundBracket() + " = " + idval.ToString();

            return qry;
        }

        private static string zSQLUpdateRow(string tbnam, string idnam, int idval, List<string> flds)
        {
            string qry = "UPDATE " + tbnam.SurroundBracket() + " SET ";

            for (int i = 0; i < flds.Count; i++)
            {
                qry = qry + flds[i].SurroundBracket() + " = ?";
                if (i < flds.Count - 1)
                    qry += ", ";
            }

            qry = qry + " WHERE " + idnam.SurroundBracket() + " = " + idval;

            return qry;
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
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);
            cat.AddField(tableName, fieldName, zAdoxDataType(fieldType), isAutoIncrement);
            zCloseCatalog(cat);

            return true;
        }

        bool IBDDCreator.CreateBase(string baseName)
        {
            bool ok;

            string cnx = zConnectionString(KACEProvider, baseName, "", false);

            try
            {
                AdoxCatalog cat = new AdoxCatalog();
                cat.CreateDatabase(cnx);
                cat.CloseDatabase();
                cat.Dispose();
                cat = null;

                _errCode = 0;
                ok = true;
            }
            catch (COMException ex)
            {
                _errCode = ex.ErrorCode;
                ok = false;
            }

            if (ok)
            {
                _bdd = new OleDbConnection(cnx);
                _bdd.Open();
            }

            return ok;
        }

        bool IBDDCreator.CreateBase(string baseName, string sql)
        {
            bool ok;

            string cnx = zConnectionString(KACEProvider, baseName, "", false);

            try
            {
                AdoxCatalog cat = new AdoxCatalog();
                cat.CreateDatabase(cnx);
                cat.CloseDatabase();
                cat.Dispose();
                cat = null;

                _errCode = 0;
                ok = true;
            }
            catch (COMException ex)
            {
                _errCode = ex.ErrorCode;
                ok = false;
            }

            if (ok)
            {
                _bdd = new OleDbConnection(cnx);
                _bdd.Open();
                OleDbCommand cmd = new OleDbCommand(sql, _bdd);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            return ok;
        }

        bool IBDDCreator.CreateTable(string tableName, string indexName, string indexField)
        {
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);

            cat.AddTable(tableName);
            cat.AddField(tableName, indexField, DataTypeEnum.adInteger, true);
            cat.AddIndex(tableName, indexField, indexName);

            zCloseCatalog(cat);

            return true;
        }

        void IBDDCreator.KillField(string tableName, string fieldName)
        {
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);
            cat.DeleteField(tableName, fieldName);
            zCloseCatalog(cat);
        }

        void IBDDCreator.KillTable(string tableName)
        {
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);
            cat.DeleteTable(tableName);
            zCloseCatalog(cat);
        }

        void IBDDCreator.RenameField(string tableName, string oldFieldName, string newFieldName)
        {
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);
            cat.RenameField(tableName, oldFieldName, newFieldName);
            zCloseCatalog(cat);
        }

        void IBDDCreator.RenameTable(string oldName, string newName)
        {
            AdoxCatalog cat = zOpenCatalog(_bdd, ref _errCode);
            cat.RenameTable(oldName, newName);
            zCloseCatalog(cat);
        }

        void IBDDCreator.SetUserVersion(int version)
        {
            throw new NotImplementedException(); // TODO
        }


        // ####################################
        // As IBDDEngine
        // ####################################

        int IBDDEngine.AddRecord(string tableName, string indexName)
        {
            // -- pas moyen de faire marcher la colonne autoincr en utilisant une commande.ExecuteNonQuery 
            // ==> souk avec adapter

            string selqry = zSQLSelectIDs(tableName, indexName);
            string insqry = zSQLInsertID(tableName, indexName);

            OleDbDataAdapter adap = new OleDbDataAdapter(selqry, _bdd);
            adap.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            OleDbCommand cmd = new OleDbCommand(insqry, _bdd);
            cmd.Parameters.Add(indexName, OleDbType.Integer, 0, indexName);
            adap.InsertCommand = cmd;

            DataTable dtb = new DataTable();
            dtb.Locale = System.Globalization.CultureInfo.InvariantCulture;

            adap.Fill(dtb);
            dtb.Columns[indexName].AutoIncrementSeed = 1L;
            dtb.Columns[indexName].AutoIncrementStep = 1L;
            DataRow drw = dtb.NewRow();
            dtb.Rows.Add(drw);

            adap.Update(dtb.Select(null, null, DataViewRowState.Added));
            dtb.Dispose();

            // -- avant le 12.12.16 : 
            // cmd.CommandText = "SELECT @@IDENTITY"
            // -- depuis le 12.12.16 :
            cmd.CommandText = "SELECT SCOPE_IDENTITY";
            int id = ConvertShop.ReadInt(cmd.ExecuteScalar());

            cmd.Dispose();
            adap.Dispose();

            return id;
        }

        string IBDDEngine.BaseProvider() => _bdd.Provider;

        string IBDDEngine.BaseVersion() => _bdd.ServerVersion;

        void IBDDEngine.CloseBase()
        {
            _bdd.Close();
            _bdd.Dispose();
            _bdd = null;
        }

        void IBDDEngine.Flush()
        {
            _bdd.Close();
            _bdd.Open();
        }

        List<int> IBDDEngine.GetRecordIds(string tableOrSql, List<object> parameters)
        {
            OleDbCommand cmd = new OleDbCommand(tableOrSql, _bdd);
            if (parameters is not null)
                for (int i = 0; i < parameters.Count; i++)
                    cmd.Parameters.Add(new OleDbParameter("?", parameters[i]));

            List<int> recids = new List<int>();
            OleDbDataReader dRdr = cmd.ExecuteReader();
            while (dRdr.Read())
                recids.Add(ConvertShop.ReadInt(dRdr[0]));
            dRdr.Close();
            cmd.Dispose();

            return recids;
        }

        bool IBDDEngine.KillRecord(string tableName, string indexName, int recordId)
        {
            string delqry = zSQLDeleteRow(tableName, indexName, recordId);

            OleDbCommand cmd = new OleDbCommand(delqry, _bdd);
            int nb = cmd.ExecuteNonQuery();
            cmd.Dispose();

            return nb == 1;
        }

        int IBDDEngine.LastError() => _errCode;    // -- très bidon, voir en VB.Net, des codes étaient référencés en commentaire mais inutilisés...

        bool IBDDEngine.NewPassword(string oldPassword, string newPassword)
        {
            bool ok;

            string qry = zSQLPassword(oldPassword, newPassword);
            OleDbCommand cmd = new OleDbCommand(qry, _bdd);

            try
            {
                cmd.ExecuteNonQuery();
                _errCode = 0;
                ok = true;
            }
            catch (OleDbException ex)
            {
                _errCode = ConvertShop.ReadInt(ex.Errors[0].SQLState);
                ok = false;
            }

            cmd.Dispose();

            return ok;
        }

        bool IBDDEngine.OpenBase(string baseName, string password, bool isExclusive)
        {
            bool ok;

            try
            {
                _bdd = new OleDbConnection(zConnectionString(KACEProvider, baseName, password, isExclusive));
                _bdd.Open();    // -- rame (~ 50 ms)
                _errCode = 0;
                ok = true;
            }
            catch (OleDbException dbex)
            {
                _errCode = ConvertShop.ReadInt(dbex.Errors[0].SQLState);
                ok = false;
            }
            catch (InvalidOperationException)
            {
                _errCode = (int)AceError.ProviderNotInstalled;
                ok = false;
            }
            catch
            {
                _errCode = -1;
                ok = false;
            }

            return ok;
        }

        (List<string> fieldsName, List<DbType> fieldsType) IBDDEngine.ReadFields(string tableName, string indexName)
        {
            List<xColumn> cols = zReadColumns(_bdd, tableName);
            cols.Sort(zCompareCols);

            List<string> fieldsName = new List<string>();
            List<DbType> fieldsType = new List<DbType>();

            for (int i = 0; i < cols.Count; i++)
            {
                if (!cols[i].name.EqualsWithoutCase(indexName))
                {
                    fieldsName.Add(cols[i].name);
                    fieldsType.Add(zDBType(cols[i].typ));
                }
            }

            return (fieldsName, fieldsType);
        }

        List<object> IBDDEngine.ReadRecord(string tableName, string indexName, int recordId, List<string> fieldsName)
        {
            // -- dans le temps yavait un systus avec index, j'ai pas trouvé l'équivalent.

            string sql = zSQLSelectRow(tableName, indexName, recordId);

            OleDbCommand cmd = new OleDbCommand(sql, _bdd);
            OleDbDataReader drdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            List<object> vals = null;

            if (drdr.HasRows)
            {
                drdr.Read();
                int nb = fieldsName.Count;

                vals = new List<object>();
                for (int i = 0; i < nb; i++)
                    vals.Add(drdr[fieldsName[i]]);
            }

            drdr.Close();
            cmd.Dispose();

            return vals;
        }

        List<string> IBDDEngine.ReadTables()
        {
            List<xTable> tbs = zReadTables(_bdd, "TABLE");
            tbs.Sort(zCompareTabs);

            List<string> tbn = new List<string>();
            for (int i = 0; i < tbs.Count; i++)
                tbn.Add(tbs[i].nam);

            return tbn;
        }

        int IBDDEngine.RecordsCount(string tableName)
        {
            // -- tbnam peut être une requête (sans paramètre)

            string sqltext = zSQLCount(tableName);
            OleDbCommand command = new OleDbCommand(sqltext, _bdd);

            int nb = ConvertShop.ReadInt(command.ExecuteScalar());

            command.Dispose();

            return nb;
        }

        int IBDDEngine.UserVersion()
        {
            throw new NotImplementedException(); // TODO
        }

        void IBDDEngine.WriteRecord(string tableName, string indexName, int recordId, List<string> fieldsName, List<object> fieldsValue)
        {
            List<xColumn> cols = zReadColumns(_bdd, tableName);
            string updqry = zSQLUpdateRow(tableName, indexName, recordId, fieldsName);
            OleDbCommand cmd = new OleDbCommand(updqry, _bdd);

            for (int i = 0; i < fieldsName.Count; i++)
            {
                int no = zFindColumn(fieldsName[i], cols);
                OleDbParameter prm = new OleDbParameter("?", cols[no].typ);     // -- absolument préciser le type, la conversion auto ne marche pas.
                prm.Value = fieldsValue[i];
                cmd.Parameters.Add(prm);
            }

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


    }
}