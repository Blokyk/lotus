using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

internal sealed class SymbolFactory(SemanticUnit unit)
{
    private readonly SemanticUnit _unit = unit;

    public EnumTypeInfo GetEnumSymbol(EnumNode node, Scope _) {
        // todo: support enums with parents
        var enumType = new EnumTypeInfo(node.Name.TypeName.Value, node.Location);

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

        return enumType;
    }

    public StructTypeInfo GetStructSymbol(StructNode node, Scope scope) {
        var structType = new StructTypeInfo(node.Name.Value, node.Location);

        foreach (var fieldDecl in node.Fields) {
            var fieldTypeSymbol = scope.ResolveQualified(fieldDecl.Type);

            if (fieldTypeSymbol is null) {
                Logger.Error(new UnknownSymbol {
                    SymbolName = fieldDecl.Type.ToFullString(),
                    ExpectedKinds = [ "type name" ],
                    Location = fieldDecl.Type.Location
                });
                structType.IsValid = false;
                continue;
            }

            if (fieldTypeSymbol is not TypeInfo fieldType) {
                Logger.Error(new UnexpectedSymbolKind {
                    TargetSymbol = fieldTypeSymbol,
                    ExpectedKinds = [ "type name" ],
                    Location = fieldDecl.Type.Location
                });
                structType.IsValid = false;
                continue;
            }

            var field = new FieldInfo(fieldDecl.Name.Value, fieldType, structType);

            if (!structType.TryAddField(field))
                structType.IsValid = false;
        }

        return structType;
    }
}