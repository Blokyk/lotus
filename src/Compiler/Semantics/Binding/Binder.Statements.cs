using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

partial class Binder : IStatementVisitor<IBoundNode<StatementNode>>
{
    IBoundNode<StatementNode> IStatementVisitor<IBoundNode<StatementNode>>.Default(StatementNode node)
        => NoBinderFor(node);
}