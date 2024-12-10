namespace Lotus.Semantics;

internal static class SymbolUtils
{
    public static string GetKindString(SymbolInfo s)
        => s switch {
            ParameterInfo => "parameter",
            NamespaceInfo => "namespace",
            MethodInfo    => "method",
            LocalInfo     => "local variable",
            FieldInfo     => "struct field",
            EnumValueInfo => "enum field",
            TypeInfo      => "type",
            _ => throw new InvalidOperationException("Type " + s.GetType() + " doesn't have an associated kind")
        };
}