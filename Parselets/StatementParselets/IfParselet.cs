public sealed class IfParslet : IStatementParslet<IfNode>
{
    public IfNode Parse(StatementParser parser, Token ifToken) {
        if (!(ifToken is ComplexToken ifKeyword && ifKeyword == "if")) {
            throw Logger.Fatal(new InvalidCallException(ifToken.Location));
        }

        var isValid = true;

        var conditionNode = parser.ExpressionParser.ConsumeValue();

        if (!(conditionNode is ParenthesizedValueNode condition)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as an if-statement condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                Logger.exceptions.Pop();

                Logger.Error(new UnexpectedValueTypeException(
                    node: conditionNode,
                    message: "You can't use a tuple as a condition. "
                            +"If you wish to combine multiple conditions, "
                            +"use the logical operators (OR ||, AND &&, XOR ^^, etc...)"
                ));

                condition = new ParenthesizedValueNode(
                    tuple.Count == 0 ? ValueNode.NULL : tuple.Values[0],
                    tuple.OpeningToken,
                    tuple.ClosingToken,
                    isValid: false
                );
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL, isValid: false);
            }
        }

        var body = parser.ConsumeSimpleBlock();

        if (parser.Tokenizer.Peek() == "else") {
            var elseNode = new ElseParslet().Parse(parser, parser.Tokenizer.Consume());

            return new IfNode(condition, body, elseNode, ifKeyword);
        }

        return new IfNode(condition, body, ifKeyword, isValid);
    }

    // TODO: Later
    /*static ElseNode[] FlattenElseChain(ElseNode elseNode) {
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new[] { elseNode };

        return (new[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}