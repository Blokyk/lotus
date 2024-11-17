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

    public override T Accept<T>(ISymbolVisitor<T> visitor) => visitor.Visit(this);
}