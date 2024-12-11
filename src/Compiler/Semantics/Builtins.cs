namespace Lotus.Semantics;

internal static class Builtins
{
    public static readonly MissingTypeInfo Unknown
        = new("<unknown>");

    public static readonly StructTypeInfo Void
        = new("void", LocationRange.NULL);
    public static readonly StructTypeInfo Bool
        = new("bool", LocationRange.NULL);
    public static readonly StructTypeInfo String
        = new("string", LocationRange.NULL);
    public static readonly StructTypeInfo Int
        = new("int", LocationRange.NULL);
    public static readonly StructTypeInfo UInt
        = new("uint", LocationRange.NULL);
    public static readonly StructTypeInfo Long
        = new("long", LocationRange.NULL);
    public static readonly StructTypeInfo ULong
        = new("ulong", LocationRange.NULL);
    public static readonly StructTypeInfo Float
        = new("float", LocationRange.NULL);
    public static readonly StructTypeInfo Double
        = new("double", LocationRange.NULL);
}