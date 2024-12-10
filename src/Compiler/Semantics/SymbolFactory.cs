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
}