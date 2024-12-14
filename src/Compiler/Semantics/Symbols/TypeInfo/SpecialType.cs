namespace Lotus.Semantics;

public enum SpecialType : byte {
    None,
    Unknown,
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
    public static TypeInfo? GetSpecialType(this SemanticUnit unit, SpecialType type)
        => type switch {
            SpecialType.Unknown => unit.UnknownType,
            SpecialType.Void => unit.VoidType,
            SpecialType.Bool => unit.BoolType,
            SpecialType.Char => unit.CharType,
            SpecialType.String => unit.StringType,
            SpecialType.Int => unit.IntType,
            SpecialType.UInt => unit.UIntType,
            SpecialType.Long => unit.LongType,
            SpecialType.ULong => unit.ULongType,
            SpecialType.Float => unit.FloatType,
            SpecialType.Double => unit.DoubleType,
            _ => null,
        };
}