namespace Lotus.Semantics.Binding;

internal abstract class MemberInfo
{
    public Accessibility Accessibility { get; set; }

    protected MemberInfo() {}
}