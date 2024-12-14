using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

internal sealed class BoolTypeInfo(SemanticUnit unit)
    : UserTypeInfo("bool", LocationRange.NULL, unit)
    , IScope
{
    public override SpecialType SpecialType => SpecialType.Bool;

    // bool doesn't have any fields
    // todo: when we have (self-)traits, add scope for core methods
    Scope IScope.Scope => Scope.Empty;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}