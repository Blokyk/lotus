using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

internal sealed class CharTypeInfo(SemanticUnit unit)
    : UserTypeInfo("char", LocationRange.NULL, unit)
    , IScope
{
    public override SpecialType SpecialType => SpecialType.Char;

    // char doesn't have any fields
    Scope IScope.Scope => Scope.Empty;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}