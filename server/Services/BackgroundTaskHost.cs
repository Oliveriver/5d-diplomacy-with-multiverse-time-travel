
namespace Services;

public class BackgroundQueueHostedService(BackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory, ILogger<BackgroundQueueHostedService> logger) : BackgroundService
{
    private readonly BackgroundTaskQueue _taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly ILogger<BackgroundQueueHostedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Dequeue and execute tasks until the application is stopped
        while (!stoppingToken.IsCancellationRequested)
        {
            // Get next task
            // This blocks until a task becomes available
            var task = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                // Run the task into the background threadpool. NOTE: this is not really a good idea to throw thousands of tiny at, nor more than one-per-core-ish for compute heavy.
                _ = Task.Run(async () => await task(_serviceScopeFactory, stoppingToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during execution of a background task");
            }
        }
    }
}