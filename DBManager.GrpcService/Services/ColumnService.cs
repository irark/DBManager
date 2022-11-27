using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using DBManager;
using DBManager.GrpcService;

namespace DBManager.GrpcService.Services
{
    public class ColumnService : Columns.ColumnsBase
    {
        private readonly ILogger<ColumnService> _logger;
        private readonly DBHelper _dBHelper = DBHelper.Instance;
        public ColumnService(ILogger<ColumnService> logger)
        {
            _logger = logger;
        }

        public override Task<Column> CreateColumn(CreateColumnRequest model, ServerCallContext context)
        {
            return Task.FromResult(
                _dBHelper.CreateColumn(model.TableName, model.Column)
                );
        }

        public override Task<BoolValue> RenameColumn(RenameColumnRequest model, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.RenameColumn(model.TableName, model.OldName, model.NewName) };
            return Task.FromResult(res);
        }

        public override Task<BoolValue> DeleteColumn(DeleteColumnRequest model, ServerCallContext context)
        {
            BoolValue res = new BoolValue() { Value = _dBHelper.DeleteColumn(model.TableName, model.ColumnIndex) };
            return Task.FromResult(res);
        }
    }
}
