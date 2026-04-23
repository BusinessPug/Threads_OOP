using System.Collections.Concurrent;

namespace Threads_OOP.Examples;

internal static class AdvancedExamples
{
    public static void ProducerConsumerExample()
    {
        Console.WriteLine("\n=== Producer-Consumer Pattern ===");
        BlockingCollection<int> queue = new(boundedCapacity: 5);

        Thread producer = new(() =>
        {
            for (int i = 1; i <= 10; i++)
            {
                queue.Add(i);
                Console.WriteLine($"Producer: Added {i} (Queue size: {queue.Count})");
                Thread.Sleep(300);
            }
            queue.CompleteAdding();
            Console.WriteLine("Producer: Finished");
        });

        Thread consumer = new(() =>
        {
            try
            {
                foreach (var item in queue.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Consumer: Processing {item}");
                    Thread.Sleep(500);
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Consumer: Queue is completed");
            }
            Console.WriteLine("Consumer: Finished");
        });

        producer.Start();
        consumer.Start();
        producer.Join();
        consumer.Join();
    }

    public static void DeadlockExample()
    {
        Console.WriteLine("\n=== Deadlock Demonstration (Will hang!) ===");
        Console.WriteLine("This will demonstrate a deadlock. Press Ctrl+C to stop if needed.");

        object lock1 = new();
        object lock2 = new();
        bool deadlockOccurred = false;

        Thread thread1 = new(() =>
        {
            lock (lock1)
            {
                Console.WriteLine("Thread 1: Acquired lock1");
                Thread.Sleep(100);

                Console.WriteLine("Thread 1: Waiting for lock2...");
                lock (lock2)
                {
                    Console.WriteLine("Thread 1: Acquired lock2");
                }
            }
        });

        Thread thread2 = new(() =>
        {
            lock (lock2)
            {
                Console.WriteLine("Thread 2: Acquired lock2");
                Thread.Sleep(100);

                Console.WriteLine("Thread 2: Waiting for lock1...");
                lock (lock1)
                {
                    Console.WriteLine("Thread 2: Acquired lock1");
                }
            }
        });

        thread1.Start();
        thread2.Start();

        bool completed = thread1.Join(TimeSpan.FromSeconds(3));
        completed &= thread2.Join(TimeSpan.FromSeconds(3));

        if (!completed)
        {
            Console.WriteLine("\n⚠️  DEADLOCK DETECTED! Threads are waiting for each other.");
            Console.WriteLine("In production, use timeout-based locks or lock ordering to prevent this.");
        }
    }

    public static void DeadlockAvoidanceExample()
    {
        Console.WriteLine("\n=== Deadlock Avoidance with Lock Ordering ===");

        object lock1 = new();
        object lock2 = new();

        Thread thread1 = new(() =>
        {
            lock (lock1)
            {
                Console.WriteLine("Thread 1: Acquired lock1");
                Thread.Sleep(100);

                lock (lock2)
                {
                    Console.WriteLine("Thread 1: Acquired lock2");
                }
            }
        });

        Thread thread2 = new(() =>
        {
            lock (lock1)
            {
                Console.WriteLine("Thread 2: Acquired lock1");
                Thread.Sleep(100);

                lock (lock2)
                {
                    Console.WriteLine("Thread 2: Acquired lock2");
                }
            }
        });

        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine("No deadlock - both threads acquired locks in the same order!");
    }

    public static void MonitorTryEnterExample()
    {
        Console.WriteLine("\n=== Monitor.TryEnter (Deadlock Prevention) ===");

        object lock1 = new();
        object lock2 = new();

        void SafeAcquireLocks(string threadName)
        {
            while (true)
            {
                bool lock1Acquired = false;
                bool lock2Acquired = false;

                try
                {
                    Monitor.TryEnter(lock1, TimeSpan.FromMilliseconds(100), ref lock1Acquired);
                    if (lock1Acquired)
                    {
                        Console.WriteLine($"{threadName}: Acquired lock1");
                        Thread.Sleep(50);

                        Monitor.TryEnter(lock2, TimeSpan.FromMilliseconds(100), ref lock2Acquired);
                        if (lock2Acquired)
                        {
                            Console.WriteLine($"{threadName}: Acquired lock2 - Success!");
                            Thread.Sleep(100);
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"{threadName}: Couldn't get lock2, releasing lock1 and retrying");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{threadName}: Couldn't get lock1, retrying");
                    }
                }
                finally
                {
                    if (lock2Acquired) Monitor.Exit(lock2);
                    if (lock1Acquired) Monitor.Exit(lock1);
                }

                Thread.Sleep(Random.Shared.Next(10, 50));
            }
        }

        Thread thread1 = new(() => SafeAcquireLocks("Thread 1"));
        Thread thread2 = new(() => SafeAcquireLocks("Thread 2"));

        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
    }

    public static async Task AsyncCoordinationExample()
    {
        Console.WriteLine("\n=== Async Coordination with SemaphoreSlim ===");
        SemaphoreSlim semaphore = new(0);

        Task worker1 = Task.Run(async () =>
        {
            Console.WriteLine("Worker 1: Doing some work...");
            await Task.Delay(1000);
            Console.WriteLine("Worker 1: Work completed, signaling");
            semaphore.Release();
        });

        Task worker2 = Task.Run(async () =>
        {
            Console.WriteLine("Worker 2: Doing some work...");
            await Task.Delay(1500);
            Console.WriteLine("Worker 2: Work completed, signaling");
            semaphore.Release();
        });

        Task coordinator = Task.Run(async () =>
        {
            Console.WriteLine("Coordinator: Waiting for both workers...");
            await semaphore.WaitAsync();
            await semaphore.WaitAsync();
            Console.WriteLine("Coordinator: Both workers completed!");
        });

        await Task.WhenAll(worker1, worker2, coordinator);
    }

    public static void BarrierExample()
    {
        Console.WriteLine("\n=== Barrier Example (Phase-Based Coordination) ===");
        int phaseNumber = 0;
        Barrier barrier = new(3, (b) =>
        {
            phaseNumber++;
            Console.WriteLine($"\n--- Phase {phaseNumber} completed ---");
        });

        void PhaseWorker(string name)
        {
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"{name}: Working on phase {i}");
                Thread.Sleep(Random.Shared.Next(500, 1000));
                Console.WriteLine($"{name}: Reached barrier for phase {i}");
                barrier.SignalAndWait();
            }
            Console.WriteLine($"{name}: All phases completed");
        }

