using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

partial class Binder : IValueVisitor<BoundExpression>
{
    BoundExpression IValueVisitor<BoundExpression>.Default(ValueNode node)
        => NoBinderFor(node);

    BoundExpression IValueVisitor<BoundExpression>.Visit(BoolNode node) => new BoundLiteral(node, BuiltinTypes.Bool);
    BoundExpression IValueVisitor<BoundExpression>.Visit(StringNode node) => new BoundLiteral(node, BuiltinTypes.String);
    BoundExpression IValueVisitor<BoundExpression>.Visit(ComplexStringNode node) => new BoundLiteral(node, BuiltinTypes.String);
    BoundExpression IValueVisitor<BoundExpression>.Visit(NumberNode node) => new BoundLiteral(node, GetNumberType(node));

    // todo(binding): we don't actually have to do all of the binding stuff to get the type
    internal TypeInfo GetType(ValueNode node) => node.Accept(this).Type;

    internal static TypeInfo GetLiteralType(ValueNode node) {
        switch (node) {
            case BoolNode:
                return BuiltinTypes.Bool;
            case StringNode:
                return BuiltinTypes.String;
            case NumberNode number:
                return GetNumberType(number);
            default:
                throw new InvalidOperationException("trying to create a BoundLiteral from unsupported node type " + node.GetType());
        }
    }

    private static TypeInfo GetNumberType(NumberNode number)
        => number.Kind switch {
            NumberKind.Double => BuiltinTypes.Double,
            NumberKind.Float => BuiltinTypes.Float,
            NumberKind.Int => BuiltinTypes.Int,
            NumberKind.UInt => BuiltinTypes.UInt,
            NumberKind.Long => BuiltinTypes.Long,
            NumberKind.ULong => BuiltinTypes.ULong,
            _ => throw new InvalidOperationException("trying to bind unknown number kind " + number.Kind),
        };
}