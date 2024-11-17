namespace Lotus.Semantics;

public class TypeInfo
    : SymbolInfo
{
    public TypeInfo() {}

    public override T Accept<T>(ISymbolVisitor<T> visitor) => visitor.Visit(this);
}