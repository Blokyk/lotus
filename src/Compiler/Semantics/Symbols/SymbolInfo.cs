namespace Lotus.Semantics;

public abstract class SymbolInfo(SemanticUnit unit)
{
    public Accessibility Accessibility { get; }

    public bool IsValid { get; set; } = true;

    internal SemanticUnit Unit { get; init; } = unit;

    public virtual T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}