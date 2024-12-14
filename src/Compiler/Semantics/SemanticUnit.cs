using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

// fixme: none of this handles accessibility
public partial class SemanticUnit
{
    internal NamespaceInfo Global { get; }

    private Scope GlobalScope => ((IScope)Global).Scope;

    public IEnumerable<NamespaceInfo> Namespaces => Global.Namespaces;
    public IEnumerable<TypeInfo> GlobalTypes => Global.Types;

    public bool IsValid { get; } = true;

    public override string ToString() => SemanticVisualizer.Format(Global);
}