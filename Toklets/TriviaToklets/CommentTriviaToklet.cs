using System;
using System.Text;
using System.Collections.Generic;

public sealed class CommentTriviaToklet : ITriviaToklet<CommentTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (consumer => {
                if (consumer.Consume() != '/') {
                    return false;
                }

                if (consumer.Consume() != '/' && consumer.Current != '*') {
                    return false;
                }

                return true;
            }
        );

    private CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer, bool isInner) {

        var startingPosition = input.Position;

        var isValid = true;

        var currChar = input.Consume();

        if (currChar != '/') {
            throw Logger.Fatal(new InvalidCallException(input.Position));
        }

        currChar = input.Consume();

        if (currChar == '/') {

            var strBuilder = new StringBuilder("//");

            while (input.Consume(out currChar) && currChar != '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(strBuilder.ToString(), new LocationRange(startingPosition, input.Position), trailing: tokenizer.ConsumeTrivia());
        }

        if (currChar == '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = new List<CommentTriviaToken>();

            while (input.Consume(out currChar) && !(currChar == '*' && input.Peek() == '/')) {
                if (currChar == '/' && input.Peek() == '*') {

                    // reconsume the '/'
                    input.Reconsume();

                    inner.Add(Consume(input, tokenizer, isInner: true));

                    strBuilder.Append(inner[^1].Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (input.Current == '\0' || input.Current == '\u0003') {
                Logger.Error(new UnexpectedEOFException(
                    context: "in a multi-line comment",
                    expected: "the comment delimiter '*/'",
                    new LocationRange(startingPosition, input.Position)
                ));

                isValid = false;
            }

            // consumes the remaining '/'
            input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position),
                isValid: isValid,
                inner: inner,
                // if this comment is a comment *inside* a comment, then don't consume anything afterwards
                trailing: isInner ? TriviaToken.NULL : tokenizer.ConsumeTrivia()
            );
        }

        throw Logger.Fatal(new InvalidCallException(new LocationRange(startingPosition, input.Position)));
    }

    public CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer)
        => Consume(input, tokenizer, isInner: false);
}