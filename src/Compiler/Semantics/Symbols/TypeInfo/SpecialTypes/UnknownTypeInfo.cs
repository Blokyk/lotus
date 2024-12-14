using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class UnknownTypeInfo(SemanticUnit unit)
    : TypeInfo(unit)
    , IScope
{
    public override SpecialType SpecialType => SpecialType.Unknown;

    Scope IScope.Scope => Scope.Empty;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}