namespace Threads_OOP.Examples;

internal static class SynchronizationExamples
{
    private static readonly SemaphoreSlim _semaphore = new(3, 3);
    private static readonly ReaderWriterLockSlim _rwLock = new();
    private static readonly Mutex _mutex = new();
    private static int _sharedResource = 0;

    public static void InterlockedExample()
    {
        Console.WriteLine("\n=== Interlocked Operations (Thread-Safe) ===");
        Counter counter = new();
        Thread thread1 = new(() => CounterIncInterlocked(counter));
        Thread thread2 = new(() => CounterIncInterlocked(counter));
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine($"Expected: 2000, Actual: {counter.GetValue()}");
    }

    public static void LockExample()
    {
        Console.WriteLine("\n=== Lock Statement (Thread-Safe) ===");
        Counter counter = new();
        object lockObj = new();
        Thread thread1 = new(() => CounterIncLock(counter, lockObj));
        Thread thread2 = new(() => CounterIncLock(counter, lockObj));
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine($"Expected: 2000, Actual: {counter.GetValue()}");
    }

    public static void UnstableExample()
    {
        Console.WriteLine("\n=== No Synchronization (Unstable) ===");
        Counter counter = new();
        Thread thread1 = new(() => CounterInc(counter));
        Thread thread2 = new(() => CounterInc(counter));
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine($"Expected: 2000, Actual: {counter.GetValue()} (likely less due to race condition)");
    }

    public static async Task SemaphoreExample()
    {
        Console.WriteLine("\n=== Semaphore Example (Max 3 concurrent) ===");
        List<Task> tasks = new();
        for (int i = 1; i <= 6; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () => await AccessResourceWithSemaphore(taskId)));
        }
        await Task.WhenAll(tasks);
    }

    public static void ReaderWriterLockExample()
    {
        Console.WriteLine("\n=== ReaderWriterLock Example ===");
        List<Thread> threads = new();

        for (int i = 0; i < 3; i++)
        {
            int readerId = i;
            threads.Add(new Thread(() => ReadResource(readerId)));
        }

        for (int i = 0; i < 2; i++)
        {
            int writerId = i;
            threads.Add(new Thread(() => WriteResource(writerId)));
        }

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
    }

    public static void MutexExample()
    {
        Console.WriteLine("\n=== Mutex Example ===");
        Thread thread1 = new(() => AccessResourceWithMutex(1));
        Thread thread2 = new(() => AccessResourceWithMutex(2));
        Thread thread3 = new(() => AccessResourceWithMutex(3));

        thread1.Start();
        thread2.Start();
        thread3.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();
    }

    public static void MonitorExample()
    {
        Console.WriteLine("\n=== Monitor Example with Pulse/Wait ===");
        object lockObj = new();
        bool dataReady = false;

        Thread producer = new(() =>
        {
            Thread.Sleep(1000);
            lock (lockObj)
            {
                Console.WriteLine("Producer: Data is ready");
                dataReady = true;
                Monitor.Pulse(lockObj);
            }
        });

        Thread consumer = new(() =>
        {
            lock (lockObj)
            {
                Console.WriteLine("Consumer: Waiting for data...");
                while (!dataReady)
                {
                    Monitor.Wait(lockObj);
                }
                Console.WriteLine("Consumer: Data received!");
            }
        });

        consumer.Start();
        producer.Start();
        producer.Join();
        consumer.Join();
    }

    public static void VolatileExample()
    {
        Console.WriteLine("\n=== Volatile Variable Example ===");
        VolatileWorker worker = new();
        Thread workerThread = new(worker.DoWork);
        workerThread.Start();

        Thread.Sleep(2000);
        worker.Stop();
        workerThread.Join();
        Console.WriteLine("Worker stopped");
    }

    private static async Task AccessResourceWithSemaphore(int taskId)
    {
        Console.WriteLine($"Task {taskId} waiting to enter...");
        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Task {taskId} entered (available slots: {_semaphore.CurrentCount})");
            await Task.Delay(1000);
        }
        finally
        {
            Console.WriteLine($"Task {taskId} exiting");
            _semaphore.Release();
        }
    }

    private static void ReadResource(int readerId)
    {
        _rwLock.EnterReadLock();
        try
        {
            Console.WriteLine($"Reader {readerId} reading: {_sharedResource}");
            Thread.Sleep(500);
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    private static void WriteResource(int writerId)
    {
        _rwLock.EnterWriteLock();
        try
        {
            _sharedResource++;
            Console.WriteLine($"Writer {writerId} writing: {_sharedResource}");
            Thread.Sleep(1000);
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    private static void AccessResourceWithMutex(int threadId)
    {
        Console.WriteLine($"Thread {threadId} waiting for mutex...");
        _mutex.WaitOne();
        try
        {
            Console.WriteLine($"Thread {threadId} acquired mutex");
            Thread.Sleep(1000);
        }
        finally
        {
            Console.WriteLine($"Thread {threadId} releasing mutex");
            _mutex.ReleaseMutex();
        }
    }

    private static void CounterInc(Counter c)
    {
        for (int i = 0; i < 1000; i++) 
            c.Increment();
    }

    private static void CounterIncInterlocked(Counter c)
    {
        for (int i = 0; i < 1000; i++) 
            c.IncrementInterlocked();
    }

    private static void CounterIncLock(Counter c, object lockObj)
    {
        for (int i = 0; i < 1000; i++)
        {
            lock (lockObj)
            {
                c.Increment();
            }
        }
    }
}

internal class VolatileWorker
{
    private volatile bool _shouldStop;

    public void DoWork()
    {
        int iteration = 0;
        while (!_shouldStop)
        {
            Console.WriteLine($"Working... iteration {++iteration}");
            Thread.Sleep(500);
        }
        Console.WriteLine("Worker received stop signal");
    }

    public void Stop()
    {
        _shouldStop = true;
    }
}
