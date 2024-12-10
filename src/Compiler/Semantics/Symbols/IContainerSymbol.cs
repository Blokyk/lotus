using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public interface IContainerSymbol<out T>
{
    IEnumerable<T> Children();
}