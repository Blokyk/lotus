using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal abstract class Expression(ValueNode node, TypeInfo type)
    : ILocalized
{
    public ValueNode SyntaxNode => node;

    public LocationRange Location => SyntaxNode.Location;

    public TypeInfo Type { get; set; } = type;
}
