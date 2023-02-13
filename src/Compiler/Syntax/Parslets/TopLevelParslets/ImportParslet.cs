namespace Lotus.Syntax;

public sealed class ImportParslet : ITopLevelParslet<ImportNode>
{
    public static readonly ImportParslet Instance = new();

    private static NameNode ParseSingleName(ExpressionParser exprParser)
        =>  exprParser.Consume<NameNode>(
                NameNode.NULL,
                @in: "an import statement",
                @as: "a namespace or type name"
            );

    private static readonly ValueTupleParslet<NameNode> _importNamesParslet
        = new(ParseSingleName) {
            Start = "{",
            End = "}",
            In = "an import statement's name list"
        };

    public ImportNode Parse(TopLevelParser parser, Token importToken, ImmutableArray<Token> modifiers) {
        Debug.Assert(importToken == "import");

        TopLevelParser.ReportIfAnyModifiers(modifiers, "import statements", out var hasModifiers);

        var exprParser = parser.ExpressionParser;

        Tuple<NameNode> names;

        if (parser.Tokenizer.Peek() is IdentToken _)
            names = new Tuple<NameNode>(ParseSingleName(exprParser));
        else
            names = _importNamesParslet.Parse(exprParser);

        FromOrigin? from = null;

        if (parser.Tokenizer.Peek() == "from") {
            var fromToken = parser.Tokenizer.Consume();

            var fromIsValid =
                exprParser.TryConsumeEither<StringNode, NameNode>(
                    defaultVal: NameNode.NULL,
                    out var fromOrigin,
                    out var fromVal
                );

            if (!fromIsValid) {
                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = fromVal,
                    In = "a from statement",
                    Expected = "either a string or a name"
                });
            }

            from = new FromOrigin(fromOrigin, fromToken) { IsValid = fromIsValid };
        }

        return new ImportNode(names, from, importToken) { IsValid = !hasModifiers && names.IsValid };
    }
}