
namespace Lotus.Semantics;

public abstract class UserTypeInfo(string name, LocationRange loc)
    : TypeInfo
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo?>
    , ILocalized
{
    public string Name { get; } = name;

    public NamespaceInfo? ContainingNamespace { get; set; }

    public LocationRange Location { get; } = loc;

    NamespaceInfo? IMemberSymbol<NamespaceInfo?>.ContainingSymbol => ContainingNamespace;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}