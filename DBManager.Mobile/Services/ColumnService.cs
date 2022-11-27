using Grpc.Net.Client;

namespace DBManager.Mobile.Services
{
    public class ColumnService : GrpcService
    {
        protected readonly Columns.ColumnsClient _client;
        public ColumnService() 
        {
            _client = new Columns.ColumnsClient(_chanel);
        }

        public async Task<Column> CreateColumn(string tableName, Column column)
        {
            return await _client.CreateColumnAsync(new CreateColumnRequest() { TableName = tableName, Column = column});
        }

        public async Task<bool> RenameColumn(string tableName, string oldName, string newName)
        {
            return (await _client.RenameColumnAsync(new RenameColumnRequest() { TableName = tableName, OldName = oldName, NewName = newName })).Value;
        }
        public async Task<bool> DeleteColumn(string tableName, int columnIndex)
        {
            return (await _client.DeleteColumnAsync(new DeleteColumnRequest() { TableName = tableName, ColumnIndex = columnIndex})).Value;
        }
    }
}