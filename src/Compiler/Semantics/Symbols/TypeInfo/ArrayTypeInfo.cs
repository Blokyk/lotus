namespace Lotus.Semantics;

public sealed class ArrayTypeInfo(TypeInfo itemType) : TypeInfo()
{
    public TypeInfo ItemType { get; } = itemType;
}