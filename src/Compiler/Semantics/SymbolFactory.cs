using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

internal sealed class SymbolFactory(SemanticUnit unit)
{
    private readonly SemanticUnit _unit = unit;

    public EnumTypeInfo GetEmptyEnumSymbol(EnumNode node)
        => new EnumTypeInfo(node.Name.TypeName.Value, node.Location);

    public void FillEnumSymbol(EnumTypeInfo enumType, EnumNode node, Scope _) {
        // todo: support enums with parents

        foreach (var value in node.Values) {
            var valueNode = value switch {
                IdentNode { Value: var rawName }
                    => new EnumValueInfo(rawName, enumType, value.Location),
                OperationNode { Operands: [IdentNode nameNode, NumberNode val] }
                    => new EnumValueInfo(nameNode.Value, enumType, value.Location) {
                        Value = (int)val.Value
                    },
                _ => throw null! // values can only be simple names or assignement betwen name and const
            };

            if (!enumType.TryAddValue(valueNode))
                enumType.IsValid = false;
        }
    }

    public StructTypeInfo GetEmptyStructSymbol(StructNode node)
        => new(node.Name.Value, node.Location);

    public void FillStructSymbol(StructTypeInfo structType, StructNode node, Scope scope) {
        foreach (var fieldDecl in node.Fields) {
            bool isValid = true;

            var fieldType = scope.ResolveQualified<TypeInfo>(fieldDecl.Type);

            if (fieldType is null) {
                structType.IsValid = false;
                isValid = false;
                fieldType = Builtins.Unknown;
            }

            var field = new FieldInfo(fieldDecl.Name.Value, fieldType, structType) {
                IsValid = isValid
            };

            if (!structType.TryAddField(field))
                structType.IsValid = false;
        }
    }
}