namespace Lotus.Semantics;

public abstract class TypeInfo(SemanticUnit unit)
    : SymbolInfo(unit)
{
    public virtual SpecialType SpecialType => SpecialType.None;

    public abstract bool Implements(TraitInfo info, [NotNullWhen(true)] out TraitImplInfo? implInfo);

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}