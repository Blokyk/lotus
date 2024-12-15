using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

internal sealed class SymbolFactory(SemanticUnit unit)
{
    private readonly SemanticUnit _unit = unit;

    public EnumTypeInfo GetEmptyEnumSymbol(EnumNode node)
        => new(node.Name.TypeName.Value, node.Location, _unit);

    public void FillEnumSymbol(EnumTypeInfo enumType, EnumNode node, Scope _) {
        // todo: support enums with parents

        foreach (var value in node.Values) {
            var valueNode = value switch {
                IdentNode { Value: var rawName }
                    => new EnumValueInfo(rawName, enumType, value.Location, _unit),
                OperationNode { Operands: [IdentNode nameNode, NumberNode val] }
                    => new EnumValueInfo(nameNode.Value, enumType, value.Location, _unit) {
                        Value = (int)(double)val.Value // note: .Value is always a double right now
                    },
                _ => throw null! // values can only be simple names or assignement betwen name and const
            };

            if (!enumType.TryAddValue(valueNode))
                enumType.IsValid = false;
        }
    }

    public StructTypeInfo GetEmptyStructSymbol(StructNode node)
        => new(node.Name.Value, node.Location, _unit);

    public void FillStructSymbol(StructTypeInfo structType, StructNode node, Scope scope) {
        foreach (var fieldDecl in node.Fields) {
            bool isValid = true;

            var fieldType = scope.ResolveQualified<TypeInfo>(fieldDecl.Type);

            if (fieldType is null) {
                structType.IsValid = false;
                isValid = false;
                fieldType = CreateFakeType(fieldDecl.Type);
            }

            var field = new FieldInfo(fieldDecl.Name.Value, fieldType, structType, _unit) {
                IsValid = isValid
            };

            if (!structType.TryAddField(field))
                structType.IsValid = false;
        }
    }

    public FunctionInfo GetEmptyFunctionSymbol(FunctionDeclarationNode node)
        => new(node.FuncName, node.Location, _unit);

    public void FillFunctionSymbol(
        FunctionInfo function,
        FunctionDeclarationNode node,
        Scope scope
    ) {
        if (node.HasReturnType) {
            var returnType = scope.ResolveQualified<TypeInfo>(node.ReturnType);
            if (returnType is null) {
                function.IsValid = false;
                returnType = CreateFakeType(node.ReturnType);
            }

            function.ReturnType = returnType;
        } else {
            function.ReturnType = _unit.VoidType;
        }

        foreach (var paramNode in node.ParamList) {
            bool isValid = true;

            var paramType = scope.ResolveQualified<TypeInfo>(paramNode.Type);

            if (paramType is null) {
                function.IsValid = false;
                isValid = false;
                paramType = CreateFakeType(paramNode.Type);
            }

            var param = new ParameterInfo(paramNode.Name.Value, paramType, function, paramNode.Location, _unit) {
                IsValid = isValid
            };

            if (!function.TryAdd(param))
                function.IsValid = false;
        }
    }

    public ErrorTypeInfo CreateFakeType(string name)
        => new(name, _unit);
    public ErrorTypeInfo CreateFakeType(NameNode name)
        => CreateFakeType(name.ToFullString());

    public ErrorTypeInfo CreateFakeType(NameNode name, Scope scope) {
        if (name is IdentNode { Value: var typeName })
            return new(typeName, _unit);

        var nameParts = ((FullNameNode)name).Parts;

        SymbolInfo container = new ErrorSymbolInfo(nameParts[0], _unit);
        var currScope = scope;

        foreach (var part in nameParts.SkipLast(1)) { // don't try to resolve the type name, obviously
            var nextContainer = currScope.Get(part);

            if (scope.Get(part) is NamespaceInfo ns)
                nextContainer = ns;
            else
                nextContainer = new ErrorSymbolInfo(part, _unit) { ContainingSymbol = container };

            container = nextContainer;
            currScope = Scope.From((container as IScope)!);
        }

        return new ErrorTypeInfo(nameParts[^1], _unit) { ContainingSymbol = container };
    }
}