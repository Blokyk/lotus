namespace Lotus.Semantics;

internal static class NumberKindUtils
{
    public static string AsTypeName(this NumberKind kind)
        => kind switch {
            NumberKind.Int => "int",
            NumberKind.UInt => "uint",
            NumberKind.Long => "long",
            NumberKind.ULong => "ulong",
            NumberKind.Float => "float",
            NumberKind.Double => "double",
            _ => throw new ArgumentOutOfRangeException($"Number kind '{kind}' does not have a corresponding type name")
        };
}