namespace Threads_OOP;

internal class PrintTask
{
    internal static async Task PrintThread()
    {
        Console.WriteLine("Running in thread: " + Thread.CurrentThread.ManagedThreadId);
        await Task.Delay(500);
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {i}");
            await Task.Delay(500);
        }
    }

    internal static void PrintThreadNoWait()
    {
        Console.WriteLine("Running in thread: " + Thread.CurrentThread.ManagedThreadId);
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {i}");
            Thread.Sleep(500);
        }
    }
}
