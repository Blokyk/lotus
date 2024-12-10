using System.IO;
using Lotus.Semantics;
using Lotus.Syntax;

namespace Lotus.Error;

#pragma warning disable CA2211 // Non-constant field should not be visible
public static class Logger
{
    public static Stack<LotusError> errorStack = new();

    public static int ErrorCount => errorStack.Count;

    public static bool HasErrors => errorStack.Count != 0;

    public static Dictionary<string, ISourceCodeProvider> providers = new();

    public static void RegisterSourceProvider(ISourceCodeProvider prov) {
        // make sure we're not registering a provider for a non-existant file
        Debug.Assert(prov.Filename != LocationRange.NULL.filename);

        var success = providers.TryAdd(prov.Filename, prov);

        Debug.Assert(success);
    }

    public static void Log(string message, LocationRange location)
        => Console.WriteLine($"{location}: {message}");

    public static void Warning(string message, LocationRange location)
        => Console.Error.WriteLine(location + ": " + message);

    public static void Warning(LotusError e)
        => Console.Error.WriteLine("WARNING: " + Format(e));

    public static void Error(LotusError e)
        => errorStack.Push(e);

    public static Exception Fatal(Exception e) {
        if (ErrorCount > 0)
            PrintAllErrors();

        return e; // todo(log-format): Do fancy stuff with method (like pretty-printing the exception)
    }

    public static void PrintAllErrors() {
        var sb = new MarkupBuilder();

        var orderedErrorStack = errorStack.Reverse()/*.OrderBy(e => {
            if (e is ILocalized el) return el.Location;
            else return LocationRange.NULL;
        })*/;

        // todo(logging): we could optimize this function to call Format() in parallel and then Thread.WaitAll() to then print them
        // Problems :
        //      - They won't be in order anymore (because we'll have to use a ConcurrentBag)
        foreach (var error in orderedErrorStack) {
            sb.AppendLine();

            var errorTypeString =
                  error.GetType().GetDisplayName()
#if DEBUG
                + " @ "
                + GetCallerString(error)
#endif
                ;

            // todo(log-format): Why not try to center the names when there's multiple exceptions ?
            var frontCharCount = Console.WindowWidth - errorTypeString.Length;

            if (frontCharCount > 2) {
                sb.PushTextFormat(TextFormat.Faint);
                var backCharCount = Console.WindowWidth / 5;

                var backChars = "";

                if (frontCharCount > backCharCount) {
                    backChars = " " + new string('-', backCharCount);

                    sb.Append(new string('-', frontCharCount - 2 - backCharCount));
                }

                sb.Append(" " + errorTypeString,
                    new Style(
                        Foreground: TextColor.Red,
                        Format: TextFormat.Reset
                    )
                );

                sb.AppendLine(backChars + "\n");

                sb.PopTextFormat();
            }

            sb.Append(Format(error));
        }

        sb.PushTextFormat(TextFormat.Bold | TextFormat.Italic | TextFormat.Underline);

        if (ErrorCount == 1) {
            sb.AppendLine("There was an error, please fix it before proceeding.");
        } else if (ErrorCount > 1) {
            sb.Append("There were ");
            sb.Append(ErrorCount, TextColor.Blue);
            sb.AppendLine(" build errors. Fix them before proceeding.");
        }

        sb.PopTextFormat();

        Console.Error.Write(TerminalCompliantStringOf(sb));
    }

    private static string TerminalCompliantStringOf(MarkupBuilder sb) {
        // don't apply any formatting if stdout is redirected or use asked
        // for NO_COLOR
#pragma warning disable RCS1001 // Add braces to if statement
        if (Console.IsErrorRedirected
        ||  Environment.GetEnvironmentVariable("NO_COLOR") is not null and not "")
            return sb.ToString();
#pragma warning restore RCS1001

        return sb.Render();
    }

    public static string FormatError(LotusError error)
        => TerminalCompliantStringOf(Format(error));

