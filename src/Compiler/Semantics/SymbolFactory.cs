using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

internal sealed partial class SymbolFactory(SemanticUnit unit)
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
                fieldType = CreateMissingType(fieldDecl.Type, scope);
            }

            var field = new FieldInfo(
                fieldDecl.Name.Value,
                fieldType,
                structType,
                fieldDecl.Location,
                _unit
            ) { IsValid = isValid };

            if (!structType.TryAddField(field))
                structType.IsValid = false;
        }
    }

    public FunctionInfo GetEmptyFunctionSymbol(FunctionHeaderNode node)
        => new(node.Name, node.Location, _unit);

    public void FillFunctionSymbol(
        FunctionInfo function,
        FunctionHeaderNode node,
        Scope scope
    ) {
        if (node.HasReturnType) {
            var returnType = scope.ResolveQualified<TypeInfo>(node.ReturnType);
            if (returnType is null) {
                function.IsValid = false;
                returnType = CreateMissingType(node.ReturnType, scope);
            }

            function.ReturnType = returnType;
        } else {
            function.ReturnType = _unit.VoidType;
        }

        foreach (var paramNode in node.Parameters) {
            bool isValid = true;

            var paramType = scope.ResolveQualified<TypeInfo>(paramNode.Type);

            if (paramType is null) {
                function.IsValid = false;
                isValid = false;
                paramType = CreateMissingType(paramNode.Type, scope);
            }

            var param = new ParameterInfo(paramNode.Name.Value, paramType, function, paramNode.Location, _unit) {
                IsValid = isValid
            };

            if (!function.TryAdd(param))
                function.IsValid = false;
        }
    }
}