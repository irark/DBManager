using DBManager;
using DBManager.GrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
namespace DBManager.GrpcService.Services
{
    public class TableService : Tables.TablesBase
    {
        private readonly ILogger<TableService> _logger;
        private readonly DBHelper _dBHelper = DBHelper.Instance;
        public TableService(ILogger<TableService> logger)
        {
            _logger = logger;
        }

        public override Task<Table> CreateTable(StringValue tableName, ServerCallContext context)
        {
            return Task.FromResult(
                _dBHelper.CreateTable(tableName.Value)
                );
        }

        public override Task<BoolValue> DeleteTable(StringValue tableName, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.DeleteTable(tableName.Value) };
            return Task.FromResult(res);
        }

        public override Task<BoolValue> RenameTable(RenameTableRequest model, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.RenameTable(model.OldTableName, model.NewTableName) };
            return Task.FromResult(res);
        }

        public override Task<Table> Difference(DifferenceRequest model, ServerCallContext context)
        {
            return Task.FromResult(_dBHelper.Difference(model.FirstTableName, model.SecondTableName));
        }

        public override Task<Table> GetTableByName(StringValue tableName, ServerCallContext context)
        {
            return Task.FromResult(_dBHelper.GetTableByName(tableName.Value));
        }

    }
}
