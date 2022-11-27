using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace DBManager.Mobile.Services
{
    public class RowService : GrpcService
    {
        protected readonly Rows.RowsClient _client;
        public RowService()
        {
            _client = new Rows.RowsClient(_chanel);
        }


        public async Task<Row> CreateRow(string tableName)
        {
            return await _client.CreateRowAsync(new StringValue() { Value = tableName});
        }

        public async Task<bool> DeleteRow(string tableName, int rowIndex)
        {
            return (await _client.DeleteRowAsync(new DeleteRowRequest() { TableName = tableName, RowIndex = rowIndex })).Value;
        }

        public async Task<bool> EditCell(string tableName, int columnIndex, int rowIndex, string newValue)
        {
            return (await _client.EditCellAsync(new EditCellRequest() { TableName = tableName, ColumnIndex = columnIndex, RowIndex = rowIndex, NewValue = newValue })).Value;
        }
    }
}