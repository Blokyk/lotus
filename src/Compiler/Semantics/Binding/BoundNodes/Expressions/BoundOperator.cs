using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal class BoundBinaryOperator : BoundExpression
{
    public BoundExpression LeftOperand { get; }
    public BoundExpression RightOperand { get; }
    public IBoundNode<Node> BackingMethod { get; set; }
    public TypeInfo ResultType { get; set; }

    internal BoundBinaryOperator(
        OperationNode opNode,
        BoundExpression leftOperand,
        BoundExpression rightOperand,
        IBoundNode<Node> backingMethod,
        TypeInfo resultType
    )
        : base(opNode, resultType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        BackingMethod = backingMethod;
        ResultType = resultType;
    }
}