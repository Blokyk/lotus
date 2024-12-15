namespace Lotus.Semantics;

public class ErrorSymbolInfo(string name, SemanticUnit unit)
    : TypedSymbolInfo(unit)
    , INamedSymbol
{
    public string Name => name;

    public SymbolInfo? ContainingSymbol { get; set; }

    private readonly ErrorTypeInfo _type = new("<" + name + "$type>", unit);
    public override TypeInfo Type => _type;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}
