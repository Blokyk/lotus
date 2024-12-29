namespace Lotus.Semantics;

public static class TypeComparer
{
    public static bool TryConvertTo(
        this TypeInfo from,
        TypeInfo to,
        [NotNullWhen(true)] out Conversion? conversion
    ) {
        conversion = null;

        if (from == to) {
            conversion = new IdentityConversion(from, to);
            return true;
        }

        throw new NotImplementedException("todo: type conversions");
    }
}