using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

internal partial class Binder : IValueVisitor<Expression>
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

        var lastSymbol = default(SymbolInfo);

        var currScope = _scope;
        foreach (var partToken in node.Parts) {
            var partName = partToken.Representation;

            var currSymbol = currScope.Get(partName);

            if (currSymbol is null) {
                var errSymbol = _unit.Factory.CreateMissingSymbol(partName);
                errSymbol.ContainingSymbol = lastSymbol;

                currSymbol = errSymbol;
            }

            nameParts.Add(new QualifiedName.Part(partName, currSymbol));

            Debug.Assert(currSymbol is IScope);
            currScope = Scope.From((IScope)currSymbol);

            lastSymbol = currSymbol;
        }

        return new QualifiedName(node, nameParts.DrainToImmutable());
    }

    Expression IValueVisitor<Expression>.Visit(FunctionCallNode node) {
        var funcExpr = Bind(node.Name);

        if (funcExpr is not QualifiedName { Symbol: FunctionInfo func })
            throw new NotImplementedException("todo: error for non-func call");

        var initialArgs = node.ArgList.Select(Bind).ToImmutableArray();

        if (!TryBindAndConvertArgs(func, initialArgs, out var finalArgs))
            throw new NotImplementedException("todo: error for type mismatch in function call");

        return new FunctionCall(node, func, finalArgs);
    }

    Expression IValueVisitor<Expression>.Visit(ObjectCreationNode node)
        => Bind(node.Invocation);

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