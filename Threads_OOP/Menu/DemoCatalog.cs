using Threads_OOP.Examples;

namespace Threads_OOP.Menu;

internal sealed record DemoItem(
    string Name,
    string MethodName,
    string SourceFile,
    string Description,
    Func<Task> Runner,
    bool Warning = false);

internal sealed record DemoCategory(string Name, string Icon, IReadOnlyList<DemoItem> Items);

// Github copilot generated the "emojis" :D
internal static class DemoCatalog
{
    public static IReadOnlyList<DemoCategory> Categories { get; } = new List<DemoCategory>
    {
        new("Basic Threading", "🧵", new List<DemoItem>
        {
            new("Basic Threads with Join", nameof(ThreadingExamples.BasicThreadWithJoin),
                "ThreadingExamples.cs",
                "Creates two Threads, starts them, and waits for them to complete via Join().\n" +
                "Join() blocks the calling thread (usually the main thread) until the target thread has finished its work.\n" +
                "Without Join(), the main thread would continue running and could even exit before the worker threads complete.\n" +
                "Think of it as saying: 'I'll pause here until you're done'.",
                () => Wrap(ThreadingExamples.BasicThreadWithJoin)),
            new("Basic Threads (no wait)", nameof(ThreadingExamples.BasicThreadNoWait),
                "ThreadingExamples.cs",
                "Starts two threads without waiting for them — shows fire-and-forget behavior.",
                () => Wrap(ThreadingExamples.BasicThreadNoWait)),
            new("Threads with Priority", nameof(ThreadingExamples.ThreadWithPriority),
                "ThreadingExamples.cs",
                "Demonstrates setting Thread.Priority to hint scheduling preference.\n" +
                "This demo is too short to reliably observe the effect.\nBut had it been longer, you might have seen the high priority execute more frequently.",
                () => Wrap(ThreadingExamples.ThreadWithPriority)),
            new("ThreadPool QueueUserWorkItem", nameof(ThreadingExamples.ThreadPoolExample),
                "ThreadingExamples.cs",
                "Queues work onto the managed ThreadPool using QueueUserWorkItem.\n" +
                "ThreadPools are a pool of worker threads managed by the runtime.\n" +
                "Creating a new Thread is expensive — allocating stack space, OS handles, etc.\n" +
                "The ThreadPool keeps a cache of reusable threads and hands one out when you queue work, then reclaims it when the work is done.\n" +
                "This is what Task.Run uses under the hood, and it's almost always what you want over 'new Thread(...)'.",
                () => Wrap(ThreadingExamples.ThreadPoolExample)),
            new("Background Thread", nameof(ThreadingExamples.BackgroundThreadExample),
                "ThreadingExamples.cs",
                "Shows IsBackground=true: process exits without waiting for the thread.\n" +
                "Threads in .NET come in two flavours: foreground and background.\n" +
                "Foreground threads keep the application alive — the process will not exit until every foreground thread has finished.\n" +
                "Background threads are the opposite: the runtime will kill them abruptly when the app shuts down.\n" +
                "Background threads are useful for non-critical work (logging, telemetry, periodic polling) that shouldn't prevent the app from closing.",
                () => Wrap(ThreadingExamples.BackgroundThreadExample)),
        }),
        new("Tasks & async/await", "⚡", new List<DemoItem>
        {
            new("Basic Task", nameof(TaskExamples.BasicTaskExample), "TaskExamples.cs",
                "Launches a Task with Task.Run and awaits its completion.\n" +
                "A Task is a higher-level abstraction over a Thread — it represents 'something that will eventually produce a result (or complete)'.\n" +
                "Task.Run schedules the delegate onto the ThreadPool and immediately returns a Task handle you can await.\n" +
                "'await' then suspends the current method until the Task finishes, without blocking the calling thread.",
                TaskExamples.BasicTaskExample),
            new("Task with Return Value", nameof(TaskExamples.TaskWithReturnValue), "TaskExamples.cs",
                "Returns a value from a Task<T> and awaits the result.\n" +
                "Task<T> is the generic version of Task — it promises to eventually produce a value of type T.\n" +
                "Awaiting a Task<T> unwraps the result so you can use it as if the call were synchronous,\n" +
                "but under the hood the method suspends and resumes without tying up a thread while it waits.",
                TaskExamples.TaskWithReturnValue),
            new("Task Cancellation", nameof(TaskExamples.TaskCancellationExample), "TaskExamples.cs",
                "Uses CancellationTokenSource to cooperatively cancel a running Task.\n" +
                "Cancellation in .NET is cooperative — you can't just 'kill' a Task from the outside.\n" +
                "Instead, a CancellationTokenSource produces a CancellationToken which you hand to the Task.\n" +
                "The Task is then responsible for periodically checking the token (via IsCancellationRequested or ThrowIfCancellationRequested)\n" +
                "and bailing out gracefully. This avoids the classic pitfalls of 'Thread.Abort' which could leave shared state corrupted.",
                TaskExamples.TaskCancellationExample),
            new("Task Continuation", nameof(TaskExamples.TaskContinuationExample), "TaskExamples.cs",
                "Chains work with ContinueWith after a Task finishes.\n" +
                "ContinueWith is the 'old school' way of saying: 'when this Task finishes, run this other piece of code'.\n" +
                "Modern C# mostly uses 'await' instead, which is cleaner and handles exceptions/context more safely,\n" +
                "but ContinueWith is still useful when you need fine-grained control over scheduling or want to react to faulted/cancelled states explicitly.",
                TaskExamples.TaskContinuationExample),
            new("Parallel Task Processing", nameof(TaskExamples.ParallelTaskProcessing), "TaskExamples.cs",
                "Runs multiple tasks concurrently and awaits them with Task.WhenAll.",
                TaskExamples.ParallelTaskProcessing),
            new("Fire and Forget", nameof(TaskExamples.FireAndForgetExample), "TaskExamples.cs",
                "Starts a Task without awaiting it — note exceptions can be lost.\n" +
                "'Fire and forget' means you kick off work and don't care when (or whether) it finishes.\n" +
                "The danger: if that Task throws an exception and nobody is awaiting it, the error silently disappears.\n" +
                "In older .NET Framework versions, unobserved Task exceptions would even crash the process on finalization.\n" +
                "Use this pattern sparingly, and always wrap the body in a try/catch that logs failures.",
                TaskExamples.FireAndForgetExample),
            new("Task.WhenAny", nameof(TaskExamples.TaskWhenAnyExample), "TaskExamples.cs",
                "Waits for the first of several tasks to complete.\n" +
                "Where Task.WhenAll says 'wait for all of these to finish', Task.WhenAny says 'wake me up as soon as ANY of them finishes'.\n" +
                "This is the building block for timeouts ('whichever finishes first: my work or a delay'), redundant requests, and race-style patterns.\n" +
                "Note that the other tasks keep running — WhenAny doesn't cancel the losers for you.",
                TaskExamples.TaskWhenAnyExample),
            new("AsyncLazy<T>", nameof(TaskExamples.AsyncLazyExample), "TaskExamples.cs",
                "Lazy async initialization pattern built on Lazy<Task<T>>.\n" +
                "Lazy<T> defers creating an expensive object until the first time it's actually requested, and caches it afterwards.\n" +
                "The trick here is that the 'expensive object' is itself an async operation (e.g. loading data from disk or a service).\n" +
                "By wrapping it as Lazy<Task<T>>, every caller awaits the same underlying Task — the work runs exactly once, no matter how many concurrent consumers ask for it.",
                TaskExamples.AsyncLazyExample),
            new("Stable Counter with Tasks", nameof(TaskExamples.TaskStableCounterExample), "TaskExamples.cs",
                "Increments a shared counter safely using Task and synchronization.",
                TaskExamples.TaskStableCounterExample),
        }),
        new("Synchronization", "🔒", new List<DemoItem>
        {
            new("Unstable (no sync)", nameof(SynchronizationExamples.UnstableExample),
                "SynchronizationExamples.cs",
                "Shows the race condition that occurs WITHOUT synchronization.\n" +
                "Incrementing a shared integer (counter++) LOOKS like one operation, but under the hood it's three:\n" +
                "read the value, add one, write it back. If two threads do this at the same time they can both read the same old value\n" +
                "and one of the increments gets lost. Run this a few times — you'll almost never get the 'correct' total.\n" +
                "This is exactly the kind of bug the rest of the synchronization demos are here to solve.",
                () => Wrap(SynchronizationExamples.UnstableExample)),
            new("Interlocked", nameof(SynchronizationExamples.InterlockedExample),
                "SynchronizationExamples.cs",
                "Fixes the race with Interlocked.Increment — atomic and fast.\n" +
                "Interlocked methods use special CPU instructions (like LOCK XADD / CMPXCHG) that perform the read-modify-write as a single, indivisible step.\n" +
                "No other thread can sneak in between the read and the write.\n" +
                "This is dramatically faster than taking a lock, because there's no OS involvement — it's all handled at the hardware level.",
                () => Wrap(SynchronizationExamples.InterlockedExample)),
            new("lock statement", nameof(SynchronizationExamples.LockExample),
                "SynchronizationExamples.cs",
                "Uses C# lock to provide mutual exclusion on a shared section.\n" +
                "'lock (x) { ... }' is syntactic sugar for Monitor.Enter/Exit wrapped in a try/finally.\n" +
                "Only one thread at a time can be inside the locked region for a given lock object; others wait their turn.\n" +
                "Rule of thumb: always lock on a private 'readonly object' field — never lock on 'this', a Type, or a string, because other code might lock on the same reference and deadlock you.",
                () => Wrap(SynchronizationExamples.LockExample)),
            new("SemaphoreSlim", nameof(SynchronizationExamples.SemaphoreExample),
                "SynchronizationExamples.cs",
                "Limits concurrency to N concurrent workers with SemaphoreSlim.\n" +
                "Where a lock allows exactly one thread in at a time, a semaphore allows up to N.\n" +
                "Think of it as a nightclub with a bouncer: 'capacity 5'. Threads call WaitAsync() to enter and Release() to leave.\n" +
                "Perfect for rate-limiting expensive operations — e.g. 'only allow 10 simultaneous HTTP requests to this API'.\n" +
                "SemaphoreSlim is the lightweight, async-friendly version; its cousin Semaphore is the kernel-backed, cross-process one.",
                SynchronizationExamples.SemaphoreExample),
            new("ReaderWriterLockSlim", nameof(SynchronizationExamples.ReaderWriterLockExample),
                "SynchronizationExamples.cs",
                "Allows many readers OR one writer — great for read-heavy workloads.\n" +
                "A regular lock is pessimistic: even two threads that only want to READ block each other unnecessarily.\n" +
                "ReaderWriterLockSlim separates the two: unlimited concurrent readers are allowed, but writers get exclusive access.\n" +
                "When a writer wants in, new readers are held back until it's done, preventing writer starvation.\n" +
                "Best for caches, config stores, and any data that's read far more often than it's mutated.",
                () => Wrap(SynchronizationExamples.ReaderWriterLockExample)),
            new("Mutex", nameof(SynchronizationExamples.MutexExample),
                "SynchronizationExamples.cs",
                "OS-level mutual exclusion primitive (named, cross-process capable).\n" +
                "Unlike 'lock', a Mutex is a real operating system object — heavier, but visible across process boundaries.\n" +
                "The classic use case is 'single instance application': give the mutex a system-wide name and if another process already holds it, refuse to start.\n" +
                "Because it crosses into kernel space, acquiring a Mutex is MUCH slower than a managed lock — reach for it only when you actually need cross-process coordination.",
                () => Wrap(SynchronizationExamples.MutexExample)),
            new("Monitor", nameof(SynchronizationExamples.MonitorExample),
                "SynchronizationExamples.cs",
                "Lower-level building block behind the lock keyword.\n" +
                "Every C# 'lock' statement is rewritten by the compiler into Monitor.Enter / Monitor.Exit inside a try/finally.\n" +
                "Using Monitor directly gives you extras that the keyword doesn't expose:\n" +
                "  • TryEnter with a timeout, so you don't block forever\n" +
                "  • Wait / Pulse / PulseAll, which let threads sleep on a lock and be woken when something changes (classic 'condition variable' pattern).",
                () => Wrap(SynchronizationExamples.MonitorExample)),
            new("volatile field", nameof(SynchronizationExamples.VolatileExample),
                "SynchronizationExamples.cs",
                "Shows volatile read/write semantics for a shared flag.\n" +
                "Modern CPUs and compilers are allowed to cache values in registers and reorder reads/writes for performance.\n" +
                "That means one thread's change to a plain field might never become visible to another thread — it can sit in a CPU cache forever.\n" +
                "Marking a field 'volatile' tells the runtime: 'always read/write this from main memory, and don't reorder accesses around it'.\n" +
                "It's a subtle primitive, mostly used for simple boolean 'stop' flags. For anything more complex, reach for Interlocked or a lock instead.",
                () => Wrap(SynchronizationExamples.VolatileExample)),
        }),
        new("Atomic Operations", "⚛", new List<DemoItem>
        {
            new("Interlocked.Increment / Decrement", nameof(AtomicExamples.InterlockedIncrementDecrement),
                "AtomicExamples.cs",
                "Atomic increment/decrement of a shared 32/64-bit integer.",
                () => Wrap(AtomicExamples.InterlockedIncrementDecrement)),
            new("Interlocked.Exchange", nameof(AtomicExamples.InterlockedExchange), "AtomicExamples.cs",
                "Atomically swaps a value and returns the original.",
                () => Wrap(AtomicExamples.InterlockedExchange)),
            new("Interlocked.CompareExchange", nameof(AtomicExamples.InterlockedCompareExchange),
                "AtomicExamples.cs",
                "CAS primitive — the foundation of most lock-free algorithms.\n" +
                "CAS stands for 'Compare-And-Swap'. In one atomic step it says:\n" +
                "  'if the current value is still X, replace it with Y; otherwise leave it alone and tell me what it actually was'.\n" +
                "That tiny guarantee is enough to build lock-free counters, queues, caches, and even the 'lock' keyword itself.\n" +
                "The typical pattern is a retry loop: read, compute a new value, CAS it in; if someone beat you to it, loop and try again.",
                () => Wrap(AtomicExamples.InterlockedCompareExchange)),
            new("Interlocked.Add", nameof(AtomicExamples.InterlockedAdd), "AtomicExamples.cs",
                "Atomically adds a value to a shared integer.",
                () => Wrap(AtomicExamples.InterlockedAdd)),
            new("Interlocked.Read (64-bit)", nameof(AtomicExamples.InterlockedRead), "AtomicExamples.cs",
                "Atomic read of a 64-bit value on 32-bit architectures.",
                () => Wrap(AtomicExamples.InterlockedRead)),
            new("SpinLock", nameof(AtomicExamples.SpinLockExample), "AtomicExamples.cs",
                "Lightweight busy-wait lock for very short critical sections.\n" +
                "A regular lock parks a waiting thread via the OS — cheap to wait, but expensive to get going again (context switch).\n" +
                "A SpinLock doesn't park; it simply spins in a tight loop checking the lock over and over until it's free.\n" +
                "That's wasteful if the wait is long, but ideal when the critical section is only a handful of instructions and you expect near-zero contention.\n" +
                "Use with care: spinning on a busy lock pegs a CPU core at 100%.",
                () => Wrap(AtomicExamples.SpinLockExample)),
            new("SpinWait", nameof(AtomicExamples.SpinWaitExample), "AtomicExamples.cs",
                "SpinWait helper for hybrid spin/yield waiting.\n" +
                "SpinWait is the 'smart' spinner. It starts by busy-looping (cheap if the wait is short),\n" +
                "and if it's been spinning too long, gradually escalates to yielding the CPU and eventually sleeping.\n" +
                "This gives you the low latency of a spin lock when contention is brief, without burning a full core if the wait drags on.",
                () => Wrap(AtomicExamples.SpinWaitExample)),
        }),
        new("Parallel Processing", "🧮", new List<DemoItem>
        {
            new("Parallel.For", nameof(ParallelExamples.ParallelForExample), "ParallelExamples.cs",
                "Parallelizes a for-loop across the ThreadPool.\n" +
                "Parallel.For chops the iteration range into chunks and runs them on multiple ThreadPool threads simultaneously.\n" +
                "It's a drop-in replacement for a regular 'for' when each iteration is independent of the others.\n" +
                "Warning: iterations may run out of order, so don't rely on side effects happening in sequence,\n" +
                "and any shared state inside the loop body still needs synchronization.",
                () => Wrap(ParallelExamples.ParallelForExample)),
            new("Parallel.ForEach", nameof(ParallelExamples.ParallelForEachExample), "ParallelExamples.cs",
                "Parallelizes an IEnumerable<T> iteration.",
                () => Wrap(ParallelExamples.ParallelForEachExample)),
            new("Parallel.For with Options", nameof(ParallelExamples.ParallelForWithOptions),
                "ParallelExamples.cs",
                "Caps parallelism with ParallelOptions.MaxDegreeOfParallelism.",
                () => Wrap(ParallelExamples.ParallelForWithOptions)),
            new("Parallel.For with Cancellation", nameof(ParallelExamples.ParallelForWithCancellation),
                "ParallelExamples.cs",
                "Stops a parallel loop cooperatively via ParallelLoopState.Stop().",
                () => Wrap(ParallelExamples.ParallelForWithCancellation)),
            new("Parallel.Invoke", nameof(ParallelExamples.ParallelInvokeExample), "ParallelExamples.cs",
                "Runs several independent actions concurrently.",
                () => Wrap(ParallelExamples.ParallelInvokeExample)),
            new("PLINQ", nameof(ParallelExamples.PLINQExample), "ParallelExamples.cs",
                "Compares sequential LINQ vs .AsParallel().\n" +
                "PLINQ = Parallel LINQ. Sticking '.AsParallel()' into a LINQ query tells the runtime it's free to split the work across multiple threads.\n" +
                "It shines on CPU-bound queries over large collections (heavy filtering, maths, transformations).\n" +
                "It's NOT magic though: for small collections or cheap operations, the overhead of coordinating threads can make it SLOWER than plain LINQ.\n" +
                "Also note that ordering is not preserved by default — see the AsOrdered demo.",
                () => Wrap(ParallelExamples.PLINQExample)),
            new("PLINQ AsOrdered", nameof(ParallelExamples.PLINQOrderedExample), "ParallelExamples.cs",
                "Preserves ordering in parallel queries using AsOrdered().",
                () => Wrap(ParallelExamples.PLINQOrderedExample)),
            new("PLINQ Aggregate", nameof(ParallelExamples.PLINQAggregateExample), "ParallelExamples.cs",
                "Parallel reduction using the four-argument Aggregate overload.\n" +
                "Aggregate is LINQ's 'fold' operation — it reduces a whole sequence down to a single value (a sum, a max, a concatenated string, etc.).\n" +
                "The parallel version can't just chain a single accumulator, because multiple threads are working at once.\n" +
                "Instead you provide FOUR things: a seed, a function that folds each item into a LOCAL accumulator (one per thread),\n" +
                "a function that merges two local accumulators together, and finally a projection for the end result.\n" +
                "This shape is exactly how map-reduce frameworks work under the hood.",
                () => Wrap(ParallelExamples.PLINQAggregateExample)),
            new("TPL Dataflow", nameof(ParallelExamples.DataflowExample), "ParallelExamples.cs",
                "Builds a pipeline with TransformBlock and ActionBlock.\n" +
                "TPL Dataflow lets you wire together independent 'blocks' — each with its own internal queue and degree of parallelism — into a pipeline.\n" +
                "TransformBlock<TIn,TOut> takes an input, does some work, and produces an output.\n" +
                "ActionBlock<T> is a sink — it consumes items and does something side-effecting with them.\n" +
                "Link blocks with LinkTo, and the runtime handles buffering, backpressure, and parallel execution for you.\n" +
                "Great for building processing pipelines: read → parse → enrich → save, each stage running at its own pace.",
                () => Wrap(ParallelExamples.DataflowExample)),
            new("Custom Partitioner", nameof(ParallelExamples.PartitionerExample), "ParallelExamples.cs",
                "Uses a load-balancing Partitioner for better work distribution.\n" +
                "By default, Parallel.For hands out contiguous chunks of the range to each worker thread.\n" +
                "That's fine when every iteration takes roughly the same time — but if some items are much slower than others,\n" +
                "one thread can end up stuck with all the slow work while the rest sit idle.\n" +
                "A custom Partitioner (e.g. Partitioner.Create with dynamic load balancing) lets workers pull smaller chunks on demand,\n" +
                "so fast workers keep helping out instead of finishing early and going home.",
                () => Wrap(ParallelExamples.PartitionerExample)),
        }),
        new("Performance Comparisons", "📊", new List<DemoItem>
        {
            new("Sequential vs Parallel", nameof(PerformanceExamples.SequentialVsParallelComparison),
                "PerformanceExamples.cs",
                "Times the same workload run sequentially vs in parallel.\n" +
                "A great reality check: parallelism is NOT free.\n" +
                "You pay for thread startup, synchronization, and memory contention, and you only win if the work per item is big enough to outweigh that cost.\n" +
                "This demo is a useful sanity check before you reach for Parallel.For or PLINQ in real code.",
                () => Wrap(PerformanceExamples.SequentialVsParallelComparison)),
            new("lock vs Interlocked", nameof(PerformanceExamples.LockVsInterlockedPerformance),
                "PerformanceExamples.cs",
                "Micro-benchmark: managed lock vs Interlocked.Increment.\n" +
                "Both fix the race condition, but they pay for it very differently.\n" +
                "A 'lock' involves the Monitor, some bookkeeping, and potentially parking threads.\n" +
                "Interlocked.Increment is usually a single CPU instruction.\n" +
                "For 'I just want to count something' scenarios, Interlocked can be an order of magnitude faster.",
                () => Wrap(PerformanceExamples.LockVsInterlockedPerformance)),
            new("SpinLock vs lock", nameof(PerformanceExamples.SpinLockVsLockPerformance),
                "PerformanceExamples.cs",
                "Contention micro-benchmark: SpinLock vs the lock keyword.",
                () => Wrap(PerformanceExamples.SpinLockVsLockPerformance)),
            new("Task vs Thread", nameof(PerformanceExamples.TaskVsThreadPerformance),
                "PerformanceExamples.cs",
                "Creation/scheduling overhead of Task vs raw Thread.",
                PerformanceExamples.TaskVsThreadPerformance),
            new("Concurrent Collections", nameof(PerformanceExamples.ConcurrentCollectionsPerformance),
                "PerformanceExamples.cs",
                "Throughput of ConcurrentBag/Queue/Dictionary under contention.",
                () => Wrap(PerformanceExamples.ConcurrentCollectionsPerformance)),
            new("Context Switching Overhead", nameof(PerformanceExamples.ContextSwitchingOverhead),
                "PerformanceExamples.cs",
                "Demonstrates the cost of excessive thread context switching.\n" +
                "A context switch happens every time the OS swaps one thread out for another on a CPU core:\n" +
                "registers are saved, caches get cold, memory pages may have to be reloaded.\n" +
                "Spawning way more threads than you have cores doesn't make things faster — it just makes the CPU spend more of its time shuffling threads around instead of doing real work.\n" +
                "This is a big part of why Task / ThreadPool (which right-sizes itself) is preferred over hand-rolled 'one Thread per task' designs.",
                () => Wrap(PerformanceExamples.ContextSwitchingOverhead)),
        }),
        new("Advanced Patterns", "🧠", new List<DemoItem>
        {
            new("Producer / Consumer", nameof(AdvancedExamples.ProducerConsumerExample),
                "AdvancedExamples.cs",
                "BlockingCollection-based producer/consumer pipeline.\n" +
                "The BlockingCollection<T>.GetConsumingEnumerable method allows the consumer to process items as they become available.\n" +
                "It is essentially a subscription model where the consumer reacts to new items being added.\n" +
                "The producer in this example is simply throwing items into the collection, and flagging when it's done.\n" +
                "This flagging is important as it tells the consumer to stop listening for items after a certain 'ItemId' is reached.",
                () => Wrap(AdvancedExamples.ProducerConsumerExample)),
            new("Deadlock Avoidance", nameof(AdvancedExamples.DeadlockAvoidanceExample),
                "AdvancedExamples.cs",
                "Orders lock acquisition to prevent a classic deadlock.\n" +
                "This example demonstrates how to avoid deadlocks by acquiring locks in a consistent order.\n" +
                "Two locks are instantiated, then in each of the thread setups, the execution is ordered.\n" +
                "For each of the threads, there is a lock body for lock 1. then WITHIN that lock body, lock 2 is acquired.\n" +
                "Had we used thread1 to lock with lock1 then lock2, and thread2 to lock with lock2 then lock1, a deadlock would occur.",
                () => Wrap(AdvancedExamples.DeadlockAvoidanceExample)),
            new("Monitor.TryEnter", nameof(AdvancedExamples.MonitorTryEnterExample),
                "AdvancedExamples.cs",
                "Attempts to acquire a lock with a timeout using Monitor.TryEnter.\n" +
                "Monitor.TryEnter Attempts, for the specified amount of time, to acquire an exclusive lock on the specified object\n" +
                "It then atomically sets a value that indicates whether the lock was taken.",
                () => Wrap(AdvancedExamples.MonitorTryEnterExample)),
            new("Async Coordination", nameof(AdvancedExamples.AsyncCoordinationExample),
                "AdvancedExamples.cs",
                "Coordinates async work with TaskCompletionSource / SemaphoreSlim.\n" +
                "TaskCompletionSource allows you to create a task that can be manually completed, while SemaphoreSlim provides a lightweight synchronization mechanism.",
                AdvancedExamples.AsyncCoordinationExample),
            new("Barrier", nameof(AdvancedExamples.BarrierExample), "AdvancedExamples.cs",
                "Uses Barrier to synchronize multiple threads at phase boundaries.\n" +
                "Barriers in C# allow multiple threads to wait for each other at a certain point before continuing.\n" +
                "In our case we have instantiated a barrier object with 3 as the participant count, and for each of the run barriers, the thread will increment a shared counter.\n" +
                "This happens through an anonymous void method which simply for 1 through 3 sleeps before calling the barrier.SignalAndWait method.\n" +
                "The SignalAndWait method signals that a participant has reached the barrier and waits for all other participants to reach the barrier before continuing.",
                () => Wrap(AdvancedExamples.BarrierExample)),
            new("CountdownEvent", nameof(AdvancedExamples.CountdownEventExample),
                "AdvancedExamples.cs",
                "Signals completion when a counter reaches zero.\n" +
                "A CountdownEvent is initialized with a number N, and threads call Signal() as they finish their piece of work.\n" +
                "Any thread that calls Wait() blocks until the internal counter hits zero — i.e. until everyone has checked in.\n" +
                "It's conceptually similar to Task.WhenAll, but works for any code that can call Signal(), not just Tasks.\n" +
                "Useful for 'wait for N independent pieces of work to all finish before proceeding' scenarios.",
                () => Wrap(AdvancedExamples.CountdownEventExample)),
            new("Channel<T>", nameof(AdvancedExamples.ChannelExample), "AdvancedExamples.cs",
                "Modern async producer/consumer with System.Threading.Channels.\n" +
                "Though it may seem that the consumer would start after the producer has written all the messages to the channel\n" +
                "That isn't quite the case. Think of the consumer as a 'Subscriber' to the channel\n" +
                "When the producer has written a message, the consumer get's notified and can start processing it immediately.",
                AdvancedExamples.ChannelExample),
            new("Deadlock (⚠ hangs!)", nameof(AdvancedExamples.DeadlockExample),
                "AdvancedExamples.cs",
                "Intentionally deadlocks two threads to show the problem. " +
                "You will need to force close the app to exit this demo.",
                () => Wrap(AdvancedExamples.DeadlockExample),
                Warning: true),
        }),
        new("Task Explanation" , "📖", new List<DemoItem>
        {
            new("Task Explanation", nameof(TaskExamples.TaskExplanation), "TaskExamples.cs",
                "A detailed explanation of how Tasks work under the hood.",
                TaskExamples.TaskExplanation),
        }),
    };

    private static Task Wrap(Action action)
    {
        action();
        return Task.CompletedTask;
    }
}
