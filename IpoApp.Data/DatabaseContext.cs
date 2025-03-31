using System.Data;
using Npgsql;

namespace IpoApp.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}