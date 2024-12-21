using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

// todo: should be split between BoundIdent and BoundQualifiedName(immut BoundExpr[] parts)
// note: a name can be dependent on non-name expr (eg: ("hello"+"world").length)
internal abstract class Name(NameNode name, TypedSymbolInfo symbol)
    : Expression(name, symbol.Type)
{
    public new NameNode SyntaxNode => name;
    public SymbolInfo Symbol => symbol;
}