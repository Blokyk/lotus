using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal class Interpolation(
    ComplexStringNode node,
    TypeInfo strType,
    ImmutableArray<Expression> sections
)
    : Expression(node, strType)
{
    public ImmutableArray<Expression> Sections => sections;
}