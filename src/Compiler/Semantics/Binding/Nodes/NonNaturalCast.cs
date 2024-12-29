namespace Lotus.Semantics.Binding;

internal sealed class NonNaturalCast(
    Expression fromExpr,
    FunctionInfo backingMethod
) : Cast(fromExpr, backingMethod.ReturnType)
{
    public FunctionInfo BackingMethod => backingMethod;
}