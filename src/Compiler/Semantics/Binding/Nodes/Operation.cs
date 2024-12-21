using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal class Operation(
    OperationNode opNode,
    ImmutableArray<Expression> operands,
    FunctionInfo backingMethod
) : Expression(opNode, backingMethod.ReturnType)
{
    public new OperationNode SyntaxNode => opNode;
    public ImmutableArray<Expression> Operands => operands;
    public FunctionInfo BackingMethod => backingMethod;
    public TypeInfo ResultType => BackingMethod.ReturnType;
}