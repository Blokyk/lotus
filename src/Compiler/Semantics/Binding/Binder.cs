using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed partial class Binder
{
    public static readonly Binder Empty = new();

    private Binder? _parent;
    private Scope _scope = new();

    private Binder() {}
    private Binder(Binder parent) => _parent = parent;

    public IBoundNode<TopLevelNode> BindNode(TopLevelNode node)
        => node.Accept(this);
    public IBoundNode<StatementNode> BindNode(StatementNode node)
        => node.Accept(this);
    public BoundExpression BindNode(ValueNode node)
        => node.Accept(this);
    public BoundExpression BindName(NameNode node)
        => node.Accept(this);

    private IBoundNode<T> NoBinderFor<T>(T node) where T : Node
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");
    private BoundExpression NoBinderFor(ValueNode node)
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");
}