namespace Lotus.Semantics;

public class ParameterInfo(string name, TypeInfo type) : SymbolInfo
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;

    public override T Accept<T>(ISymbolVisitor<T> visitor) => visitor.Visit(this);
}