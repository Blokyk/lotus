using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class BoundLiteral : BoundExpression
{
    public object GetValue() => throw new NotImplementedException();

    public BoundLiteral(ValueNode node, TypeInfo type) : base(node, type) {}
}