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
                        Value = (int)val.Value
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
                fieldType = Builtins.Unknown;
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
                returnType = Builtins.Unknown;
            }

            function.ReturnType = returnType;
        } else {
            function.ReturnType = Builtins.Void;
        }

        foreach (var paramNode in node.ParamList) {
            bool isValid = true;

            var paramType = scope.ResolveQualified<TypeInfo>(paramNode.Type);

            if (paramType is null) {
                function.IsValid = false;
                isValid = false;
                paramType = Builtins.Unknown;
            }

            var param = new ParameterInfo(paramNode.Name.Value, paramType, function, _unit) {
                IsValid = isValid
            };

            if (!function.TryAdd(param))
                function.IsValid = false;
        }
    }
}