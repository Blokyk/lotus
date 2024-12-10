namespace Lotus.Semantics;

public class ParameterInfo(string name, TypeInfo type)
    : SymbolInfo
    , INamedSymbol
    , ILocalized
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;

    public LocationRange Location { get; init; }

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}