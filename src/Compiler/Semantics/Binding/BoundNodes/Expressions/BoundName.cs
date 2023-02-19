using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal sealed class BoundName : BoundExpression
{
    public MemberInfo Info { get; set; }

    public BoundName(NameNode name, MemberInfo info, TypeInfo type)
        : base(name, type)
    {
        Info = info;
    }
}