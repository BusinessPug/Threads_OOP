using Threads_OOP.Examples;

namespace Threads_OOP;

internal static class DemoOrchestrator
{
    public static async Task RunAllDemos()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        C# Threading & Concurrency Demonstrations           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

        await RunThreadingDemos();
        await RunTaskDemos();
        await RunSynchronizationDemos();
        await RunAtomicDemos();
        await RunParallelDemos();
        await RunPerformanceDemos();
        await RunAdvancedDemos();

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  All Demos Completed!                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
    }

    public static async Task RunThreadingDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│              BASIC THREADING EXAMPLES                   │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        ThreadingExamples.BasicThreadWithJoin();
        WaitForUser();

        ThreadingExamples.BasicThreadNoWait();
        WaitForUser();

        ThreadingExamples.ThreadWithPriority();
        WaitForUser();

        ThreadingExamples.ThreadPoolExample();
        WaitForUser();

        ThreadingExamples.BackgroundThreadExample();
        WaitForUser();
    }

    public static async Task RunTaskDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│              TASK-BASED EXAMPLES                        │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        await TaskExamples.BasicTaskExample();
        WaitForUser();

        await TaskExamples.TaskWithReturnValue();
        WaitForUser();

        await TaskExamples.TaskCancellationExample();
        WaitForUser();

        await TaskExamples.TaskContinuationExample();
        WaitForUser();

        await TaskExamples.ParallelTaskProcessing();
        WaitForUser();

        await TaskExamples.FireAndForgetExample();
        WaitForUser();

        await TaskExamples.TaskWhenAnyExample();
        WaitForUser();

        await TaskExamples.AsyncLazyExample();
        WaitForUser();

        await TaskExamples.TaskStableCounterExample();
        WaitForUser();
    }

    public static async Task RunSynchronizationDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│           SYNCHRONIZATION EXAMPLES                      │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        SynchronizationExamples.UnstableExample();
        WaitForUser();

        SynchronizationExamples.InterlockedExample();
        WaitForUser();

        SynchronizationExamples.LockExample();
        WaitForUser();

        await SynchronizationExamples.SemaphoreExample();
        WaitForUser();

        SynchronizationExamples.ReaderWriterLockExample();
        WaitForUser();

        SynchronizationExamples.MutexExample();
        WaitForUser();

        SynchronizationExamples.MonitorExample();
        WaitForUser();

        SynchronizationExamples.VolatileExample();
        WaitForUser();
    }

    public static async Task RunAtomicDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│              ATOMIC OPERATIONS EXAMPLES                 │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        AtomicExamples.InterlockedIncrementDecrement();
        WaitForUser();

        AtomicExamples.InterlockedExchange();
        WaitForUser();

        AtomicExamples.InterlockedCompareExchange();
        WaitForUser();

        AtomicExamples.InterlockedAdd();
        WaitForUser();

        AtomicExamples.InterlockedRead();
        WaitForUser();

        AtomicExamples.SpinLockExample();
        WaitForUser();

        AtomicExamples.SpinWaitExample();
        WaitForUser();
    }

    public static async Task RunParallelDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│           PARALLEL PROCESSING EXAMPLES                  │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        ParallelExamples.ParallelForExample();
        WaitForUser();

        ParallelExamples.ParallelForEachExample();
        WaitForUser();

        ParallelExamples.ParallelForWithOptions();
        WaitForUser();

        ParallelExamples.ParallelForWithCancellation();
        WaitForUser();

        ParallelExamples.ParallelInvokeExample();
        WaitForUser();

        ParallelExamples.PLINQExample();
        WaitForUser();

        ParallelExamples.PLINQOrderedExample();
        WaitForUser();

        ParallelExamples.PLINQAggregateExample();
        WaitForUser();

        ParallelExamples.DataflowExample();
        WaitForUser();

        ParallelExamples.PartitionerExample();
        WaitForUser();
    }

    public static async Task RunPerformanceDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│           PERFORMANCE COMPARISONS                       │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        PerformanceExamples.SequentialVsParallelComparison();
        WaitForUser();

        PerformanceExamples.LockVsInterlockedPerformance();
        WaitForUser();

        PerformanceExamples.SpinLockVsLockPerformance();
        WaitForUser();

        await PerformanceExamples.TaskVsThreadPerformance();
        WaitForUser();

        PerformanceExamples.ConcurrentCollectionsPerformance();
        WaitForUser();

        PerformanceExamples.ContextSwitchingOverhead();
        WaitForUser();
    }

    public static async Task RunAdvancedDemos()
    {
        Console.WriteLine("\n\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│              ADVANCED PATTERNS & SCENARIOS              │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        AdvancedExamples.ProducerConsumerExample();
        WaitForUser();

        AdvancedExamples.DeadlockAvoidanceExample();
        WaitForUser();

        AdvancedExamples.MonitorTryEnterExample();
        WaitForUser();

        await AdvancedExamples.AsyncCoordinationExample();
        WaitForUser();

        AdvancedExamples.BarrierExample();
        WaitForUser();

        AdvancedExamples.CountdownEventExample();
        WaitForUser();

        await AdvancedExamples.ChannelExample();
        WaitForUser();

        Console.WriteLine("\n⚠️  WARNING: Next demo will show a DEADLOCK!");
        Console.WriteLine("Press any key to continue (you may need to force close)...");
        Console.ReadKey(true);
        AdvancedExamples.DeadlockExample();
        WaitForUser();
    }

    private static void WaitForUser()
    {
        Console.WriteLine("\n[Press any key to continue...]");
        Console.ReadKey(true);
    }
}
