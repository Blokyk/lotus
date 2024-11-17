using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

partial class Binder : ITopLevelVisitor<IBoundNode<TopLevelNode>>
{
    IBoundNode<TopLevelNode> ITopLevelVisitor<IBoundNode<TopLevelNode>>.Default(TopLevelNode node)
        => NoBinderFor(node);

    IBoundNode<TopLevelNode> Visit(TypeDecName name)
        => throw new NotImplementedException("can't bind TypeDecName");
}