    internal static MarkupBuilder Format(LotusError error) {
        var markupBuilder = new MarkupBuilder();

        switch (error) {
            case UnexpectedError eUnx:
                markupBuilder.AppendLine(FormatUnexpected(eUnx));
                break;
            case DuplicateSymbol eDup:
                markupBuilder.AppendLine(FormatDuplicateSymbol(eDup));
                break;
            case UnknownSymbol eUnk:
                markupBuilder.Append(FormatUnknownSymbol(eUnk));
                break;
            case UnexpectedSymbolKind eUnxKind:
                markupBuilder.Append(FormatUnexpectedKind(eUnxKind));
                break;
            default:
                markupBuilder.AppendLine(FormatGeneric(error));
                break;
        }

        if (error.Message != null) {
            markupBuilder.AppendLine().AppendLine(error.Message);
        }

        if (error.ExtraNotes != null) {
            markupBuilder.Append("\nNote: ").AppendLine(error.ExtraNotes);
        }

        markupBuilder.AppendLine();

        return markupBuilder;
    }

    internal static MarkupBuilder FormatGeneric(LotusError error) {
        var sb = new MarkupBuilder();

        if (error is IContextualized eCtx)
            sb.AppendLine(FormatContextualized(eCtx));

        if (error is ILocalized eLoc)
            sb.AppendLine(FormatLocalized(eLoc));

        return sb;
    }

    internal static MarkupBuilder FormatLocalized(ILocalized localizedErr) {
        var sb = new MarkupBuilder();

        var errorLoc = localizedErr.Location;

        // todo(logging): Allow passing a short message to be displayed in the sample
        if (TryGetProvider(errorLoc, out var scp))
            sb.AppendLine(FormatLocation(errorLoc, scp.Source));
        else if (localizedErr is LotusError error && TryGetSourceCodeFromError(error, out var src))
            sb.AppendLine(FormatLocation(errorLoc, src));
        else
            sb
                .Append("Error happened at location ")
                .Append(errorLoc)
                .Append(", but unable to display the source code.");

        sb.AppendLine();

        return sb;
    }

    internal static MarkupBuilder FormatUnexpected(UnexpectedError error) {
        var sb = new MarkupBuilder("Unexpected ");

        switch (error) {
            case IValued<Token> unxToken:
                var token = unxToken.Value;

                if (token.Kind is TokenKind.EOF) {
                    sb.Append("end of file");
                    break;
                }

                if (token is not OperatorToken opToken) {
                    sb.Append(token.Kind).Append(" '").Append(ASTUtils.PrintToken(token, false).Trim()).Append('\'');

                    break;
                }

                var opKind = SyntaxFacts.GetExpressionKind(opToken);

                switch (opKind) {
                    case ExpressionKind.Array:
                        sb.Append("array access operator");
                        break;
                    case ExpressionKind.LeftParen:
                        sb.Append("left parenthesis");
                        break;
                    case ExpressionKind.Eq:
                        sb.Append("equality operator");
                        break;
                    case ExpressionKind.NotEq:
                        sb.Append("non-equality operator");
                        break;
                    case ExpressionKind.Less:
                        sb.Append("less-than operator");
                        break;
                    case ExpressionKind.LessOrEq:
                        sb.Append("less-or-equal operator");
                        break;
                    case ExpressionKind.Greater:
                        sb.Append("greater-than operator");
                        break;
                    case ExpressionKind.GreaterOrEq:
                        sb.Append("greater-or-equal operator");
                        break;
                    case ExpressionKind.Or:
                    case ExpressionKind.And:
                    case ExpressionKind.Xor:
                    case ExpressionKind.Not:
                        sb.Append("boolean ").Append(opKind.ToString().ToLowerInvariant());
                        break;
                    default:
                        sb.Append(opKind.ToString().ToLowerInvariant()).Append(" operator");
                        break;
                }

                sb
                    .Append(" '")
                    .Append(opToken.Representation)
                    .Append('\'');

                break;
            case IValued<IEnumerable<Token>> unxTokens: // todo(logging): handle IValued<Node[]> as well
                var tokens = unxTokens.Value;

                sb
                    .Append("tokens '")
                    .Append(MiscUtils.Join(" ", t => ASTUtils.PrintToken(t, false), tokens).Trim())
                    .Append('\'');

                break;
            case IValued<Node> unxNode:
                var node = unxNode.Value;

                if (node is ValueNode.Dummy or StatementNode.Dummy or TopLevelNode.Dummy) {
                    return FormatUnexpected(
                        new UnexpectedError<Token>(
                            error.Area,
                            caller: error.Caller,
                            callerPath: error.CallerPath
                        ) {
                            Value = node.Token ?? Token.NULL,
                            As = error.As,
                            In = error.In,
                            Location = error.Location,
                            Expected = error.Expected,
                            Message = error.Message,
                        }
                    );
                }

                if (node is StatementExpressionNode stmtExprNode) {
                    return FormatUnexpected(
                        new UnexpectedError<ValueNode>(
                            error.Area,
                            caller: error.Caller,
                            callerPath: error.CallerPath
                        ) {
                            Value = stmtExprNode.Value,
                            As = error.As,
                            In = error.In,
                            Location = error.Location,
                            Expected = error.Expected,
                            Message = error.Message,
                        }
                    );
                }

                if (node is not OperationNode opNode) {
                    sb.Append(node.GetType().Name);
                } else {
                    sb
                        .Append(opNode.OperationType)
                        .Append(" (")
                        .Append(opNode.Token)
                        .Append(')');
                }

                if (node.Location.LineLength == 1)
                    sb.Append(" '").Append(ASTUtils.PrintNode(node, false).Trim()).Append('\"');

                break;
            case IValued<string> unxString:
                sb.Append("input '" + unxString.Value.Trim() + "'");
                break;
            case IValued<char> unxChar:
                var chr = unxChar.Value;
                if (chr is '\n' or '\r') {
                    sb.Append("newline character");
                } else if (chr == '\t') {
                    sb.Append("tabulation character");
                } else if (Char.IsWhiteSpace(chr)) {
                    sb.Append("whitespace character '" + System.Text.RegularExpressions.Regex.Escape(chr.ToString()));
                } else {
                    sb.Append("character '" + chr + "'");
                }
                break;
            default:
                sb.Append(String.Join(", ", error.GetType().GenericTypeArguments.Select(t => t.GetDisplayName())));
                break;
        }

        if (error.In is not null)
            sb.Append(" in " + error.In);

        if (error.As is not null)
            sb.Append(" as " + error.As);

        _ = error.Expected.Match(
            s => sb.Append("\nExpected " + s + "."),
            list => sb.Append("\nExpected one of:\n\t- " + String.Join("\n\t- ", list)),
            _ => sb
        );

        sb.AppendLine().AppendLine(FormatLocalized(error));

        return sb;
    }

