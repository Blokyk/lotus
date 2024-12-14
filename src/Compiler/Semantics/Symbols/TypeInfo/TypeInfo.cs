namespace Lotus.Semantics;

public class TypeInfo(SemanticUnit unit)
    : SymbolInfo(unit)
{
    public virtual SpecialType SpecialType => SpecialType.None;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}