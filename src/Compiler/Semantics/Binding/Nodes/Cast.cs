namespace Lotus.Semantics.Binding;

internal class Cast(Expression fromExpr, TypeInfo toType)
: Expression(
    fromExpr.SyntaxNode, toType
) {
    public Expression Operand => fromExpr;
    public TypeInfo FromType => Operand.Type;
    public TypeInfo ToType => this.Type; // avoids capturing toType twice
}