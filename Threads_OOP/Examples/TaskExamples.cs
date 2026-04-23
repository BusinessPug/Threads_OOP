namespace Threads_OOP.Examples;

internal static class TaskExamples
{
    public static async Task BasicTaskExample()
    {
        Console.WriteLine("\n=== Basic Task Example ===");
        Task task1 = Task.Run(async () => await PrintTask.PrintThread());
        Task task2 = Task.Run(async () => await PrintTask.PrintThread());

        await Task.WhenAll(task1, task2);
        Console.WriteLine("Printed from main using thread: " + Thread.CurrentThread.ManagedThreadId);
    }

    public static async Task TaskWithReturnValue()
    {
        Console.WriteLine("\n=== Task with Return Value ===");
        Task<int> task1 = Task.Run(() => CalculateSum(1, 50));
        Task<int> task2 = Task.Run(() => CalculateSum(51, 100));

        int[] results = await Task.WhenAll(task1, task2);
        Console.WriteLine($"Task 1 result: {results[0]}");
        Console.WriteLine($"Task 2 result: {results[1]}");
        Console.WriteLine($"Total sum: {results[0] + results[1]}");
    }

    public static async Task TaskCancellationExample()
    {
        Console.WriteLine("\n=== Task Cancellation Example ===");
        CancellationTokenSource cts = new();

        Task longRunningTask = Task.Run(async () =>
        {
            for (int i = 0; i < 10; i++)
            {
                cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"Working... {i}");
                await Task.Delay(500, cts.Token);
            }
        }, cts.Token);

        await Task.Delay(2000);
        Console.WriteLine("Cancelling task...");
        cts.Cancel();

        try
        {
            await longRunningTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task was cancelled successfully");
        }
    }

    public static async Task TaskContinuationExample()
    {
        Console.WriteLine("\n=== Task Continuation Example ===");
        Task<int> task = Task.Run(() =>
        {
            Console.WriteLine("First task executing...");
            Thread.Sleep(1000);
            return 42;
        });

        Task continuation = task.ContinueWith(t =>
        {
            Console.WriteLine($"Continuation executing with result: {t.Result}");
            return t.Result * 2;
        });

        await continuation;
    }

    public static async Task ParallelTaskProcessing()
    {
        Console.WriteLine("\n=== Parallel Task Processing ===");
        List<int> numbers = Enumerable.Range(1, 10).ToList();

        var tasks = numbers.Select(n => Task.Run(async () =>
        {
            await Task.Delay(n * 100);
            Console.WriteLine($"Processed number {n} on thread {Thread.CurrentThread.ManagedThreadId}");
            return n * n;
        }));

        int[] results = await Task.WhenAll(tasks);
        Console.WriteLine($"Sum of squares: {results.Sum()}");
    }

    public static async Task FireAndForgetExample()
    {
        Console.WriteLine("\n=== Fire and Forget Pattern ===");
        Task order1 = OrderTask.HandleOrder("Order1");
        Task order2 = OrderTask.HandleOrder("Order2");
        Task order3 = OrderTask.HandleOrder("Order3");
        Task order4 = OrderTask.HandleOrder("Order4");

        List<Task> orders = new() { order1, order2, order3, order4 };
        orders.ForEach(task => Task.Run(() => task));

        Console.WriteLine("Happening whilst orders are being processed");
        await Task.Delay(2000);
    }

    public static async Task TaskWhenAnyExample()
    {
        Console.WriteLine("\n=== Task.WhenAny Example ===");
        Task<string> task1 = Task.Run(async () =>
        {
            await Task.Delay(1000);
            return "Task 1 completed";
        });

        Task<string> task2 = Task.Run(async () =>
        {
            await Task.Delay(2000);
            return "Task 2 completed";
        });

        Task<string> task3 = Task.Run(async () =>
        {
            await Task.Delay(500);
            return "Task 3 completed";
        });

        Task<string> firstCompleted = await Task.WhenAny(task1, task2, task3);
        Console.WriteLine($"First to complete: {await firstCompleted}");
    }

    public static async Task AsyncLazyExample()
    {
        Console.WriteLine("\n=== Lazy Task Initialization ===");
        Lazy<Task<int>> lazyTask = new(async () =>
        {
            Console.WriteLine("Expensive operation starting...");
            await Task.Delay(1000);
            Console.WriteLine("Expensive operation completed");
            return 100;
        });

        Console.WriteLine("Lazy task created but not started");
        await Task.Delay(500);

        Console.WriteLine("Accessing lazy task value...");
        int result = await lazyTask.Value;
        Console.WriteLine($"Result: {result}");
    }

    public static async Task TaskStableCounterExample()
    {
        Console.WriteLine("\n=== Task-Based Counter (Thread-Safe) ===");
        Counter counter = new();
        Task inc1 = Task.Run(() => { for (int i = 0; i < 1000; i++) counter.IncrementInterlocked(); });
        Task inc2 = Task.Run(() => { for (int i = 0; i < 1000; i++) counter.IncrementInterlocked(); });
        await Task.WhenAll(inc1, inc2);
        Console.WriteLine($"Expected: 2000, Actual: {counter.GetValue()}");
    }

    private static int CalculateSum(int start, int end)
    {
        Console.WriteLine($"Calculating sum from {start} to {end} on thread {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1000);
        return Enumerable.Range(start, end - start + 1).Sum();
    }
}
