using Lotus.Syntax;
using Lotus.Syntax.Visitors;

namespace Lotus.Semantics.Binding;

partial class Binder : IValueVisitor<BoundExpression>
{
    BoundExpression IValueVisitor<BoundExpression>.Visit(OperationNode node) {
        throw new NotImplementedException();
    }

}