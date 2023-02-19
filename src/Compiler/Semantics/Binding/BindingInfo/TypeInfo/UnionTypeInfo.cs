namespace Lotus.Semantics.Binding;

internal sealed class UnionTypeInfo : TypeInfo
{
    public List<TypeInfo> Types { get; }

    public UnionTypeInfo(IEnumerable<TypeInfo> types) {
        Types = new(types);
    }

    public UnionTypeInfo(List<TypeInfo> types) {
        Types = types;
    }
}