using System.Threading.Tasks;
using EDaemonWebServer.Repositories.Interfaces;
using EDaemonWebServer.Services.Interfaces;

namespace EDaemonWebServer.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _repository;

        public HealthCheckService(IHealthCheckRepository repository)
        {
            _repository = repository;
        }

        public Task<(bool ok, string message, object? scalar, string? error)> TestDbConnectionAsync()
        {
            return _repository.TestDbConnectionAsync();
        }
    }
}
