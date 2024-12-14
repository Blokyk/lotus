namespace Lotus.Semantics;

public abstract class TypedSymbolInfo(SemanticUnit unit) : SymbolInfo(unit) {
    public abstract TypeInfo Type { get; }

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}