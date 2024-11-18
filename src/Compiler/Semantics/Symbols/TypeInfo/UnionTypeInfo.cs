namespace Lotus.Semantics;

public sealed class UnionTypeInfo : TypeInfo
{
    public List<TypeInfo> Types { get; }

    public UnionTypeInfo(IEnumerable<TypeInfo> types) {
        Types = [..types];
    }

    public UnionTypeInfo(List<TypeInfo> types) {
        Types = types;
    }
}