using System.Collections.Generic;

namespace DBManager.GrpcService
{ 
    public partial class Database
    {
        public Database (string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
