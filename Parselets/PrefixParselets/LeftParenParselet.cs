using System;

public sealed class LeftParenParselet : IPrefixParselet<ValueNode>
{
    public ValueNode Parse(Parser parser, Token leftParenToken) {
        // This one is a bit tricky : when we have a left parenthesis, it could either be a type cast,
        // or a grouping parenthesis. For example, we need to differentiate between those two expressions :
        // (char)0
        // and
        // (9 * 6) / 7
        // However, those are simple examples, and it can become trickier with other features, such as array initialization
        // and type nesting and, well, grouping parenthesis :
        // (Array.IntArray)[6, 8, 322, 42, 3]
        // and
        // (int)(2.5 / 3.0)
        // We don't want to have edge-cases exceptions to the type casting operators.
        // Let's define formally these two kind of expressions :
        //
        //
        // value-literal :
        //      integer-literal
        //    | string-literal
        //    | array-literal
        //    | float-literal
        //
        // value :
        //      identifier
        //    | value-literal
        //    | operation
        //    | grouping-paren
        //
        //
        // access-operation :
        //      value '.' identifier
        //
        // type-name :
        //        identifier
        //      | access-operation
        //
        // cast_expression :
        //      '(' type-name ')' value
        //
        //
        // grouping-paren :
        //      '(' value ')'
        //
        // Since the start of each rule can be ambigous for the parser, we need a way to differentiate easily between the two
        // In most top-down parsers, this is resolved by making each defined type a keyword, and then check that the value inside of
        // the cast expression's parenthesis is a keyword (i.e. a type)
        // However, this parser is not top-down, therefore, we gotta find another solution.
        // The solution used here, for now, is to check that the value inside of the parenthesis is either an identifier or an
        // access-operation.
        // Implementation-wise, we basically check that the top node is either an IdentNode, or an OperationNode with the
        // OperationType set to "Access" (alternatively, we could check if the representation is '.', but I prefer the opType check)
        // If the check succeeds, we parse it as type-casting expression ;
        // otherwise, it means that this is a grouping parenthesis and parse it as such

        if (leftParenToken != "(")
            throw Logger.Fatal(new InvalidCallException(leftParenToken.Location));

        var value = parser.ConsumeValue();

        var rightParenToken = parser.Tokenizer.Consume();

        // if the next token isn't a right/closing parenthesis, throw an error
        if (rightParenToken != ")") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "after a parenthesized expression",
                expected: ")"
            ));

            value.IsValid = false;

            parser.Tokenizer.Reconsume(); // TODO: could we try to parse this as a list of values (still throw tho)
        }

        // if the value is a name (an identifier or an access operation), parse it as a type-cast expression
        if (Utilities.IsName(value))
            return new TypeCastNode(value, parser.ConsumeValue(Precedence.TypeCast), leftParenToken, value.IsValid);

        // otherwise, parse it as a grouping parenthesis expression
        // (which is just returning the value)

        return new ParenthesizedValueNode(leftParenToken, rightParenToken, value);
    }
}
