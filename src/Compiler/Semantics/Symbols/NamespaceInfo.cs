namespace Lotus.Semantics;

public class NamespaceInfo(string name)
    : SymbolInfo
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo?>
    , IContainerSymbol<NamespaceInfo>
    , IContainerSymbol<TypeInfo>
{
    public string Name { get; } = name;

    public List<NamespaceInfo> Namespaces { get; } = [];
    IEnumerable<NamespaceInfo> IContainerSymbol<NamespaceInfo>.Children() => Namespaces;

    public List<TypeInfo> Types { get; } = [];
    IEnumerable<TypeInfo> IContainerSymbol<TypeInfo>.Children() => Types;

    public NamespaceInfo? ContainingNamespace { get; set; } = null;
    NamespaceInfo? IMemberSymbol<NamespaceInfo?>.ContainingSymbol => ContainingNamespace;
}