    internal static MarkupBuilder FormatDuplicateSymbol(DuplicateSymbol dupError) {
        var sb = new MarkupBuilder();

        var targetName
            = dupError.TargetSymbol is INamedSymbol { Name: var name }
            ? name
            : dupError.TargetSymbol.ToString();

        sb.Append($"Symbol '{targetName}'");

        if (dupError.ContainingSymbol is INamedSymbol { Name: var containerName }) {
            sb.AppendLine($" is a duplicate inside of '{containerName}'.");
        } else {
            sb.AppendLine(" is a duplicate.");
        }

        sb
            .AppendLine()
            .AppendLine(FormatLocation(dupError.Location));

        if (dupError.ExistingSymbol is ILocalized { Location: var existingLocation }) {
            if (TryGetProvider(existingLocation, out var scp)) {
                sb.AppendLine();
                sb.PushForeground(TextColor.Green);
                sb.AppendLine($"Hint: '{targetName}' conflicts with symbol from here: ");
                // fixme: for some reason, the coloring doesn't apply here
                sb.Append(FormatLocation(existingLocation, scp.Source));
                sb.PopBackground();
            }
        }

        return sb;
    }

    private static string JoinSymbolKinds(ImmutableArray<string> kinds) {
        switch (kinds) {
            case []:
                throw new InvalidOperationException("we shouldn't have an empty list of expected kinds...");
            case [var singleKind]:
                return "a " + singleKind;
            case [..var firstFewKinds, var lastKind]:
                return "a "
                  + String.Join(", a ", firstFewKinds)
                  + ", or a "
                  + lastKind;
        }
    }

    internal static MarkupBuilder FormatUnknownSymbol(UnknownSymbol unkError) {
        var sb = new MarkupBuilder();

        sb
            .Append("Couldn't find any symbol named '")
            .Append(unkError.SymbolName)
            .Append("'");

        if (unkError.ContainingSymbol is INamedSymbol { Name: var containerName })
            sb.Append(" in symbol '").Append(containerName);
        else
            sb.Append(" in current scope");

        sb.Append(". Expected ")
          .Append(JoinSymbolKinds(unkError.ExpectedKinds))
          .AppendLine('.')
          .Append(FormatLocation(unkError.Location));

        return sb;
    }

