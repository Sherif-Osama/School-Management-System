using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using School.DAL.Interfaces;
using School.DTO.LogDTOs;
using System.Threading.Channels;
namespace School.BLL.Logging
{
    public class LogBackgroundService : BackgroundService
    {
        private readonly Channel<LogEntryDTO> _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        public LogBackgroundService(Channel<LogEntryDTO> channel, IServiceScopeFactory scopeFactory)
        {
            _channel = channel;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (LogEntryDTO entry in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                ILogData logData = scope.ServiceProvider.GetRequiredService<ILogData>();

                try
                {
                    await logData.AddLogAsync(entry);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to persist log entry: {ex.Message}");
                }
            }
        }
    }
}
