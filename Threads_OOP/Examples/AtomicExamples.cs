namespace Threads_OOP.Examples;

internal static class AtomicExamples
{
    public static void InterlockedIncrementDecrement()
    {
        Console.WriteLine("\n=== Interlocked Increment/Decrement ===");
        int value = 0;
        Thread[] threads = new Thread[10];

        for (int i = 0; i < 5; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 1000; j++)
                    Interlocked.Increment(ref value);
            });
        }

        for (int i = 5; i < 10; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 1000; j++)
                    Interlocked.Decrement(ref value);
            });
        }

        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        Console.WriteLine($"Final value: {value} (Expected: 0)");
    }

    public static void InterlockedExchange()
    {
        Console.WriteLine("\n=== Interlocked Exchange ===");
        int sharedValue = 0;

        Thread writer = new(() =>
        {
            for (int i = 1; i <= 5; i++)
            {
                int oldValue = Interlocked.Exchange(ref sharedValue, i * 10);
                Console.WriteLine($"Writer: Set value to {i * 10}, old value was {oldValue}");
                Thread.Sleep(500);
            }
        });

        Thread reader = new(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                int value = Interlocked.CompareExchange(ref sharedValue, 0, 0);
                Console.WriteLine($"Reader: Current value is {value}");
                Thread.Sleep(600);
            }
        });

        writer.Start();
        reader.Start();
        writer.Join();
        reader.Join();
    }

    public static void InterlockedCompareExchange()
    {
        Console.WriteLine("\n=== Interlocked CompareExchange (Optimistic Locking) ===");
        int balance = 1000;

        Thread[] threads = new Thread[5];
        for (int i = 0; i < threads.Length; i++)
        {
            int threadId = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 3; j++)
                {
                    int currentBalance, newBalance;
                    do
                    {
                        currentBalance = balance;
                        newBalance = currentBalance - 50;
                        Thread.Sleep(10);
                    } while (Interlocked.CompareExchange(ref balance, newBalance, currentBalance) != currentBalance);

                    Console.WriteLine($"Thread {threadId}: Withdrew 50, new balance: {newBalance}");
                }
            });
        }

        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        Console.WriteLine($"Final balance: {balance} (Expected: {1000 - (5 * 3 * 50)})");
    }

    public static void InterlockedAdd()
    {
        Console.WriteLine("\n=== Interlocked Add ===");
        long counter = 0;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            int threadId = i + 1;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    Interlocked.Add(ref counter, threadId);
                }
            });
        }

        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        long expected = (1 + 2 + 3 + 4) * 1000;
        Console.WriteLine($"Final counter: {counter} (Expected: {expected})");
    }

    public static void InterlockedRead()
    {
        Console.WriteLine("\n=== Interlocked Read (for 64-bit values on 32-bit systems) ===");
        long sharedValue = 0;

        Thread writer = new(() =>
        {
            for (long i = 1; i <= 1000000; i++)
            {
                sharedValue = i;
            }
        });

        Thread reader = new(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                long value = Interlocked.Read(ref sharedValue);
                Console.WriteLine($"Read value: {value}");
                Thread.Sleep(200);
            }
        });

        writer.Start();
        reader.Start();
        writer.Join();
        reader.Join();
    }

    public static void SpinLockExample()
    {
        Console.WriteLine("\n=== SpinLock Example ===");
        SpinLock spinLock = new();
        int counter = 0;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    bool lockTaken = false;
                    try
                    {
                        spinLock.Enter(ref lockTaken);
                        counter++;
                    }
                    finally
                    {
                        if (lockTaken)
                            spinLock.Exit();
                    }
                }
            });
        }

        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        Console.WriteLine($"Final counter: {counter} (Expected: 4000)");
    }

    public static void SpinWaitExample()
    {
        Console.WriteLine("\n=== SpinWait Example ===");
        bool dataReady = false;

        Thread producer = new(() =>
        {
            Console.WriteLine("Producer: Preparing data...");
            Thread.Sleep(1000);
            dataReady = true;
            Console.WriteLine("Producer: Data ready");
        });

        Thread consumer = new(() =>
        {
            Console.WriteLine("Consumer: Waiting for data...");
            SpinWait spinWait = new();
            while (!dataReady)
            {
                spinWait.SpinOnce();
            }
            Console.WriteLine("Consumer: Data received!");
        });

        consumer.Start();
        producer.Start();
        producer.Join();
        consumer.Join();
    }
}
