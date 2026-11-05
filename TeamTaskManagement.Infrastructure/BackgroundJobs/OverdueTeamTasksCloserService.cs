using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Repositories;

namespace TeamTaskManagement.Infrastructure.BackgroundJobs;

public sealed class OverdueTeamTasksCloserService : BackgroundService {

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueTeamTasksCloserService> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);
    private const int BatchSize = 50;

    public OverdueTeamTasksCloserService
        (IServiceScopeFactory scopeFactory,
         ILogger<OverdueTeamTasksCloserService> logger) {

        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {

        // کمی تأخیر بعد از استارت اپ
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested) {
            try {
                await CloseOverdueAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException) {
                _logger.LogError(ex, "Error while closing overdue tasks");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CloseOverdueAsync(CancellationToken ct) {

        using var scope = _scopeFactory.CreateScope();

        var tasks = scope.ServiceProvider.GetRequiredService<ITeamTaskRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var overdue = await tasks.GetOpenTasksPastDeadlineAsync(BatchSize, ct);

        if (overdue.Count == 0)
            return;

        var closed = 0;

        foreach (var task in overdue) {
            var result = task.CloseAsDeadlineFailure();
            if (result.IsSuccess)
                closed++;
        }

        if (closed > 0) {
            await uow.SaveChangesAsync(ct);
            _logger.LogInformation("Closed {Count} overdue tasks as deadline failure", closed);
        }
    }
}