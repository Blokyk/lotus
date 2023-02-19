using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

internal partial class Binder : ITopLevelVisitor<IBoundNode<TopLevelNode>>
{
    IBoundNode<TopLevelNode> ITopLevelVisitor<IBoundNode<TopLevelNode>>.Default(TopLevelNode node)
        => throw new NotImplementedException("No binder found for " + node.GetType().Name + "s");

    IBoundNode<TopLevelNode> Visit(TypeDecName name)
        => throw new NotImplementedException("can't bind TypeDecName");
}