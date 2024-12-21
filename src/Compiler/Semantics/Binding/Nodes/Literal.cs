using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal class Literal(LiteralNode node, TypeInfo type)
    : Expression(node, type)
{
    public SpecialType LiteralType => Type!.SpecialType;
    public object GetValue() => node.Value;
}