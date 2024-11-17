namespace Lotus.Semantics;

public static class BuiltinTypes
{
    public static readonly MissingTypeInfo Unknown
        = new("<unknown>");

    public static readonly StructTypeInfo Void
        = new("void");
    public static readonly StructTypeInfo Bool
        = new("bool");
    public static readonly StructTypeInfo String
        = new("string");
    public static readonly StructTypeInfo Int
        = new("int");
    public static readonly StructTypeInfo UInt
        = new("uint");
    public static readonly StructTypeInfo Long
        = new("long");
    public static readonly StructTypeInfo ULong
        = new("ulong");
    public static readonly StructTypeInfo Float
        = new("float");
    public static readonly StructTypeInfo Double
        = new("double");
}