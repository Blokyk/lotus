namespace Lotus.Semantics;

internal static class SymbolUtils
{
    public static string GetKindString(SymbolInfo s)
        => s switch {
            ParameterInfo => "parameter",
            NamespaceInfo => "namespace",
            FunctionInfo    => "method",
            LocalInfo     => "local variable",
            FieldInfo     => "struct field",
            EnumValueInfo => "enum field",
            TypeInfo      => "type",
            _ => throw new InvalidOperationException("Type " + s.GetType() + " doesn't have an associated kind")
        };

    public static string GetKindString<T>() where T : SymbolInfo {
        if (typeof(T).IsAssignableTo(typeof(ParameterInfo)))
            return "parameter";
        if (typeof(T).IsAssignableTo(typeof(NamespaceInfo)))
            return "namespace";
        if (typeof(T).IsAssignableTo(typeof(FunctionInfo)))
            return "method";
        if (typeof(T).IsAssignableTo(typeof(LocalInfo)))
            return "local";
        if (typeof(T).IsAssignableTo(typeof(FieldInfo)))
            return "field";
        if (typeof(T).IsAssignableTo(typeof(EnumValueInfo)))
            return "enum field";
        if (typeof(T).IsAssignableTo(typeof(TypeInfo)))
            return "type";

        throw new InvalidOperationException("Type " + typeof(T) + " doesn't have an associated kind");
    }
}