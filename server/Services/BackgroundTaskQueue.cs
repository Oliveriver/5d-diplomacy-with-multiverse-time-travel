
using System.Collections.Concurrent;

namespace Services;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class BackgroundTaskQueue
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1001 // Types that own disposable fields should be disposable

{
    private readonly ConcurrentQueue<Func<IServiceScopeFactory, CancellationToken, Task>> _items = new();

    // Holds the current count of tasks in the queue.
    private readonly SemaphoreSlim _signal = new(0);

    public void EnqueueTask(Func<IServiceScopeFactory, CancellationToken, Task> task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        _items.Enqueue(task);
        _signal.Release();
    }

    public async Task<Func<IServiceScopeFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        // Wait for task to become available
        await _signal.WaitAsync(cancellationToken);

        return _items.TryDequeue(out var task)
            ? task
            : throw new Exception("For some reason, somehow, workqueue became desynced from its semaphore lock");
    }
}