namespace Lotus.Syntax;

public sealed record StructNode(
    Token Token,
    IdentNode Name,
    Tuple<StructField> Fields,
    ImmutableArray<Token> Modifiers
) : TopLevelNode(Token, new LocationRange(Token, Fields)){
    public new static readonly StructNode NULL = new(Token.NULL, IdentNode.NULL, Tuple<StructField>.NULL, default) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}

public sealed record StructField(
    IdentNode Name,
    NameNode Type, // fixme: this isn't always a name!! (eg: int[])
    ValueNode? DefaultValue,
    Token? EqualSign,
    ImmutableArray<Token> Modifiers
) : Parameter(Type, Name, new LocationRange(Name, DefaultValue ?? Name)) {
    public static readonly StructField NULL = new(
        IdentNode.NULL,
        NameNode.NULL,
        ImmutableArray<Token>.Empty
    ) { IsValid = false };

    public StructField(IdentNode name, NameNode type, ImmutableArray<Token> modifiers)
        : this(name, type, null, null, modifiers) {}

    [MemberNotNullWhen(true, nameof(DefaultValue), nameof(EqualSign))]
    public bool HasDefaultValue => DefaultValue is not null;
}