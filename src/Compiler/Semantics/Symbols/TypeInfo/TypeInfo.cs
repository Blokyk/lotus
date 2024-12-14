namespace Lotus.Semantics;

public class TypeInfo(SemanticUnit unit)
    : SymbolInfo(unit)
{
    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}