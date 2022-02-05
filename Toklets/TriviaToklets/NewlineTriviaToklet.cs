
public sealed class NewlineTriviaToklet : ITriviaToklet<NewlineTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => input.Consume() is '\n');

    public NewlineTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        if (input.Consume() is not '\n')
            throw Logger.Fatal(new InvalidCallException(new LocationRange(startingPosition, input.Position)));

        int charCounter = 1;

        // we don't need to check if there's a '\r' before cause it's always followed by a '\n' anyway
        while (input.Consume(out char currChar) && currChar == '\n') charCounter++;

        input.Reconsume();

        return new NewlineTriviaToken(
            charCounter,
            new LocationRange(startingPosition, input.Position)
        ) { TrailingTrivia = tokenizer.ConsumeTrivia() };
    }
}