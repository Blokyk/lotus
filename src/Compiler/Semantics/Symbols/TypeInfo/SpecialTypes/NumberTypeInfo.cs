using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

internal sealed class NumberTypeInfo : UserTypeInfo
    , IScope
{
    public NumberKind Kind { get; }

    public NumberTypeInfo(NumberKind numberKind, SemanticUnit unit) : base(numberKind.AsTypeName(), LocationRange.NULL, unit) {
        if (numberKind is NumberKind.Unknown or NumberKind.Unsigned || !Enum.IsDefined(numberKind))
            throw new ArgumentException($"A symbol can't have an unknown number kind!  (got value {numberKind})");
        Kind = numberKind;
    }

    public override SpecialType SpecialType
        => Kind switch {
            NumberKind.Int => SpecialType.Int,
            NumberKind.UInt => SpecialType.UInt,
            NumberKind.Long => SpecialType.Long,
            NumberKind.ULong => SpecialType.ULong,
            NumberKind.Float => SpecialType.Float,
            NumberKind.Double => SpecialType.Double,
            _ => throw null!
        };

    // numbers don't have any fields
    // todo: when we have (self-)traits, add scope for core methods
    Scope IScope.Scope => Scope.Empty;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}