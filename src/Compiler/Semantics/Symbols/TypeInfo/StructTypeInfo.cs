namespace Lotus.Semantics;

public sealed class StructTypeInfo(string name)
    : TypeInfo
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo?>
    , IContainerSymbol<FieldInfo>
{
    public string Name { get; } = name;

    public NamespaceInfo? ContainingNamespace { get; set; }
    NamespaceInfo? IMemberSymbol<NamespaceInfo?>.ContainingSymbol => ContainingNamespace;

    public List<FieldInfo> Fields { get; } = [];
    IEnumerable<FieldInfo> IContainerSymbol<FieldInfo>.Children() => Fields;
}