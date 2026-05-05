using System.Data;

using Microvision.Types;

// ***************************************************************************************************
// Interfaces issue de iBDDEngine, à implémenter par les boulons 
// réalisés avec les différentes technologies. 
// ***************************************************************************************************
// Un peu d'histoire :
// - DAO est apparu avec VB3 (1992), en même temps que ODBC
// - est remplacé par RDO (VB4, 1995), basé sur ODBC
// - en 1996 apparaît OLE DB, alternative et parallèle à ODBC...
// - ...puis ADO, ActiveX basé sur OLE DB (1996 aussi)
// - ADO prend de l'envergure (ADO Extensions et ADO MultiDimensional) pendant que OLEDB masque ODBC :
// 
// ADO (+ ADOX + ADOMD)
// !
// OLE DB          -->         ODBC
// !                         !
// -------------------------------------------------
// !               !               !               !
// Différentes bases de données avec leur drivers OLEDB et/ou ODBC
// 
// - et devient ADO.Net en 2002. Sont inclus des "providers" pour OLE DB, ODBC, SQL Server et (c'est 
// nouveau) pour des sources xml. 
// On décide en 2026 de supprimer ADO, parce que c'est obsolete depuis bien longtemps et qu'on a les
// bases SQLite pour faire le job.
// ***************************************************************************************************

namespace Microvision.DataBase
{
    public interface IBDDCreator
    {
        // ***************************************************************************************************
        // 21.06.11 : (iBDDEngine) libs 1.8
        // 21.01.14 : (iBDDCreator) libs 2.0, extension de iBDDEngine pour bdds qu'on peut créer et dont
        //            on peut modifier la structure. Typiquement, les bonnes vieilles bases.
        // 12.05.17 : (libs 2.1)
        // 16.06.17 : Suppression de CompactBase et RepairBase; inutilisés de toutes façons
        // 21.11.19 : (libs 2.2)
        // 24.11.20 : Ajout de CreateBase avec un script
        // 13.04.22 : (libs 3.0)
        // 23.03.23 : Ajout de la version utilisateur
        // ***************************************************************************************************

        bool AddField(string tableName, string fieldName, DbType fieldType, bool isAutoIncrement = false);
        bool CreateBase(string fileName);
        bool CreateBase(string fileName, string sqlScript);
        bool CreateTable(string tableName, string? indexName, string indexField);
        void KillField(string tableName, string fieldName);
        void KillTable(string tableName);
        void RenameField(string tableName, string fieldName, string newFieldName);
        void RenameTable(string tableName, string newTableName);
        void SetUserVersion(int version);
    }

    public interface IBDDEngine : IPreservable
    {
        // ***************************************************************************************************
        // 21.06.11 : libs 1.8
        // 21.01.14 : libs 2.0, restriction aux méthodes pour bdds complètement utilisables, mais qu'on ne
        //            peut pas créer et dont on ne peut modifier la structure. Typiquement, les bases modernes.
        //            Nom du champ "ID" passé en argument à ReadFields, ReadRecord et WriteRecord, mais nom
        //            de l'index supprimé de ReadRecord et WriteRecord parce que je ne sais pas comment
        //            l'exploiter.
        // 19.09.16 : ajout de surcharges à listes
        // 12.05.17 : (libs 2.1)
        // 10.11.20 : Ajout de Flush
        // 13.04.22 : (libs 3.0)
        // 23.03.23 : Ajout de la version utilisateur
        // ***************************************************************************************************

        int AddRecord(string tableName, string indexName);
        string BaseProvider();
        string BaseVersion();
        void CloseBase();
        void Flush();
        List<int> GetRecordIds(string tabOrSql, List<object>? parameters);
        bool KillRecord(string tableName, string idName, int id);
        int LastError();
        bool NewPassword(string oldPasswork, string newPassword);
        bool OpenBase(string baseName, string password, bool isExclusive = false);
        (List<string> fieldsName, List<DbType> fieldsType) ReadFields(string tableName, string indexName);
        List<object> ReadRecord(string tableName, string indexName, int id, List<string> fields);
        List<string> ReadTables();
        int RecordsCount(string tableName);
        int UserVersion();
        void WriteRecord(string tableName, string indexName, int id, List<string> fields, List<object> values);
    }
}