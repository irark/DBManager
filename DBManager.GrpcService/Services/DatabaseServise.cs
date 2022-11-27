using DBManager;
using DBManager.GrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.IO;

namespace DBManager.GrpcService.Services
{
    public class DatabaseServise : Databases.DatabasesBase
    {
        private readonly ILogger<DatabaseServise> _logger; 
        private readonly DBHelper _dBHelper = DBHelper.Instance;
        public DatabaseServise(ILogger<DatabaseServise> logger)
        {
            _logger = logger;
        }

        public override Task<GetAllDatabasesResponse> GetAllDatabases(Empty request, ServerCallContext context)
        {
            var res = new GetAllDatabasesResponse();
            res.Databases.AddRange(_dBHelper.GetAllDatabases() ?? new List<Database>());
            return Task.FromResult(res);
        }

        public override Task<Database> OpenDatabase(StringValue path, ServerCallContext context)
        {
            return Task.FromResult(_dBHelper.OpenDatabase(path.Value));
        }

        public override Task<BoolValue> RenameDatabase(StringValue newName, ServerCallContext context)
        {
            var res = new BoolValue();
            res.Value = _dBHelper.RenameDatabase(newName.Value);
            return Task.FromResult(res);
        }

        public override Task<Database> CreateDatabase(Database database, ServerCallContext context)
        {
            return Task.FromResult(_dBHelper.CreateDatabase(database));
        }

        public override Task<BoolValue> SaveDatabase(Empty database, ServerCallContext context)
        {
            var res = new BoolValue();
            res.Value = _dBHelper.SaveDatabase();
            return Task.FromResult(res);
        }

        public override Task<BoolValue> DeleteDataBase(Empty database, ServerCallContext context)
        {
            var res = new BoolValue();
            res.Value = _dBHelper.DeleteDataBase();
            return Task.FromResult(res);
        }

        public override Task<BoolValue> DeleteDataBaseByPath(StringValue databasePath, ServerCallContext context)
        {
            var res = new BoolValue();
            res.Value = _dBHelper.DeleteDataBaseByPath(databasePath.Value);
            return Task.FromResult(res);
        }
    }
}
