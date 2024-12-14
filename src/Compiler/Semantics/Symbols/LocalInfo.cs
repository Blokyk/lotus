namespace Lotus.Semantics;

public class LocalInfo(string name, TypeInfo type, LocationRange loc, SemanticUnit unit)
    : TypedSymbolInfo(unit)
    , INamedSymbol
{
    public string Name { get; } = name;
    public override TypeInfo Type => type;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}