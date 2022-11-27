using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Newtonsoft.Json;

namespace DBManager.Mobile.Services
{
    public class DatabaseService : GrpcService
    {
        protected readonly Databases.DatabasesClient _client;
        public DatabaseService()
        {
            _client = new Databases.DatabasesClient(_chanel);
        }

        public async Task<List<Database>> GetAllDatabases()
        {
            return (await _client.GetAllDatabasesAsync(new Empty())).Databases.ToList();
        }

        public async Task<Database> OpenDatabase(string path)
        {
            return await _client.OpenDatabaseAsync(new StringValue() { Value = path });
        }


        public async Task<bool> RenameDatabase(string newName)
        {
            return (await _client.RenameDatabaseAsync(new StringValue() { Value = newName})).Value;
        }
        public async Task<Database> CreateDatabase(Database database)
        {
            return await _client.CreateDatabaseAsync(database);
        }

        public async Task<bool> SaveDatabase()
        {
            return (await _client.SaveDatabaseAsync(new Empty())).Value;
        }

        public async Task<bool> DeleteDataBase()
        {
            return (await _client.DeleteDataBaseAsync(new Empty())).Value;
        }

        public async Task<bool> DeleteDataBaseByPath(string databasePath)
        {
            return (await _client.DeleteDataBaseByPathAsync(new StringValue() { Value = databasePath})).Value;
        }
    }
}