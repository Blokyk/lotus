using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

internal partial class Binder : IValueVisitor<BoundExpression>
{
    private Dictionary<> _names;

    BoundExpression IValueVisitor<BoundExpression>.Visit(NameNode node) {

    }
}