namespace Lotus.Semantics;

public enum SpecialType : byte {
    None,
    Void,
    Bool,
    Char,
    String,
    Int,
    UInt,
    Long,
    ULong,
    Float,
    Double
}

public static class SpecialTypeUtils
{
    public static SpecialType AsSpecialType(this NumberKind kind)
        => kind switch {
            NumberKind.Int => SpecialType.Int,
            NumberKind.UInt => SpecialType.UInt,
            NumberKind.Long => SpecialType.Long,
            NumberKind.ULong => SpecialType.ULong,
            NumberKind.Float => SpecialType.Float,
            NumberKind.Double => SpecialType.Double,
            _ => throw new ArgumentOutOfRangeException($"Number kind '{kind}' does not have a corresponding type")
        };
}