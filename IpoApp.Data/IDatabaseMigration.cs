using Npgsql;

namespace IpoApp.Data
{
 public interface IDatabaseMigration
    {
        void Execute(NpgsqlConnection connection);
        string ScriptName { get; }
    }
}