namespace Lotus.Semantics.Binding;

internal sealed class NamedTypeInfo : TypeInfo
{
    public string Name { get; }
    public NamespaceInfo? ContainingNamespace { get; set; }
    public NamedTypeInfo? ContainingType { get; set; }

    public bool IsStruct { get; }
    public bool IsEnum { get; }

    public NamedTypeInfo(string name) {
        Name = name;
    }
}