using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DBManager.Mobile.Services
{
    public class TableService : GrpcService
    {
        protected readonly Tables.TablesClient _client;
        public TableService()
        {
            _client = new Tables.TablesClient(_chanel);
        }


        public async Task<Table> CreateTable(string tableName)
        {
            return await _client.CreateTableAsync(new StringValue() { Value = tableName});
        }

        public async Task<bool> DeleteTable(string tableName)
        {
            return (await _client.DeleteTableAsync(new StringValue() { Value = tableName})).Value;
        }

        public async Task<bool> RenameTable(string oldTableName, string newTableName)
        {
            return (await _client.RenameTableAsync(new RenameTableRequest() { OldTableName = oldTableName, NewTableName = newTableName })).Value;
        }

        public async Task<Table> Difference(string firstTableName, string secondTableName)
        {
            return await _client.DifferenceAsync(new DifferenceRequest() { FirstTableName = firstTableName, SecondTableName = secondTableName });
        }

        public async Task<Table> GetTableByName(string tableName)
        {
            return await _client.GetTableByNameAsync(new StringValue() { Value = tableName });
        }
    }
}