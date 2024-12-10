namespace Lotus.Syntax;

public class SyntaxTree : ILocalized
{
    public ImmutableArray<TopLevelNode> TopNodes { get; }

    public bool IsValid { get; }

    public LocationRange Location { get; }
    public string Filename => Location.filename;

    public SyntaxTree(TextStream stream) : this(new Tokenizer(stream)) { }

    public SyntaxTree(Tokenizer tokenizer) : this(new Parser(tokenizer)) { }
    public SyntaxTree(Parser parser) {
        var nodes = ImmutableArray.CreateBuilder<TopLevelNode>();

        var startLoc = parser.Position;

        // fixme: ensure nodes are in the right order (i.e. declarations
        // come after usings/imports and namespaces)
        while (!parser.EndOfStream) nodes.Add(parser.ConsumeTopLevel());

        var endLoc = parser.Position;

        TopNodes = nodes.DrainToImmutable();

        // todo: this should also check that every node has IsValid = true
        IsValid = !Logger.HasErrors;
        Location = new(startLoc, endLoc);
    }
}