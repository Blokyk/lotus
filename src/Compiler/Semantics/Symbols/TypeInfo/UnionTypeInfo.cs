namespace Lotus.Semantics;

public sealed class UnionTypeInfo : TypeInfo
{
    // todo: this should really be an ImmutableArray
    public List<TypeInfo> Types { get; }

    public UnionTypeInfo(IEnumerable<TypeInfo> types, SemanticUnit unit) : base(unit)
        => Types = [..types];

    public UnionTypeInfo(List<TypeInfo> types, SemanticUnit unit) : base(unit)
        => Types = [..types];

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}