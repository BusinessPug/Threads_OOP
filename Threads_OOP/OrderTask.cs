namespace Threads_OOP;

internal static class OrderTask
{
    public async static Task HandleOrder(string orderName)
    {
        Console.WriteLine($"Order {orderName} is being processed in thread: {Thread.CurrentThread.ManagedThreadId}");
        await Task.Delay(1000);
        Console.WriteLine($"Order {orderName} has been processed in thread: {Thread.CurrentThread.ManagedThreadId}");
    }
}
