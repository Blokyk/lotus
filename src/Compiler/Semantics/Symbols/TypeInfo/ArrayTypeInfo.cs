namespace Lotus.Semantics;

public sealed class ArrayTypeInfo(TypeInfo itemType) : TypeInfo()
{
    public TypeInfo ItemType { get; } = itemType;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}