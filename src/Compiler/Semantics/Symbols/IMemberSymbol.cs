namespace Lotus.Semantics;

public interface IMemberSymbol<out T>
{
    T ContainingSymbol { get; }
}