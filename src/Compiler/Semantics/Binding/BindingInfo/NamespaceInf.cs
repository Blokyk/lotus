namespace Lotus.Semantics.Binding;

internal class NamespaceInfo : MemberInfo
{
    public string Name { get; }
    public List<MemberInfo> Members { get; } = new();

    public NamespaceInfo? ContainingInfo { get; set; }

    public NamespaceInfo(string name) {
        Name = name;
    }
}