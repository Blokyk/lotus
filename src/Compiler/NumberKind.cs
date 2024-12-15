namespace Lotus;

[Flags]
public enum NumberKind
{
    Unknown = 0,
    Unsigned = 1 << 0,
    Int = 1 << 1,
    Long = 1 << 2,
    Float = 1 << 3,
    Double = 1 << 4,

    UInt = Unsigned | Int,
    ULong = Unsigned | Long
}