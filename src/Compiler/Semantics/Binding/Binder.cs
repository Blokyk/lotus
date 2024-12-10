using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed partial class Binder
{
    public static readonly Binder Empty = new();

    private Binder? _parent;
    private Scope _scope = Scope.Empty;

    public Binder() {}
    public Binder(Binder parent) => _parent = parent;

    private IBoundNode<T> NoBinderFor<T>(T node) where T : Node
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");
    private BoundExpression NoBinderFor(ValueNode node)
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");
}