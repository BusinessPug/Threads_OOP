using Threads_OOP.Menu;

namespace Threads_OOP;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        await MenuRunner.RunAsync();
    }
}
