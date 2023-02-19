using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed partial class Binder
{
    public static readonly Binder Empty = new();

    private Binder? Parent { get; }

    private Binder() {}
    private Binder(Binder parent) => Parent = parent;

    public IBoundNode<TopLevelNode> BindNode(TopLevelNode node)
        => node.Accept(this);
    public IBoundNode<StatementNode> BindNode(StatementNode node)
        => node.Accept(this);
    public BoundExpression BindNode(ValueNode node)
        => node.Accept(this);
    public BoundExpression BindName(NameNode node)
        => node.Accept(this);
}