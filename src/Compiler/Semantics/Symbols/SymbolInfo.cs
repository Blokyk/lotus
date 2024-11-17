namespace Lotus.Semantics;

public abstract class SymbolInfo
{
    public Accessibility Accessibility { get; }

    public virtual T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}