using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class LateBoundNode : IBoundNode<Node>
{
    public Node BackingNode { get; }
    public Binder? ResolvingBinder { get; set; }

    public LateBoundNode(Node node)
        => BackingNode = node;
}