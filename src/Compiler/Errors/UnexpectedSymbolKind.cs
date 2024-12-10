using Lotus.Semantics;
using System.Runtime.CompilerServices;

namespace Lotus.Error;

public class UnexpectedSymbolKind : LotusError, ILocalized, IContextualized
{
    public required LocationRange Location { get; init; }

    public string? In { get; init; }

    public required ImmutableArray<string> ExpectedKinds { get; init; } = [];
    public required SymbolInfo TargetSymbol { get; init; }

    public UnexpectedSymbolKind(ErrorArea area = ErrorArea.Binder, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller, callerPath) {}
}