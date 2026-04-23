namespace Threads_OOP.Examples;

internal static class ThreadingExamples
{
    public static void BasicThreadWithJoin()
    {
        Console.WriteLine("\n=== Basic Thread with Join ===");
        Thread thread1 = new(new ThreadStart(PrintTask.PrintThreadNoWait));
        Thread thread2 = new(new ThreadStart(PrintTask.PrintThreadNoWait));
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine("Both threads completed.");
    }

    public static void BasicThreadNoWait()
    {
        Console.WriteLine("\n=== Basic Thread without Join ===");
        Thread thread1 = new(new ThreadStart(PrintTask.PrintThreadNoWait));
        Thread thread2 = new(new ThreadStart(PrintTask.PrintThreadNoWait));
        thread1.Start();
        thread2.Start();
        Console.WriteLine("Started threads without waiting for completion.");
        Console.WriteLine("(Main has already moved past Start(); waiting here only so their output " +
                          "finishes before the next demo step.)");

        // Without this, the threads keep printing after the "Press any key" prompt
        // and bleed into the next example. The teaching point (no Join at start) is
        // still intact - we just drain the output before returning to the orchestrator.
        thread1.Join();
        thread2.Join();
    }

    public static void ThreadWithPriority()
    {
        Console.WriteLine("\n=== Thread Priority Example ===");
        Thread lowPriority = new(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Low Priority Thread: {i}");
                Thread.Sleep(100);
            }
        })
        {
            Priority = ThreadPriority.Lowest
        };

        Thread highPriority = new(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"High Priority Thread: {i}");
                Thread.Sleep(100);
            }
        })
        {
            Priority = ThreadPriority.Highest
        };

        lowPriority.Start();
        highPriority.Start();
        lowPriority.Join();
        highPriority.Join();
    }

    public static void ThreadPoolExample()
    {
        Console.WriteLine("\n=== ThreadPool Example ===");
        for (int i = 0; i < 5; i++)
        {
            int taskNumber = i;
            ThreadPool.QueueUserWorkItem(state =>
            {
                Console.WriteLine($"Task {taskNumber} executing on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine($"Task {taskNumber} completed on thread {Thread.CurrentThread.ManagedThreadId}");
            });
        }
        Thread.Sleep(2000);
    }

    public static void BackgroundThreadExample()
    {
        Console.WriteLine("\n=== Background vs Foreground Thread ===");

        Thread foregroundThread = new(() =>
        {
            Console.WriteLine("Foreground thread started");
            Thread.Sleep(1000);
            Console.WriteLine("Foreground thread completed");
        })
        {
            IsBackground = false
        };

        Thread backgroundThread = new(() =>
        {
            Console.WriteLine("Background thread started");
            Thread.Sleep(5000);
            Console.WriteLine("Background thread completed (won't see this)");
        })
        {
            IsBackground = true
        };

        foregroundThread.Start();
        backgroundThread.Start();
        foregroundThread.Join();
        Console.WriteLine("Main thread continues; if the process exited now, the background thread " +
                          "would be terminated immediately.");
        Console.WriteLine("(This demo keeps running, so we wait for the background thread here to " +
                          "avoid its output leaking into the next step.)");

        // In a standalone app, process exit would kill the background thread and
        // "Background thread completed" would never print. Because this demo is
        // hosted inside a larger run, we explicitly wait so the output stays in order.
        backgroundThread.Join();
    }
}
