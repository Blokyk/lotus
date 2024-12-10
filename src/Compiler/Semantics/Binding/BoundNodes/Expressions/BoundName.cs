using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class BoundName : BoundExpression
{
    public INamedSymbol Symbol { get; set; }

    public BoundName(NameNode name, INamedSymbol symbol)
        : base(name)
    {
        Symbol = symbol;
    }
}