using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal abstract class BoundExpression : IBoundNode<ValueNode>
{
    public ValueNode BackingNode { get; }

    public TypeInfo Type { get; set; } = BuiltinTypes.Unknown;

    protected BoundExpression(ValueNode node) {
        BackingNode = node;
    }

    protected BoundExpression(ValueNode node, TypeInfo type) : this(node) {
        Type = type;
    }
}
