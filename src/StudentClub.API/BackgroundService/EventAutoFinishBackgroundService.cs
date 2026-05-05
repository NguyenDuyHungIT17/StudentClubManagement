using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentClub.Application.IServices;

namespace StudentClub.API.BackgroundServices
{
    public class EventAutoFinishBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventAutoFinishBackgroundService> _logger;

        public EventAutoFinishBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<EventAutoFinishBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Event auto-finish background service started");

            await RunAutoFinishAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                    await RunAutoFinishAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi vòng lặp background auto-finish");
                }
            }

            _logger.LogInformation("Event auto-finish background service stopped");
        }

        private async Task RunAutoFinishAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

                var result = await eventService.AutoFinishExpiredEventsAsync();

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Auto-finish events success. UpdatedCount: {Count}", result.Data);
                }
                else
                {
                    _logger.LogWarning("Auto-finish events failed. Message: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy auto-finish events");
            }
        }
    }
}
