using System.Threading.Tasks;

namespace EDaemonWebServer.Services.Interfaces
{
    public interface IHealthCheckService
    {
        Task<(bool ok, string message, object? scalar, string? error)> TestDbConnectionAsync();
    }
}
