using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

// note: a name can be dependent on non-name expr (eg: ("hello"+"world").length)
internal sealed class Identifier(IdentNode name, TypedSymbolInfo symbol)
    : Name(name, symbol)
{
    public new IdentNode SyntaxNode => name;
}