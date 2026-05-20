using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microvision.Types;

namespace Microvision.DataBase
{
    public class StdBDD : Citizen
    {
        // ***************************************************************************************************
        //            création : abstraction d'une bdd, avec moteur variable.
        // 21.06.11 : libs 1.8
        // 21.01.14 : libs 2.0
        // 05.07.16 : _tables à list(of), qq surcharges.
        // 19.09.16 : exploitation du iBDDEngine à listes
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 22.05.23 : Correction renommage table
        // ***************************************************************************************************

        private readonly IBDDEngine _engine;
        private string? _fileName;
        private string? _password;

        private string? _provider;
        private string? _version;
        private List<string>? _tables;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public StdBDD(IBDDEngine eng) : base()
        {
            _engine = eng.AddLife();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool CanManage => _engine is IBDDCreator;

        public string DataSourceName => ArgumentNullException.Check(_fileName);

        public IBDDEngine Engine => _engine;

        public int LastError => _engine.LastError();

        public string Password => ArgumentNullException.Check(_password);

        public string Provider => ArgumentNullException.Check(_provider);

        public int TablesCount => ArgumentNullException.Check(_tables).Count;

        public string Version => ArgumentNullException.Check(_version);


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void CloseBase()
        {
            _fileName = null;
            _password = null;

            _provider = null;
            _tables = null;
            _version = null;
        }

        public void CloseTable(StdBDDTable tb)
        {
            tb.Close();
        }

        public List<int>? GetRecordIds(string sql)
        {
            oThrowIfNotOpen();

            List<int>? ids = null;

            if (_engine.OpenBase(_fileName, _password))
            {
                ids = _engine.GetRecordIds(sql, null);
                _engine.CloseBase();
            }

            return ids;
        }

        public string GetTableName(int no)
        {
            oThrowIfNotOpen();

            return _tables[no];
        }

        public List<string> GetTableNames()
        {
            oThrowIfNotOpen();

            return [.. _tables];
        }

        public void KillTable(string tableName)
        {
            oThrowIfNotOpen();

            if (_engine is IBDDCreator eng && _engine.OpenBase(_fileName, _password))
            {
                eng.KillTable(tableName);
                _engine.CloseBase();
                _tables.Remove(tableName);
            }
        }

        public bool NewBase(string fileName)
        {
            bool ok = false;

            if (_engine is IBDDCreator eng && eng.CreateBase(fileName))
            {
                _tables = _engine.ReadTables();
                _fileName = fileName;
                _password = "";
                _provider = _engine.BaseProvider();
                _version = _engine.BaseVersion();
                _engine.CloseBase();

                ok = true;
            }

            return ok;
        }

        public bool NewPassword(string newPassword)
        {
            oThrowIfNotOpen();

            bool ok = false;

            if (_engine.OpenBase(_fileName, _password, true))
            {
                ok = _engine.NewPassword(_password, newPassword);
                if (ok) _password = newPassword;
                _engine.CloseBase();
            }

            return ok;
        }

        public StdBDDTable? NewTable(string tableName, StdBDDTable dst)
        {
            oThrowIfNotOpen();

            bool ok = false;

            if (_engine is IBDDCreator eng && _engine.OpenBase(_fileName, _password))
            {
                ok = eng.CreateTable(tableName, dst.IndexName, dst.IDFieldName);
                _engine.CloseBase();

                if (ok)
                {
                    dst.Open(_fileName, _password, tableName, _engine);
                    _tables.Add(tableName);
                }
            }

            return ok ? dst : null;
        }

        public bool OpenBase(string fileName, string password)
        {
            bool ok = false;

            if (_engine.OpenBase(fileName, password))
            {
                _tables = _engine.ReadTables();
                _fileName = fileName;
                _password = password;
                _provider = _engine.BaseProvider();
                _version = _engine.BaseVersion();

                _engine.CloseBase();
                ok = true;
            }

            return ok;
        }

        public StdBDDTable? OpenTable(string tableName, StdBDDTable dst)
        {
            oThrowIfNotOpen();

            StdBDDTable? output = null;

            if (_tables.IndexOf(tableName) >= 0)
            {
                dst.Open(_fileName, _password, tableName, _engine);
                output = dst;
            }

            return output;
        }

        public int RecordsCount(string tableName)
        {
            oThrowIfNotOpen();

            int nb = 0;

            if (_engine.OpenBase(_fileName, _password))
            {
                nb = _engine.RecordsCount(tableName);
                _engine.CloseBase();
            }

            return nb;
        }

        public void RenameTable(StdBDDTable tb, string newName)
        {
            oThrowIfNotOpen();

            string oldnam = tb.Name;

            if (newName != oldnam)
            {
                if (_engine is IBDDCreator eng && _engine.OpenBase(_fileName, _password))
                {
                    eng.RenameTable(oldnam, newName);
                    _engine.CloseBase();
                    tb.Name = newName;
                    _tables[_tables.IndexOf(oldnam)] = newName;
                }
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _engine.Dispose();

            base.oDispose(isExplicit);
        }

        [MemberNotNull(nameof(_fileName), nameof(_password), nameof(_tables))]
        protected void oThrowIfNotOpen()
        {
            ArgumentNullException.Check(_fileName);
            ArgumentNullException.Check(_password);
            ArgumentNullException.Check(_tables);
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