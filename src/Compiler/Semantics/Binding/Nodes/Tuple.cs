using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class Tuple(
    TupleNode node,
    ImmutableArray<Expression> items,
    TypeInfo type
)
    : Expression(node, type)
{
    public ImmutableArray<Expression> Items => items;

    public Tuple(TupleNode node, ImmutableArray<Expression> items)
        : this(
            node,
            items,
            new TupleTypeInfo(
                items.Select(n => n.Type).ToImmutableArray(),
                items[0].Type.Unit
            )
        ) {}
}