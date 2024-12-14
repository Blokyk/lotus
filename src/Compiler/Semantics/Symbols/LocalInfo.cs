namespace Lotus.Semantics;

public class LocalInfo(string name, TypeInfo type, LocationRange loc, SemanticUnit unit)
    : TypedSymbolInfo(unit)
    , INamedSymbol
    , ILocalized
{
    public string Name { get; } = name;
    public override TypeInfo Type => type;
    public LocationRange Location => loc;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}