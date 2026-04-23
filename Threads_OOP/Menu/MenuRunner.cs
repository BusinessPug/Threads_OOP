using Spectre.Console;

namespace Threads_OOP.Menu;

internal static class MenuRunner
{
    private const string BackLabel = "← Back";
    private const string ExitLabel = "✖ Exit";

    public static async Task RunAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            RenderHeader();

            List<string> choices = new();
            foreach (DemoCategory category in DemoCatalog.Categories)
            {
                choices.Add($"{category.Icon}  {category.Name}  [grey]({category.Items.Count} demos)[/]");
            }
            choices.Add(ExitLabel);

            string selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Choose a category:[/]")
                    .PageSize(15)
                    .HighlightStyle(new Style(foreground: Color.Black, background: Color.Aqua))
                    .AddChoices(choices)
                    .UseConverter(s => s));

            if (selection == ExitLabel)
            {
                AnsiConsole.MarkupLine("\n[green]Goodbye! 👋[/]");
                return;
            }

            int index = choices.IndexOf(selection);
            await RunCategoryAsync(DemoCatalog.Categories[index]);
        }
    }

    private static async Task RunCategoryAsync(DemoCategory category)
    {
        while (true)
        {
            AnsiConsole.Clear();
            RenderHeader();
            AnsiConsole.Write(new Rule($"[bold aqua]{category.Icon}  {category.Name}[/]")
            {
                Justification = Justify.Left,
            });
            AnsiConsole.WriteLine();

            List<string> choices = category.Items
                .Select(item => item.Warning
                    ? $"[red]⚠[/]  {item.Name}"
                    : $"   {item.Name}")
                .ToList();
            choices.Add(BackLabel);

            string selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Choose a demo:[/]")
                    .PageSize(15)
                    .HighlightStyle(new Style(foreground: Color.Black, background: Color.Aqua))
                    .AddChoices(choices));

            if (selection == BackLabel)
            {
                return;
            }

            int index = choices.IndexOf(selection);
            await RunDemoAsync(category, category.Items[index]);
        }
    }

    private static async Task RunDemoAsync(DemoCategory category, DemoItem demo)
    {
        AnsiConsole.Clear();
        RenderHeader();

        AnsiConsole.Write(new Rule($"[bold aqua]{category.Icon} {category.Name} → {demo.Name}[/]")
        {
            Justification = Justify.Left,
        });
        AnsiConsole.WriteLine();

        // Description panel.
        Panel description = new(new Markup($"[white]{Markup.Escape(demo.Description)}[/]"))
        {
            Header = new PanelHeader(" 📖 What this demo shows ", Justify.Left),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(foreground: Color.Aqua),
        };
        AnsiConsole.Write(description);
        AnsiConsole.WriteLine();

        // Source code panel with lightweight C# syntax highlighting.
        string source = SourceProvider.GetMethodSource(demo.SourceFile, demo.MethodName);
        string highlighted = CSharpSyntaxHighlighter.Highlight(source);
        Panel codePanel = new(new Markup(highlighted))
        {
            Header = new PanelHeader($" 💻 {demo.SourceFile} → {demo.MethodName} ", Justify.Left),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(foreground: Color.Yellow),
        };
        AnsiConsole.Write(codePanel);
        AnsiConsole.WriteLine();

        if (demo.Warning)
        {
            AnsiConsole.MarkupLine(
                "[bold red on yellow] ⚠ WARNING [/] [red]This demo intentionally deadlocks. " +
                "You may need to force close the app to recover.[/]");
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine("[bold green]Press any key to run the example...[/]");
        Console.ReadKey(true);

        AnsiConsole.Write(new Rule("[bold green]▶ Running demo[/]")
        {
            Justification = Justify.Left,
        });
        AnsiConsole.WriteLine();

        try
        {
            await demo.Runner();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold green]✔ Demo finished[/]")
        {
            Justification = Justify.Left,
        });
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
        Console.ReadKey(true);
    }

    private static void RenderHeader()
    {
        FigletText title = new FigletText("Threads OOP")
            .LeftJustified()
            .Color(Color.Aqua);
        AnsiConsole.Write(title);
        AnsiConsole.MarkupLine("[grey]C# Threading & Concurrency — Interactive Demos[/]");
        AnsiConsole.WriteLine();
    }
}
