using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

internal class Binder : IValueVisitor<Expression>
{
    private SemanticUnit _unit;
    private Scope _scope;

    public Binder(SemanticUnit unit, Scope scope) {
        _unit = unit;
        _scope = scope;
    }

    public Expression Bind(ValueNode node) => node.Accept(this);

    Expression IValueVisitor<Expression>.Default(ValueNode node) => throw new NotImplementedException();

    Expression IValueVisitor<Expression>.Visit(ValueNode node) => throw new NotImplementedException();

    Expression IValueVisitor<Expression>.Visit(BoolNode node) => new Literal(node, _unit.BoolType);
    Expression IValueVisitor<Expression>.Visit(CharNode node) => new Literal(node, _unit.CharType);
    Expression IValueVisitor<Expression>.Visit(StringNode node) => new Literal(node, _unit.StringType);
    Expression IValueVisitor<Expression>.Visit(NumberNode node) => new Literal(node, _unit.GetNumericType(node.Kind));

    Expression IValueVisitor<Expression>.Visit(ComplexStringNode node)
        => new Interpolation(
            node,
            _unit.StringType,
            node.CodeSections.Select(Bind).ToImmutableArray()
        );

    Expression IValueVisitor<Expression>.Visit(ParenthesizedValueNode node)
        => Bind(node.Value);

    Expression IValueVisitor<Expression>.Visit(TupleNode node) {
        var exprBuilder = ImmutableArray.CreateBuilder<Expression>(node.Count);

        Debug.Assert(!node.Items.IsEmpty);

        foreach (var val in node.Items)
            exprBuilder.Add(Bind(val));

        return new Tuple(node, exprBuilder.DrainToImmutable());
    }

    Expression IValueVisitor<Expression>.Visit(NameNode node) => throw new NotImplementedException();
    Expression IValueVisitor<Expression>.Visit(IdentNode node) {
        var symbol = _scope.ResolveQualified<TypedSymbolInfo>(node)
                  ?? _unit.Factory.CreateMissingSymbol(node, _scope);

        return new Identifier(node, symbol);
    }

    Expression IValueVisitor<Expression>.Visit(FullNameNode node) {
        var nameParts = ImmutableArray.CreateBuilder<QualifiedName.Part>();

        var currSymbol = default(SymbolInfo);

        var currScope = _scope;
        foreach (var partToken in node.Parts) {
            var partName = partToken.Representation;

            currSymbol = currScope.Get(partName)
                      ?? _unit.Factory.CreateMissingSymbol(partName);

            nameParts.Add(new QualifiedName.Part(partName, currSymbol));

            Debug.Assert(currSymbol is IScope);
            currScope = Scope.From((IScope)currSymbol);
        }

        return new QualifiedName(node, nameParts.DrainToImmutable());
    }

    Expression IValueVisitor<Expression>.Visit(FunctionCallNode node) => throw new NotImplementedException();

    Expression IValueVisitor<Expression>.Visit(ObjectCreationNode node)
        => Bind(node.Invocation);
        // => _scope.ResolveQualified<Expression>((NameNode)node.TypeName)
        //         ?? _unit.Factory.CreateMissingType((NameNode)node.TypeName, _scope);

    FunctionInfo? GetBinaryOpBackingFunc(TypeInfo t1, TypeInfo t2, OperationType type) {
        throw new NotImplementedException();
    }

    Expression IValueVisitor<Expression>.Visit(OperationNode node) {
        switch (node.OperationKind) {
            case OperationKind.Binary: {
                var op1Expr = Bind(node.Operands[0]);
                var op2Expr = Bind(node.Operands[1]);
                var func = GetBinaryOpBackingFunc(op1Expr.Type, op2Expr.Type, node.OperationType)
                        ?? throw new NotImplementedException("todo: throw error for not found");
                return new BinaryOperation(node, op1Expr, op2Expr, func);
            }

            default:
                throw new NotImplementedException($"todo: binding for {node.OperationKind}");
        }
    }
}