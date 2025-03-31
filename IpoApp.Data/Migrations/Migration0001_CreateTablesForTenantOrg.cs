using System.IO;
using System.Reflection;
using Npgsql;
using IpoApp.Data;

namespace IpoApp.Data.Migrations
{
    public class Migration0001_CreateTablesForTenantOrg : IDatabaseMigration
    {
        public string ScriptName => "0001_CreateTablesForTenantOrg.sql";

        public void Execute(NpgsqlConnection connection)
        {
            var sqlScript = ReadScript(ScriptName);
            using (var command = new NpgsqlCommand(sqlScript, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private string ReadScript(string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"IpoApp.Data.Migrations.Scripts.{scriptName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Script '{scriptName}' not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}