using System.Collections.Concurrent;
using System.Reflection;

namespace Threads_OOP.Menu;

internal static class SourceProvider
{
    private static readonly ConcurrentDictionary<string, string> _fileCache = new();

    public static string GetMethodSource(string exampleFileName, string methodName)
    {
        string source = LoadFile(exampleFileName);
        if (string.IsNullOrEmpty(source))
        {
            return $"// Source for {methodName} not found.";
        }

        string[] lines = source.Replace("\r\n", "\n").Split('\n');

        // Find the line that contains the method signature.
        int startLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Contains(" " + methodName + "(") &&
                (line.Contains("public ") || line.Contains("private ") || line.Contains("internal ")))
            {
                startLine = i;
                break;
            }
        }

        if (startLine < 0)
        {
            return $"// Method {methodName} not found in {exampleFileName}.";
        }

        // Walk backwards to include XML/doc comments or attributes on preceding lines.
        int contextStart = startLine;
        while (contextStart > 0)
        {
            string prev = lines[contextStart - 1].TrimStart();
            if (prev.StartsWith("//") || prev.StartsWith("[") || prev.StartsWith("///"))
            {
                contextStart--;
            }
            else
            {
                break;
            }
        }

        // Find the opening brace of the method (may be same line or next line).
        int braceLine = startLine;
        while (braceLine < lines.Length && !lines[braceLine].Contains('{'))
        {
            braceLine++;
        }

        if (braceLine >= lines.Length)
        {
            return string.Join('\n', lines[contextStart..(startLine + 1)]);
        }

        // Track brace depth to find the matching closing brace.
        int depth = 0;
        int endLine = braceLine;
        bool started = false;
        for (int i = braceLine; i < lines.Length; i++)
        {
            foreach (char c in lines[i])
            {
                if (c == '{')
                {
                    depth++;
                    started = true;
                }
                else if (c == '}')
                {
                    depth--;
                }
            }

            if (started && depth == 0)
            {
                endLine = i;
                break;
            }
        }

        string[] slice = lines[contextStart..(endLine + 1)];
        return Dedent(slice);
    }

    private static string LoadFile(string fileName)
    {
        return _fileCache.GetOrAdd(fileName, name =>
        {
            Assembly asm = typeof(SourceProvider).Assembly;
            string? resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("." + name, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                return string.Empty;
            }

            using Stream? stream = asm.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                return string.Empty;
            }

            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        });
    }

    private static string Dedent(string[] lines)
    {
        int minIndent = int.MaxValue;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int indent = 0;
            while (indent < line.Length && line[indent] == ' ')
            {
                indent++;
            }

            if (indent < minIndent)
            {
                minIndent = indent;
            }
        }

        if (minIndent is int.MaxValue or 0)
        {
            return string.Join('\n', lines);
        }

        return string.Join('\n', lines.Select(l =>
            l.Length >= minIndent && string.IsNullOrWhiteSpace(l[..minIndent]) ? l[minIndent..] : l));
    }
}
