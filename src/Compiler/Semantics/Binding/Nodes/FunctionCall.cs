using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class FunctionCall(
    FunctionCallNode node,
    FunctionInfo function,
    ImmutableArray<Expression> args
) : Expression(node, function.ReturnType)
{
    public FunctionInfo Function => function;
    public ImmutableArray<Expression> Args => args;
}