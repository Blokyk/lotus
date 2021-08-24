public record PrintNode(Token Token, ValueNode Value, bool IsValid = true)
: StatementNode(Token, Token.Location, IsValid)
{
    public new static readonly PrintNode NULL = new(Token.NULL, ValueNode.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}