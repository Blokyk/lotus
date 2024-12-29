namespace Lotus.Semantics;

public abstract class TraitImplInfo(LocationRange loc, SemanticUnit unit)
    : SymbolInfo(unit)
    , IMemberSymbol<NamespaceInfo>
    , ILocalized
{
    public LocationRange Location => loc;

    public NamespaceInfo? ContainingNamespace { get; set; }
    NamespaceInfo IMemberSymbol<NamespaceInfo>.ContainingSymbol => ContainingNamespace;
}