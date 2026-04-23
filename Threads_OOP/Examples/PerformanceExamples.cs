using System.Diagnostics;

namespace Threads_OOP.Examples;

internal static class PerformanceExamples
{
    public static void SequentialVsParallelComparison()
    {
        Console.WriteLine("\n=== Sequential vs Parallel Performance ===");

        int[] data = Enumerable.Range(1, 1_000_000).ToArray();
        Stopwatch sw = new();

        // Sequential
        sw.Start();
        long sequentialSum = 0;
        foreach (var item in data)
        {
            sequentialSum += ComputeIntensive(item);
        }
        sw.Stop();
        var sequentialTime = sw.ElapsedMilliseconds;

        // Parallel
        sw.Restart();
        long parallelSum = 0;
        object lockObj = new();
        Parallel.ForEach(data, item =>
        {
            long result = ComputeIntensive(item);
            lock (lockObj)
            {
                parallelSum += result;
            }
        });
        sw.Stop();
        var parallelTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Sequential: {sequentialTime}ms, Sum: {sequentialSum}");
        Console.WriteLine($"Parallel: {parallelTime}ms, Sum: {parallelSum}");
        Console.WriteLine($"Speedup: {(double)sequentialTime / parallelTime:F2}x");
    }

    public static void LockVsInterlockedPerformance()
    {
        Console.WriteLine("\n=== Lock vs Interlocked Performance ===");
        const int iterations = 1000000;
        Stopwatch sw = new();

        // Using lock
        int lockCounter = 0;
        object lockObj = new();
        sw.Start();
        Parallel.For(0, iterations, i =>
        {
            lock (lockObj)
            {
                lockCounter++;
            }
        });
        sw.Stop();
        var lockTime = sw.ElapsedMilliseconds;

        // Using Interlocked
        int interlockedCounter = 0;
        sw.Restart();
        Parallel.For(0, iterations, i =>
        {
            Interlocked.Increment(ref interlockedCounter);
        });
        sw.Stop();
        var interlockedTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Lock: {lockTime}ms, Final: {lockCounter}");
        Console.WriteLine($"Interlocked: {interlockedTime}ms, Final: {interlockedCounter}");
        Console.WriteLine($"Interlocked is {(double)lockTime / interlockedTime:F2}x faster");
    }

    public static void SpinLockVsLockPerformance()
    {
        Console.WriteLine("\n=== SpinLock vs Regular Lock Performance ===");
        const int iterations = 1000000;
        Stopwatch sw = new();

        // Regular lock
        int regularCounter = 0;
        object lockObj = new();
        sw.Start();
        Parallel.For(0, iterations, i =>
        {
            lock (lockObj)
            {
                regularCounter++;
            }
        });
        sw.Stop();
        var regularTime = sw.ElapsedMilliseconds;

        // SpinLock
        int spinCounter = 0;
        SpinLock spinLock = new();
        sw.Restart();
        Parallel.For(0, iterations, i =>
        {
            bool lockTaken = false;
            try
            {
                spinLock.Enter(ref lockTaken);
                spinCounter++;
            }
            finally
            {
                if (lockTaken)
                    spinLock.Exit();
            }
        });
        sw.Stop();
        var spinTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Regular Lock: {regularTime}ms, Final: {regularCounter}");
        Console.WriteLine($"SpinLock: {spinTime}ms, Final: {spinCounter}");
        Console.WriteLine($"Note: SpinLock is better for very short critical sections");
    }

    public static async Task TaskVsThreadPerformance()
    {
        Console.WriteLine("\n=== Task vs Thread Creation Performance ===");
        const int count = 1000;
        Stopwatch sw = new();

        // Thread creation
        sw.Start();
        Thread[] threads = new Thread[count];
        for (int i = 0; i < count; i++)
        {
            threads[i] = new Thread(() => { Thread.Sleep(1); });
            threads[i].Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
        sw.Stop();
        var threadTime = sw.ElapsedMilliseconds;

        // Task creation
        sw.Restart();
        Task[] tasks = new Task[count];
        for (int i = 0; i < count; i++)
        {
            tasks[i] = Task.Run(async () => { await Task.Delay(1); });
        }
        await Task.WhenAll(tasks);
        sw.Stop();
        var taskTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Thread creation: {threadTime}ms");
        Console.WriteLine($"Task creation: {taskTime}ms");
        Console.WriteLine($"Tasks are {(double)threadTime / taskTime:F2}x faster");
    }

    public static void ConcurrentCollectionsPerformance()
    {
        Console.WriteLine("\n=== Concurrent Collections Performance ===");
        const int operations = 100000;
        Stopwatch sw = new();

        // ConcurrentBag
        var concurrentBag = new System.Collections.Concurrent.ConcurrentBag<int>();
        sw.Start();
        Parallel.For(0, operations, i => concurrentBag.Add(i));
        sw.Stop();
        var bagTime = sw.ElapsedMilliseconds;

        // ConcurrentQueue
        var concurrentQueue = new System.Collections.Concurrent.ConcurrentQueue<int>();
        sw.Restart();
        Parallel.For(0, operations, i => concurrentQueue.Enqueue(i));
        sw.Stop();
        var queueTime = sw.ElapsedMilliseconds;

        // List with lock
        var list = new List<int>();
        object lockObj = new();
        sw.Restart();
        Parallel.For(0, operations, i =>
        {
            lock (lockObj)
            {
                list.Add(i);
            }
        });
        sw.Stop();
        var listTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"ConcurrentBag: {bagTime}ms, Count: {concurrentBag.Count}");
        Console.WriteLine($"ConcurrentQueue: {queueTime}ms, Count: {concurrentQueue.Count}");
        Console.WriteLine($"List+Lock: {listTime}ms, Count: {list.Count}");
    }

    public static void ContextSwitchingOverhead()
    {
        Console.WriteLine("\n=== Context Switching Overhead ===");
        const int operations = 1000000;
        Stopwatch sw = new();

        // Single thread
        sw.Start();
        long sum = 0;
        for (int i = 0; i < operations; i++)
        {
            sum += i;
        }
        sw.Stop();
        var singleThreadTime = sw.ElapsedMilliseconds;

        // Multiple threads with excessive locking
        sw.Restart();
        long parallelSum = 0;
        object lockObj = new();
        Parallel.For(0, operations, new ParallelOptions { MaxDegreeOfParallelism = 10 }, i =>
        {
            lock (lockObj)
            {
                parallelSum += i;
            }
        });
        sw.Stop();
        var parallelTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Single thread: {singleThreadTime}ms");
        Console.WriteLine($"Parallel with contention: {parallelTime}ms");
        Console.WriteLine($"Overhead ratio: {(double)parallelTime / singleThreadTime:F2}x slower");
        Console.WriteLine("Note: Parallelism isn't always faster due to overhead!");
    }

    private static long ComputeIntensive(int value)
    {
        // Simulate some computation
        long result = value;
        for (int i = 0; i < 100; i++)
        {
            result = (result * 31 + value) % 1000000007;
        }
        return result;
    }
}