    internal static MarkupBuilder FormatUnexpectedKind(UnexpectedSymbolKind unxEror) {
        var sb = new MarkupBuilder();

        sb.Append("Expected ")
          .Append(JoinSymbolKinds(unxEror.ExpectedKinds))
          .Append(", but got a ")
          .Append(SymbolUtils.GetKindString(unxEror.TargetSymbol))
          .Append(" name.")
          .AppendLine();

        sb.Append(FormatLocation(unxEror.Location));

        return sb;
    }

    internal static MarkupBuilder FormatContextualized(IContextualized error)
        => new("Error happened in " + error.In + "\n");

    internal static MarkupBuilder FormatLocation(LocationRange location)
        => FormatLocation(location, TryGetProvider(location, out var scp) ? scp.Source : null);

    internal static MarkupBuilder FormatLocation(LocationRange location, SourceCode? src) {
        var sb = new MarkupBuilder();
        var fileInfo = new FileInfo(location.filename);

        var relPath = "";

        if (fileInfo.Exists)
            relPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), fileInfo.FullName);

        // todo(log-format): Ideas for formatting the filename :
        //      - Underline the path and put a '@' prefix
        //      - Use the '-->' prefix
        //      - Put in bold

        sb.PushTextFormat(TextFormat.Bold);
        sb.Append($"\t --> @{relPath}({location.firstLine}:{location.firstColumn}");
        if (!location.IsSingleLocation()) {
            sb.Append($" ~ {location.lastLine}:{location.lastColumn}");
        }
        sb.AppendLine(")");
        sb.PopTextFormat();

        sb.PushForeground(TextColor.Blue);

        if (src is not null)
            sb.Append(FormatTextAt(location, src));

        sb.PopForeground();

        return sb;
    }

    private static bool TryGetProvider(LocationRange location, [NotNullWhen(true)] out ISourceCodeProvider? scp) {
        if (providers.TryGetValue(location.filename, out scp))
            return true;

        if (!File.Exists(location.filename))
            return false;

        var src = new SourceCode(File.ReadAllLines(location.filename));

        scp = new SourceCodeWrapper(location.filename, src);
        RegisterSourceProvider(scp);
        return true;
    }

    private static bool TryGetSourceCodeFromError(LotusError error, [NotNullWhen(true)] out SourceCode? src) {
        var srcText = error switch {
            IValued<Node?> { Value: not null } eNode => ASTUtils.PrintNode(eNode.Value),
            IValued<Token?> { Value: not null } eToken => ASTUtils.PrintToken(eToken.Value),
            IValued<string> eString => eString.Value,
            _ => null,
        };

        if (srcText is null) {
            src = default;
            return false;
        }

        src = new(srcText);
        return true;
    }

    private static string FormatTextAt(LocationRange location, in LotusError error) {
        if (TryGetProvider(location, out var scp))
            return FormatTextAt(location, scp.Source);

        if (TryGetSourceCodeFromError(error, out var src)) {
            var newLoc
                = new LocationRange(
                    firstLine: 1,
                    lastLine: src.RawLines.Length,
                    firstColumn: 1,
                    lastColumn: Int32.MaxValue,
                    filename: location.filename
                );

            return FormatTextAt(location, src)
                + "\nWARNING : This source code is approximated, because no source code could be found for " + location.filename
                ;
        }

        return "";
    }

    private static string FormatTextAt(LocationRange range, SourceCode source) {
        if (range.LineLength == 1) {
            if (range.ColumnLength == 1)
                return source.FormatTextAtPoint(range.GetFirstLocation());
            else
                return source.FormatTextAtLine(range);
        }

        return source.FormatTextAtLines(range);
    }

    private static string GetCallerString(LotusError error) => Path.GetFileNameWithoutExtension(error.CallerPath) + '.' + error.Caller;

    internal class SourceCodeWrapper : ISourceCodeProvider {
        public string Filename { get; }
        public SourceCode Source { get; }

        public SourceCodeWrapper(string filename, SourceCode src) {
            Filename = filename;
            Source = src;
        }
    }
}