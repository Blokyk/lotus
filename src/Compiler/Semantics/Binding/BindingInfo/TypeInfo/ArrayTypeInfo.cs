namespace Lotus.Semantics.Binding;

internal sealed class ArrayTypeInfo : TypeInfo
{
    public TypeInfo ItemType { get; }

    public ArrayTypeInfo(TypeInfo itemType) : base() {
        ItemType = itemType;
    }
}