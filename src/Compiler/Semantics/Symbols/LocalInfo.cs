namespace Lotus.Semantics;

public class LocalInfo(string name, TypeInfo type)
    : SymbolInfo
    , INamedSymbol
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}