using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class ErrorTypeInfo(string name, SemanticUnit unit)
    : TypeInfo(unit)
    , INamedSymbol
    , IScope
{
    public string Name => name;

    public SymbolInfo? ContainingSymbol { get; set; }

    private readonly ErrorScope _scope = new(unit);
    Scope IScope.Scope => _scope;
    private sealed class ErrorScope(SemanticUnit unit) : Scope
    {
        private Dictionary<string, ErrorSymbolInfo> _symbols = [];
        public override SymbolInfo Get(string name) {
            if (!_symbols.TryGetValue(name, out var sym)) {
                sym = new ErrorSymbolInfo(name, unit);
                _symbols.Add(name, sym);
            }

            return sym;
        }
    }

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}