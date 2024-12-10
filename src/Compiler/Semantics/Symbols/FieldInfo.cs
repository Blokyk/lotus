namespace Lotus.Semantics;

public class FieldInfo(string name, TypeInfo type, TypeInfo containingType)
    : SymbolInfo
    , INamedSymbol
    , IMemberSymbol<StructTypeInfo>
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;

    public TypeInfo ContainingType { get; } = containingType;
    StructTypeInfo IMemberSymbol<StructTypeInfo>.ContainingSymbol => throw new NotImplementedException();

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}