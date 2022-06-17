public sealed class EnumParselet : ITopLevelParslet<EnumNode>
{
    private static readonly EnumParselet _instance = new();
    public static EnumParselet Instance => _instance;

    private EnumParselet() { }

    public EnumNode Parse(TopLevelParser parser, Token enumToken) {

        /*
        *   Enums, just like a lot of top-lvl stuff comes in all shapes
        *   and sizes, so parsing them is gonna require a bit of caution
        *   I want enums to be able to inherit from another enum's
        *   value, meaning that you have something like this :
        *
        *       @TokenKind.lts
        *       public enum TokenKind {
        *           delim,
        *           value,
        *           trivia,
        *           EOF,
        *       }
        *
        *       @TriviaKind.lts
        *       public enum TokenKind.trivia::TriviaKind {
        *           whitespace,
        *           comment,
        *           character
        *       }
        *
        */

        bool isValid = true;

        var nameOrParent = parser.ExpressionParser.Consume();

        var nameValue = nameOrParent;
        var parent = ValueNode.NULL;

        if (parser.Tokenizer.Peek() == "::") {
            parser.Tokenizer.Consume();
            parent = nameOrParent;

            if (!Utilities.IsName(parent)) {
                // TODO: make a custom error for things that are supposed to be names
                // + that would allow them to be clearer about why the value cannot
                //   be a name, because rn it would spit out "OperationNodes cannot
                //   be used to specify an enum's parent" but to the user that doesn't
                //   mean anything
                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = parent,
                    Message = parent.GetType().Name + "s cannot be used to specify an enum's parent",
                    As = "an enum's parent value"
                });

                isValid = false;
            }

            nameValue = parser.ExpressionParser.Consume();
        }

        if (nameValue is not IdentNode name) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = nameValue,
                Expected = "an identifier",
                As = "a new enum name"
            });

            isValid = false;
            name = new IdentNode(
                new IdentToken(
                    nameValue.Token.Representation,
                    nameValue.Token.Location,
                    false
                ),
                false
            );
        }

        if (!parser.Tokenizer.Consume(out var openBracket) || openBracket != "{") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openBracket,
                Expected = "an opening bracket '{'"
            });

            isValid = false;
        }

        var values = new List<ValueNode>();

        while (parser.Tokenizer.Consume(out var nextToken) && nextToken != "}") {
            parser.Tokenizer.Reconsume();

            var val = parser.ExpressionParser.Consume();

            if (val is not IdentNode && val is not OperationNode { OperationType: OperationType.Assign }) {
                if (val.Token.Kind == TokenKind.EOF) {
                    Logger.errorStack.Pop();
                    break;
                }

                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = val,
                    Expected = "a name or an assignment",
                    As = "an enum value"
                });

                val.IsValid = false;
            }

            values.Add(val);

            if (!parser.Tokenizer.Consume(out nextToken) || nextToken != ",") {
                if (nextToken == "}") {
                    break; // normal path
                }

                isValid = false;

                if (nextToken.Kind == TokenKind.EOF)
                    break; // handled by code afterwards

                if (nextToken.Kind is TokenKind.identifier)
                    parser.Tokenizer.Reconsume();

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = nextToken,
                    Expected = "a comma ',' or a closing bracket '}'"
                });
            }
        }

        var closingBracket = parser.Tokenizer.Current;

        if (closingBracket.Kind == TokenKind.EOF) {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                Expected = "a closing bracket '}'",
                Location = values.LastOrDefault()?.Location ?? openBracket.Location
            });

            isValid = false;
        }

        return new EnumNode(name, values, parent, enumToken, openBracket, closingBracket, isValid);
    }
}