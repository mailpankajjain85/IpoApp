using System;
using System.Collections.Generic;
using System.Linq;
using IpoApp.Data.Migrations;
using Npgsql;

namespace IpoApp.Data
{
    public class MigrationRunner
    {
        private readonly string _connectionString;

        public MigrationRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RunMigrations()
        {
            var migrations = GetMigrations();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Ensure the AppliedMigrations table exists
                new Migration0000_CreateMigrationTable().Execute(connection);

                foreach (var migration in migrations)
                {
                    if (!IsMigrationApplied(connection, migration.ScriptName))
                    {
                        migration.Execute(connection);
                        MarkMigrationAsApplied(connection, migration.ScriptName);
                    }
                }
            }
        }

        private List<IDatabaseMigration> GetMigrations()
        {
            // Return all migrations in the correct order
            return new List<IDatabaseMigration>
            {
                new Migration0000_CreateMigrationTable(),
                new Migration0001_CreateTablesForTenantOrg(),
                new Migration0002_CreateUser(),
                new Migration0003_CreateIpoMaster(),
                new Migration0004_CreateTransactionTable(),
            };
        }

        private bool IsMigrationApplied(NpgsqlConnection connection, string scriptName)
        {
            using (var command = new NpgsqlCommand(
                "SELECT 1 FROM AppliedMigrations WHERE MigrationId = @MigrationId;", connection))
            {
                command.Parameters.AddWithValue("@MigrationId", scriptName);
                return command.ExecuteScalar() != null;
            }
        }

        private void MarkMigrationAsApplied(NpgsqlConnection connection, string scriptName)
        {
            using (var command = new NpgsqlCommand(
                "INSERT INTO AppliedMigrations (MigrationId) VALUES (@MigrationId);", connection))
            {
                command.Parameters.AddWithValue("@MigrationId", scriptName);
                command.ExecuteNonQuery();
            }
        }
    }
}