namespace Lotus.Semantics;

public class TupleTypeInfo(ImmutableArray<TypeInfo> _types, SemanticUnit unit)
    : TypeInfo(unit)
{
    public ImmutableArray<TypeInfo> Types => _types;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}