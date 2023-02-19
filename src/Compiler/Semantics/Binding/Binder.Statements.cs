using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

internal partial class Binder : IStatementVisitor<IBoundNode<StatementNode>>
{
    IBoundNode<StatementNode> IStatementVisitor<IBoundNode<StatementNode>>.Default(StatementNode node)
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");

}