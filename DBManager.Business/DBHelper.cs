using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManager.Entities.Enums;
using DBManager.Entities.Models;

namespace DBManager.Business
{
    public class DBHelper
    {
        private Database _database;

        public Database Database
        {
            get => _database;
            set => _database = value;
        }

        private static DBHelper _instance;

        #region Separators

        private const char _tablesSeparator = '$';
        private const char _columnsSeparator = '%';

        #endregion

        public static DBHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBHelper();
                }

                return _instance;
            }
        }

        private const string databasesFile = "databases_info.txt";

        private DBHelper()
        {
        }

        #region Database

        public List<Database> GetAllDatabases()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, databasesFile);
                if (!File.Exists(path))
                {
                    File.Create(path);
                    return null;
                }

                var streamReader = new StreamReader(path);
                string file = streamReader.ReadToEnd();
                var databasesInfo = file.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                var dataBases = new List<Database>();
                foreach (var item in databasesInfo)
                {
                    var info = item.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    dataBases.Add(new Database(info[0], info[1]));
                }

                streamReader.Close();
                return dataBases;
            }
            catch
            {
                return null;
            }
        }

        public Database OpenDatabase(string path)
        {
            try
            {
                var streamReader = new StreamReader(path);
                _database = new Database(streamReader.ReadLine(), path);
                string file = streamReader.ReadToEnd();
                ReadTables(file.Split(_tablesSeparator, StringSplitOptions.RemoveEmptyEntries));
                streamReader.Close();
                var databases = GetAllDatabases();
                if (!databases.Exists(i => i.Path == _database.Path))
                    WriteDatabase();
                return _database;
            }
            catch
            {
                return null;
            }
        }

        public Database CreateDatabase(Database database)
        {
            try
            {
                _database = new Database(database);
                var databases = GetAllDatabases() ?? new List<Database>();
                if (!databases.Exists(i => i.Path == _database.Path))
                    WriteDatabase();
                SaveDatabase();
                return _database;
            }
            catch
            {
                return null;
            }
        }

        public bool RenameDatabase(string newName)
        {
            try
            {
                _database.Name = newName;
                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        private void WriteDatabase()
        {
            var path = Path.Combine(AppContext.BaseDirectory, databasesFile);
            var file = new FileStream(path, FileMode.Append);
            var streamWriter = new StreamWriter(file);
            streamWriter.WriteLine($"{_database.Name}\t{_database.Path}");
            streamWriter.Close();
        }

        private void DeleteDatabaseByPath(string databasePath)
        {
            var path = Path.Combine(AppContext.BaseDirectory, databasesFile);
            File.WriteAllLines(path,
                File.ReadLines(path).Where(i => !i.Contains(databasePath)).ToList());
        }

        public bool SaveDatabase()
        {
            try
            {
                var file = new FileStream(_database.Path, FileMode.Create);
                var streamWriter = new StreamWriter(file);
                streamWriter.WriteLine(_database.Name);
                WriteTables(streamWriter);
                streamWriter.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteDataBase()
        {
            try
            {
                if (!string.IsNullOrEmpty(_database.Path))
                {
                    File.Delete(_database.Path);
                    DeleteDatabaseByPath(_database.Path);
                }

                _database = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDataBaseByPath(string databasePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(databasePath))
                {
                    DeleteDatabaseByPath(databasePath);
                    File.Delete(databasePath);
                }

                _database = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Table

        public Table CreateTable(string tableName)
        {
            try
            {
                if (_database.Tables.Select(i => i.Name).Contains(tableName))
                {
                    return null;
                }

                var newTable = new Table(tableName);
                _database.Tables.Add(newTable);
                SaveDatabase();
                return newTable;
            }
            catch
            {
                return null;
            }
        }

        public bool RenameTable(string oldName, string newName)
        {
            try
            {
                var table = GetTableByName(oldName);
                if (table == null)
                    return false;
                table.Name = newName;
                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTable(string tableName)
        {
            try
            {
                _database.Tables.RemoveAll(i => i.Name == tableName);
                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        public Table GetTableByName(string tableName) => _database?.Tables?.FirstOrDefault(i => i.Name == tableName);
        public List<Table> GetAllTables() => _database?.Tables;

        private void ReadTables(string[] tables)
        {
            foreach (var table in tables)
            {
                var tableData = table.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!tableData.Any())
                    continue;
                var newTable = new Table(tableData[0]);
                _database.Tables.Add(newTable);
                if (tableData.Count > 1)
                {
                    ReadColumns(tableData[1], newTable);
                }

                if (tableData.Count <= 2) continue;
                tableData.RemoveAt(0);
                tableData.RemoveAt(0);
                ReadRows(tableData, newTable);
            }
        }

        private void WriteTables(StreamWriter streamWriter)
        {
            foreach (var table in _database.Tables)
            {
                streamWriter.WriteLine(table.Name);
                WriteColumns(streamWriter, table);
                WriteRows(streamWriter, table);
                streamWriter.WriteLine(_tablesSeparator);
            }
        }

        #endregion

        #region Columns

        public Column GetColumnByIndex(string tableName, int columnIndex)
        {
            return GetTableByName(tableName)?.Columns.ElementAtOrDefault(columnIndex);
        }

        public Column CreateColumn(string tableName, Column column)
        {
            try
            {
                var table = GetTableByName(tableName);
                if (table.Columns.Select(i => i.Name).Contains(column.Name))
                {
                    return null;
                }

                table.Columns.Add(column);
                foreach (var row in table.Rows)
                {
                    row.Values.Add("");
                }

                SaveDatabase();
                return column;
            }
            catch
            {
                return null;
            }
        }

        public bool RenameColumn(string tableName, string oldName, string newName)
        {
            try
            {
                var table = GetTableByName(tableName);
                var column = table.Columns.FirstOrDefault(i => i.Name == oldName);
                if (column == null)
                    return false;
                if (table.Columns.Select(i => i.Name).Contains(newName))
                    return false;
                column.Name = newName;
                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteColumn(string tableName, int column)
        {
            try
            {
                var table = GetTableByName(tableName);
                table.Columns.RemoveAt(column);
                foreach (var row in table.Rows)
                {
                    row.Values.RemoveAt(column);
                }

                if (table.Columns.Count == 0)
                {
                    table.Rows.Clear();
                }

                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        private void ReadColumns(string data, Table table)
        {
            var columns = data.Split(_columnsSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var column in columns)
            {
                var columnData = column.Split('\t', StringSplitOptions.RemoveEmptyEntries).ToList();
                Enum.TryParse(columnData[1], out ColumnType columnType);
                var availableValues = new List<string>();
                if (columnData.Count > 2)
                    availableValues = columnData[2]?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                table.Columns.Add(new Column(columnData[0], columnType, availableValues));
            }
        }

        private void WriteColumns(StreamWriter streamWriter, Table table)
        {
            foreach (var column in table.Columns)
            {
                streamWriter.Write(column.Name + '\t' + column.Type + '\t' + string.Join(',', column.AvailableValues) +
                                   _columnsSeparator);
            }

            streamWriter.WriteLine();
        }

        #endregion

        #region Rows

        public Row CreateRow(string tableName)
        {
            var table = GetTableByName(tableName);
            var newRow = new Row();
            foreach (var column in table.Columns)
            {
                newRow.Values.Add("");
            }

            table.Rows.Add(newRow);
            SaveDatabase();
            return newRow;
        }

        public Row GetRowByIndex(string tableName, int rowIndex)
        {
            return GetTableByName(tableName)?.Rows.ElementAtOrDefault(rowIndex);
        }

        public bool DeleteRow(string tableName, int row)
        {
            try
            {
                var table = GetTableByName(tableName);
                table.Rows.RemoveAt(row);
                return SaveDatabase();
            }
            catch
            {
                return false;
            }
        }

        private void ReadRows(List<string> rows, Table table)
        {
            foreach (var row in rows)
            {
                var rowData = row.Split('\t', StringSplitOptions.RemoveEmptyEntries).ToList();

                table.Rows.Add(new Row() {Values = rowData});
            }
        }

        private void WriteRows(StreamWriter streamWriter, Table table)
        {
            foreach (var row in table.Rows)
            {
                streamWriter.WriteLine(string.Join('\t', row.Values));
            }
        }

        #endregion

        public bool EditCell(string tableName, int column, int row, string newValue)
        {
            try
            {
                var table = GetTableByName(tableName);
                if (table.Columns[column].IsValid(newValue))
                {
                    table.Rows[row].Values[column] = newValue;

                    return SaveDatabase();
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public Table Difference(string firstTableName, string secondTableName)
        {
            try
            {
                var firstTable = GetTableByName(firstTableName);
                var secondTable = GetTableByName(secondTableName);
                var result = new Table("difference");
                foreach (var column in firstTable.Columns.Where(column =>
                             secondTable.Columns.FirstOrDefault(i => i.Name == column.Name && i.Type == column.Type) !=
                             null))
                {
                    result.Columns.Add(column);
                }

                if (!result.Columns.Any())
                    return null;

                var tempRows = new List<Row>();
                foreach (var row in secondTable.Rows)
                {
                    var newRow = new Row();
                    for (var i = 0; i < row.Values.Count; i++)
                    {
                        if (result.Columns.FirstOrDefault(c =>
                                c.Name == secondTable.Columns.ElementAtOrDefault(i)?.Name &&
                                c.Type == secondTable.Columns.ElementAtOrDefault(i)?.Type) != null)
                        {
                            newRow.Values.Add(row.Values.ElementAtOrDefault(i));
                        }
                    }

                    tempRows.Add(newRow);
                }

                foreach (var row in firstTable.Rows)
                {
                    var newRow = new Row();
                    for (var i = 0; i < row.Values.Count; i++)
                    {
                        if (result.Columns.Contains(firstTable.Columns.ElementAtOrDefault(i)))
                        {
                            newRow.Values.Add(row.Values.ElementAtOrDefault(i));
                        }
                    }

                    if (!tempRows.Any(r => r.Values.SequenceEqual(newRow.Values)))
                        result.Rows.Add(newRow);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}