using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class BinaryOperation(
    OperationNode opNode,
    Expression left,
    Expression right,
    FunctionInfo backingMethod
) : Expression(
    opNode,
    backingMethod.ReturnType
) {
    public Expression Left => left;
    public Expression Right => right;

    public new OperationNode SyntaxNode => opNode;
    public FunctionInfo BackingMethod => backingMethod;
    public TypeInfo ResultType => BackingMethod.ReturnType;

}