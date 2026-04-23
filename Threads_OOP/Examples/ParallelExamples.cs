using System.Threading.Tasks.Dataflow;

namespace Threads_OOP.Examples;

internal static class ParallelExamples
{
    public static void ParallelForExample()
    {
        Console.WriteLine("\n=== Parallel.For Example ===");
        Console.WriteLine("Processing items in parallel...");

        Parallel.For(0, 10, i =>
        {
            Console.WriteLine($"Processing item {i} on thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(500);
        });

        Console.WriteLine("All items processed");
    }

    public static void ParallelForEachExample()
    {
        Console.WriteLine("\n=== Parallel.ForEach Example ===");
        List<string> cities = new() { "New York", "London", "Tokyo", "Paris", "Sydney", "Mumbai", "Berlin", "Toronto" };

        Parallel.ForEach(cities, city =>
        {
            Console.WriteLine($"Processing {city} on thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(300);
        });

        Console.WriteLine("All cities processed");
    }

    public static void ParallelForWithOptions()
    {
        Console.WriteLine("\n=== Parallel.For with ParallelOptions ===");
        ParallelOptions options = new()
        {
            MaxDegreeOfParallelism = 3
        };

        Console.WriteLine($"Max parallel threads: {options.MaxDegreeOfParallelism}");

        Parallel.For(0, 10, options, i =>
        {
            Console.WriteLine($"Item {i} on thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(500);
        });
    }

    public static void ParallelForWithCancellation()
    {
        Console.WriteLine("\n=== Parallel.For with Cancellation ===");
        Console.WriteLine("Starting a long parallel loop; cancellation will be requested after 2 seconds.");
        CancellationTokenSource cts = new();

        Task.Run(() =>
        {
            Thread.Sleep(2000);
            Console.WriteLine("\nCancellation requested!");
            cts.Cancel();
        });

        // Use ParallelLoopState.Stop() to exit the loop cooperatively without
        // throwing OperationCanceledException, so the demo ends cleanly.
        ParallelLoopResult result = Parallel.For(0, 100, (i, state) =>
        {
            if (cts.IsCancellationRequested)
            {
                state.Stop();
                return;
            }

            Console.WriteLine($"Processing item {i}");
            Thread.Sleep(300);
        });

        if (!result.IsCompleted)
        {
            Console.WriteLine($"Parallel loop stopped gracefully after cancellation ");
        }
        else
        {
            Console.WriteLine("Parallel loop completed before cancellation was observed.");
        }
    }

    public static void ParallelInvokeExample()
    {
        Console.WriteLine("\n=== Parallel.Invoke Example ===");

        Parallel.Invoke(
            () =>
            {
                Console.WriteLine($"Action 1 starting on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine("Action 1 completed");
            },
            () =>
            {
                Console.WriteLine($"Action 2 starting on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(800);
                Console.WriteLine("Action 2 completed");
            },
            () =>
            {
                Console.WriteLine($"Action 3 starting on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(600);
                Console.WriteLine("Action 3 completed");
            }
        );

        Console.WriteLine("All actions completed");
    }

    public static void PLINQExample()
    {
        Console.WriteLine("\n=== PLINQ (Parallel LINQ) Example ===");

        var numbers = Enumerable.Range(1, 20).ToList();

        Console.WriteLine("Sequential processing:");
        var sequentialResult = numbers
            .Select(n =>
            {
                Thread.Sleep(100);
                return n * n;
            })
            .ToList();

        Console.WriteLine($"Sequential result count: {sequentialResult.Count}");

        Console.WriteLine("\nParallel processing:");
        var parallelResult = numbers
            .AsParallel()
            .Select(n =>
            {
                Console.WriteLine($"Processing {n} on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(100);
                return n * n;
            })
            .ToList();

        Console.WriteLine($"Parallel result count: {parallelResult.Count}");
        Console.WriteLine($"Sum of squares: {parallelResult.Sum()}");
    }

    public static void PLINQOrderedExample()
    {
        Console.WriteLine("\n=== PLINQ with AsOrdered ===");

        var numbers = Enumerable.Range(1, 10);

        Console.WriteLine("Unordered parallel processing:");
        var unorderedResult = numbers
            .AsParallel()
            .Select(n =>
            {
                Thread.Sleep(100);
                return n;
            })
            .ToList();
        Console.WriteLine($"Result: {string.Join(", ", unorderedResult)}");

        Console.WriteLine("\nOrdered parallel processing:");
        var orderedResult = numbers
            .AsParallel()
            .AsOrdered()
            .Select(n =>
            {
                Thread.Sleep(100);
                return n;
            })
            .ToList();
        Console.WriteLine($"Result: {string.Join(", ", orderedResult)}");
    }

    public static void PLINQAggregateExample()
    {
        Console.WriteLine("\n=== PLINQ Aggregate Example ===");

        var numbers = Enumerable.Range(1, 1000);

        int sum = numbers
            .AsParallel()
            .Aggregate(
                0,
                (localSum, n) => localSum + n,
                (total, localSum) => total + localSum,
                finalResult => finalResult
            );

        Console.WriteLine($"Sum of 1 to 1000: {sum}");
        Console.WriteLine($"Expected: {1000 * 1001 / 2}");
    }

    public static void DataflowExample()
    {
        Console.WriteLine("\n=== TPL Dataflow Example ===");

        var transformBlock = new TransformBlock<int, int>(
            n =>
            {
                Console.WriteLine($"Transform: {n} -> {n * n} on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(200);
                return n * n;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 3
            });

        var actionBlock = new ActionBlock<int>(
            n =>
            {
                Console.WriteLine($"Action: Received {n} on thread {Thread.CurrentThread.ManagedThreadId}");
            });

        transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

        for (int i = 1; i <= 5; i++)
        {
            transformBlock.Post(i);
        }

        transformBlock.Complete();
        actionBlock.Completion.Wait();
        Console.WriteLine("Dataflow completed");
    }

    public static void PartitionerExample()
    {
        Console.WriteLine("\n=== Custom Partitioner Example ===");

        var numbers = Enumerable.Range(1, 100).ToList();
        var partitioner = System.Collections.Concurrent.Partitioner.Create(numbers, loadBalance: true);

        Parallel.ForEach(partitioner, (number, state) =>
        {
            Console.WriteLine($"Processing {number} on thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(50);
        });

        Console.WriteLine("Partitioned processing completed");
    }
}
