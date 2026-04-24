using System.Text;

namespace Threads_OOP.Menu;

/// <summary>
/// A tiny, pragmatic C# syntax highlighter that emits Spectre.Console markup.
/// It handles: line/block comments, strings (including verbatim and interpolated),
/// char literals, numeric literals, keywords, and a small set of known BCL type names.
/// Good enough for displaying short demo snippets — not a full parser.
/// This was stolen from GitHub.
/// </summary>
internal static class CSharpSyntaxHighlighter
{
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "abstract","as","base","bool","break","byte","case","catch","char","checked","class",
        "const","continue","decimal","default","delegate","do","double","else","enum","event",
        "explicit","extern","false","finally","fixed","float","for","foreach","goto","if",
        "implicit","in","int","interface","internal","is","lock","long","namespace","new",
        "null","object","operator","out","override","params","private","protected","public",
        "readonly","ref","return","sbyte","sealed","short","sizeof","stackalloc","static",
        "string","struct","switch","this","throw","true","try","typeof","uint","ulong",
        "unchecked","unsafe","ushort","using","virtual","void","volatile","while",
        "async","await","var","yield","nameof","record","init","dynamic","global","when",
        "get","set","value","partial","where","from","select","group","into","orderby",
        "join","let","on","equals","by","ascending","descending","with","and","or","not"
    };

    private static readonly HashSet<string> KnownTypes = new(StringComparer.Ordinal)
    {
        "Task","Task`1","ValueTask","Thread","ThreadPool","Parallel","ParallelOptions",
        "ParallelLoopState","ParallelLoopResult","CancellationToken","CancellationTokenSource",
        "Console","Enumerable","List","Dictionary","HashSet","Queue","Stack","IEnumerable",
        "IReadOnlyList","IList","Func","Action","Exception","OperationCanceledException",
        "Stopwatch","Interlocked","SpinLock","SpinWait","Monitor","Mutex","Semaphore",
        "SemaphoreSlim","ReaderWriterLockSlim","Barrier","CountdownEvent","ManualResetEvent",
        "ManualResetEventSlim","AutoResetEvent","BlockingCollection","ConcurrentBag",
        "ConcurrentQueue","ConcurrentStack","ConcurrentDictionary","Channel","ChannelReader",
        "ChannelWriter","TransformBlock","ActionBlock","BufferBlock","BroadcastBlock",
        "ExecutionDataflowBlockOptions","DataflowLinkOptions","Partitioner","StringBuilder",
        "DateTime","TimeSpan","Guid","Random","Environment","TaskCompletionSource",
        "Lazy","Nullable","Span","ReadOnlySpan","Memory","ReadOnlyMemory","Array","String",
        "Int32","Int64","Double","Boolean","Object","IDisposable"
    };

    // Spectre colors (named, so themes still work on dark terminals).
    private const string KeywordColor  = "deepskyblue1";
    private const string TypeColor     = "mediumturquoise";
    private const string StringColor   = "lightsalmon1";
    private const string NumberColor   = "palegreen1";
    private const string CommentColor  = "grey50";
    private const string OperatorColor = "grey85";
    private const string TextColor     = "grey93";

    public static string Highlight(string code)
    {
        StringBuilder sb = new(code.Length * 2);
        int i = 0;

        while (i < code.Length)
        {
            char c = code[i];

            // Line comment
            if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                int start = i;
                while (i < code.Length && code[i] != '\n') i++;
                AppendColored(sb, CommentColor, code[start..i]);
                continue;
            }

            // Block comment
            if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
            {
                int start = i;
                i += 2;
                while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == '/')) i++;
                i = Math.Min(code.Length, i + 2);
                AppendColored(sb, CommentColor, code[start..i]);
                continue;
            }

            // Verbatim / interpolated string: @"...", $"...", $@"...", @$"..."
            if ((c == '@' || c == '$') && i + 1 < code.Length)
            {
                int look = i;
                bool verbatim = false, interp = false;
                while (look < code.Length && (code[look] == '@' || code[look] == '$'))
                {
                    if (code[look] == '@') verbatim = true;
                    if (code[look] == '$') interp = true;
                    look++;
                }
                if (look < code.Length && code[look] == '"')
                {
                    int start = i;
                    i = look + 1;
                    int braceDepth = 0;
                    while (i < code.Length)
                    {
                        char cc = code[i];
                        if (interp && cc == '{' && i + 1 < code.Length && code[i + 1] == '{') { i += 2; continue; }
                        if (interp && cc == '}' && i + 1 < code.Length && code[i + 1] == '}') { i += 2; continue; }
                        if (interp && cc == '{') { braceDepth++; i++; continue; }
                        if (interp && cc == '}' && braceDepth > 0) { braceDepth--; i++; continue; }
                        if (braceDepth == 0 && cc == '"')
                        {
                            if (verbatim && i + 1 < code.Length && code[i + 1] == '"') { i += 2; continue; }
                            i++;
                            break;
                        }
                        if (!verbatim && cc == '\\' && i + 1 < code.Length) { i += 2; continue; }
                        i++;
                    }
                    AppendColored(sb, StringColor, code[start..i]);
                    continue;
                }
            }

            // Regular string
            if (c == '"')
            {
                int start = i++;
                while (i < code.Length && code[i] != '"')
                {
                    if (code[i] == '\\' && i + 1 < code.Length) { i += 2; continue; }
                    if (code[i] == '\n') break;
                    i++;
                }
                if (i < code.Length) i++;
                AppendColored(sb, StringColor, code[start..i]);
                continue;
            }

            // Char literal
            if (c == '\'')
            {
                int start = i++;
                while (i < code.Length && code[i] != '\'')
                {
                    if (code[i] == '\\' && i + 1 < code.Length) { i += 2; continue; }
                    if (code[i] == '\n') break;
                    i++;
                }
                if (i < code.Length) i++;
                AppendColored(sb, StringColor, code[start..i]);
                continue;
            }

            // Number literal
            if (char.IsDigit(c))
            {
                int start = i;
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '.' || code[i] == '_'))
                {
                    i++;
                }
                AppendColored(sb, NumberColor, code[start..i]);
                continue;
            }

            // Identifier / keyword / type
            if (c == '_' || char.IsLetter(c))
            {
                int start = i;
                while (i < code.Length && (code[i] == '_' || char.IsLetterOrDigit(code[i])))
                {
                    i++;
                }

                string token = code[start..i];
                if (Keywords.Contains(token))
                {
                    AppendColored(sb, KeywordColor, token);
                }
                else if (KnownTypes.Contains(token) || LooksLikePascalType(token))
                {
                    AppendColored(sb, TypeColor, token);
                }
                else
                {
                    AppendColored(sb, TextColor, token);
                }
                continue;
            }

            // Operators / punctuation
            if (IsOperatorChar(c))
            {
                int start = i;
                while (i < code.Length && IsOperatorChar(code[i])) i++;
                AppendColored(sb, OperatorColor, code[start..i]);
                continue;
            }

            // Whitespace & everything else — emit as-is (escaped) without color markup.
            sb.Append(Spectre.Console.Markup.Escape(c.ToString()));
            i++;
        }

        return sb.ToString();
    }

    private static void AppendColored(StringBuilder sb, string color, string text)
    {
        sb.Append('[').Append(color).Append(']')
          .Append(Spectre.Console.Markup.Escape(text))
          .Append("[/]");
    }

    private static bool LooksLikePascalType(string token) =>
        token.Length > 1 && char.IsUpper(token[0]) && token.Any(char.IsLower);

    private static bool IsOperatorChar(char c) =>
        "+-*/%=<>!&|^~?:;,.()[]{}".IndexOf(c) >= 0;
}
