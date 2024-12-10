using System.Runtime.CompilerServices;

namespace Lotus.Semantics;

public class DuplicateSymbol : LotusError, ILocalized, IContextualized
{
    public LocationRange Location
        => TargetSymbol is ILocalized { Location: var targetLoc }
        ? targetLoc
        : LocationRange.NULL;

    public string? In { get; init; }

    public SymbolInfo? ContainingSymbol { get; init; }

    public SymbolInfo? ExistingSymbol { get; init; }
    public required SymbolInfo TargetSymbol { get; init; }

    public DuplicateSymbol([CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(ErrorArea.Semantics, caller, callerPath) {}
}