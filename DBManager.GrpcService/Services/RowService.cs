using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using DBManager;
using DBManager.GrpcService;

namespace DBManager.GrpcService.Services
{
    public class RowService : Rows.RowsBase
    {
        private readonly ILogger<RowService> _logger;
        private readonly DBHelper _dBHelper = DBHelper.Instance;
        public RowService(ILogger<RowService> logger)
        {
            _logger = logger;
        }

        public override Task<Row> CreateRow(StringValue tableName, ServerCallContext context)
        {
            return Task.FromResult(
                _dBHelper.CreateRow(tableName.Value)
                );
        }

        public override Task<BoolValue> DeleteRow(DeleteRowRequest model, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.DeleteRow(model.TableName, model.RowIndex) };
            return Task.FromResult(res);
        }

        public override Task<BoolValue> EditCell(EditCellRequest model, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.EditCell(model.TableName, model.ColumnIndex, model.RowIndex, model.NewValue) };
            return Task.FromResult(res);
        }

    }
}
