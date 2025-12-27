using System.Threading.Tasks;

namespace EDaemonWebServer.Repositories.Interfaces
{
    public interface IHealthCheckRepository
    {
        /// <summary>
        /// Attempts to open a connection to the configured database and run a simple scalar query.
        /// Returns a tuple with success flag, message, scalar result (when successful) and error message (when failed).
        /// </summary>
        Task<(bool ok, string message, object? scalar, string? error)> TestDbConnectionAsync();
    }
}
