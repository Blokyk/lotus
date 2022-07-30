public sealed class IfParslet : IStatementParslet<IfNode>
{
    public static readonly IfParslet Instance = new();

    public IfNode Parse(StatementParser parser, Token ifToken) {
        Debug.Assert(ifToken == "if");

        var isValid = true;

        var conditionNode = parser.ExpressionParser.Consume();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = conditionNode,
                As = "an if-statement condition",
                Expected = "a condition between parenthesis (e.g. `(a == b)`)"
            });

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                Logger.errorStack.Pop();

                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = conditionNode,
                    Message = "You can't use a tuple as a condition. ",
                    ExtraNotes = "If you want to combine multiple conditions, "
                            +"you can use the logical operators (OR ||, AND &&, XOR ^^, etc...)"
                });

                condition = tuple.AsParenthesized();
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL, isValid: false);
            }
        }

        var body = parser.ConsumeStatementBlock();

        var elseNode = ElseNode.NULL;

        if (parser.Tokenizer.Peek() == "else") {
            elseNode = ElseParslet.Instance.Parse(parser, parser.Tokenizer.Consume());
        }

        return new IfNode(condition, body, elseNode, ifToken, isValid);
    }

    // TODO: Later
    /*static ElseNode[] FlattenElseChain(ElseNode elseNode) {
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new[] { elseNode };

        return (new[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}