using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

internal sealed class VoidTypeInfo(SemanticUnit unit)
    : UserTypeInfo("void", LocationRange.NULL, unit)
    , IScope
{
    public override SpecialType SpecialType => SpecialType.Void;

    // void doesn't have any field
    Scope IScope.Scope => Scope.Empty;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}