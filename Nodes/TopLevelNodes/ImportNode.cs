using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ImportNode : TopLevelNode
{
    public new static readonly ImportNode NULL = new(Array.Empty<ValueNode>(), FromNode.NULL, Token.NULL, false);

    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    public ImportNode(IList<ValueNode> imports, FromNode from, Token importToken, bool isValid = true)
        : base(importToken, new LocationRange(from.Location, imports[0].Location), isValid)
    {
        ImportsName = imports.AsReadOnly();
        FromStatement = from;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
