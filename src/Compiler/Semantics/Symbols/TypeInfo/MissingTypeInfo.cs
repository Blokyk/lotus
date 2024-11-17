namespace Lotus.Semantics;

public sealed class MissingTypeInfo(string name) : TypeInfo
{
    public string Name { get; } = name;

    public override T Accept<T>(ISymbolVisitor<T> visitor) => visitor.Visit(this);
}