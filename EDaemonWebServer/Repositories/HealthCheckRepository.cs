using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using EDaemonWebServer.Repositories.Interfaces;

namespace EDaemonWebServer.Repositories
{
    public class HealthCheckRepository : BaseRepository, IHealthCheckRepository
    {
        public HealthCheckRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<(bool ok, string message, object? scalar, string? error)> TestDbConnectionAsync()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                var result = await command.ExecuteScalarAsync();

                return (true, "Database connection successful.", result, null);
            }
            catch (Exception ex)
            {
                return (false, "Failed to connect to database.", null, ex.Message);
            }
        }
    }
}
