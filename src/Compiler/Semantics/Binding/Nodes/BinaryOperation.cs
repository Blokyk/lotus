using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal class BinaryOperation(
    OperationNode opNode,
    Expression left,
    Expression right,
    FunctionInfo backingMethod
) : Operation(
    opNode,
    [left, right],
    backingMethod
) {
    public Expression Left => left;
    public Expression Right => right;
}