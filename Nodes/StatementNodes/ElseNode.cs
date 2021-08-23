public class ElseNode : StatementNode
{
    public new static readonly ElseNode NULL = new(SimpleBlock.NULL, Token.NULL, false);

    public SimpleBlock Body { get; protected set; }

    public IfNode? IfNode { get; protected set; }

    public bool HasIf { get => IfNode != null; }

    public ElseNode(SimpleBlock body, Token elseToken, bool isValid = true)
        : base(elseToken, new LocationRange(elseToken.Location, body.Location), isValid)
    {
        Body = body;
        IfNode = null; // FIXME: we shouldn't have pure nulls here. another reason to write nulls for every node
    }

    public ElseNode(IfNode ifNode, Token elseToken, bool isValid = true)
        : base(elseToken, new LocationRange(elseToken.Location, ifNode.Location), isValid)
    {
        IfNode = ifNode;
        Body = ifNode.Body; // works like a pointer so it's fine
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}