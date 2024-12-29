namespace Lotus.Semantics;

public abstract class UserTypeInfo(string name, LocationRange loc, SemanticUnit unit)
    : TypeInfo(unit)
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo>
    , ILocalized
{
    public string Name { get; } = name;

    public NamespaceInfo ContainingNamespace { get; set; } = unit.Global;

    public LocationRange Location => loc;

    NamespaceInfo IMemberSymbol<NamespaceInfo>.ContainingSymbol => ContainingNamespace;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}