        Thread thread1 = new(() => PhaseWorker("Worker 1"));
        Thread thread2 = new(() => PhaseWorker("Worker 2"));
        Thread thread3 = new(() => PhaseWorker("Worker 3"));

        thread1.Start();
        thread2.Start();
        thread3.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();

        barrier.Dispose();
    }

    public static void CountdownEventExample()
    {
        Console.WriteLine("\n=== CountdownEvent Example ===");
        CountdownEvent countdown = new(5);

        for (int i = 1; i <= 5; i++)
        {
            int taskId = i;
            Thread thread = new(() =>
            {
                Console.WriteLine($"Task {taskId}: Starting...");
                Thread.Sleep(taskId * 200);
                Console.WriteLine($"Task {taskId}: Completed");
                countdown.Signal();
            });
            thread.Start();
        }

        Console.WriteLine("Main: Waiting for all tasks to complete...");
        countdown.Wait();
        Console.WriteLine("Main: All tasks completed!");
        countdown.Dispose();
    }

    public static async Task ChannelExample()
    {
        Console.WriteLine("\n=== System.Threading.Channels Example ===");
        var channel = System.Threading.Channels.Channel.CreateBounded<int>(3);

        Task producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 10; i++)
            {
                await channel.Writer.WriteAsync(i);
                Console.WriteLine($"Producer: Wrote {i}");
                await Task.Delay(200);
            }
            channel.Writer.Complete();
            Console.WriteLine("Producer: Completed");
        });

        Task consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumer: Read {item}");
                await Task.Delay(300);
            }
            Console.WriteLine("Consumer: Channel closed");
        });

        await Task.WhenAll(producer, consumer);
    }
}
