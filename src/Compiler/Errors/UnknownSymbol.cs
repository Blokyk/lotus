using Lotus.Semantics;
using System.Runtime.CompilerServices;

namespace Lotus.Error;

public class UnknownSymbol : LotusError, ILocalized, IContextualized
{
    public required LocationRange Location { get; init; }

    public string? In { get; init; }

    public ImmutableArray<string> ExpectedKinds { get; init; } = [];
    public required string SymbolName { get; init; }

    public SymbolInfo? ContainingSymbol { get; init; }

    public UnknownSymbol(ErrorArea area = ErrorArea.Binder, [CallerMemberName] string caller = "<unknown-caller>", [CallerFilePath] string callerPath = "")
        : base(area, caller, callerPath) {}
}