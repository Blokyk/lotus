using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal interface IBoundNode<out T> where T : Node
{
    public T BackingNode { get; }
}
