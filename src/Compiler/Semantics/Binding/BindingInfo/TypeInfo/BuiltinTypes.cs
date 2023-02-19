namespace Lotus.Semantics.Binding;

internal static class BuiltinTypes
{
    public static readonly ErrorTypeInfo Unknown
        = new();
    public static readonly ErrorTypeInfo Error
        = new();

    public static readonly NamedTypeInfo Void
        = new("void");
    public static readonly NamedTypeInfo Bool
        = new("bool");
    public static readonly NamedTypeInfo String
        = new("string");
    public static readonly NamedTypeInfo Int
        = new("int");
    public static readonly NamedTypeInfo UInt
        = new("uint");
    public static readonly NamedTypeInfo Long
        = new("long");
    public static readonly NamedTypeInfo ULong
        = new("ulong");
    public static readonly NamedTypeInfo Float
        = new("float");
    public static readonly NamedTypeInfo Double
        = new("double");
}