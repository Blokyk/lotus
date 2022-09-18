namespace Lotus.Syntax;

public partial class Tokenizer : IConsumer<Token>
{
    private Token ConsumeTokenCore() {
        if (!_input.Consume(out char currChar))
            return ConsumeEOFToken(in currChar);

        switch (currChar) {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return ConsumeNumberToken();
            case '.':
                if (MiscUtils.IsAsciiDigit(_input.Peek()))
                    return ConsumeNumberToken();
                else
                    return ConsumeOperatorToken();
            case '_':
            case '@':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
            case 'K':
            case 'L':
            case 'M':
            case 'N':
            case 'O':
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
            case 'T':
            case 'U':
            case 'V':
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'k':
            case 'l':
            case 'm':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 'r':
            case 's':
            case 't':
            case 'u':
            case 'v':
            case 'w':
            case 'x':
            case 'y':
            case 'z':
                return ConsumeIdentToken();
            case '$' when _input.Peek() is '"' or '\'':
                return ConsumeComplexStringToken();
            case '"':
            case '\'':
                return ConsumeStringToken();
            case '+':
            case '-':
            case '*':
            case '/':
            case '%':
            case '^':
            case '=':
            case '>':
            case '<':
            case '!':
            case '?':
                return ConsumeOperatorToken();
            case '&':
                if (_input.Peek() is '&')
                    return ConsumeOperatorToken();
                goto default;
            case '|':
                if (_input.Peek() is '|')
                    return ConsumeOperatorToken();
                goto default;
            case ';':
                return ConsumeSemicolonToken();
            case ':':
                if (_input.Peek() is ':')
                    return ConsumeDoubleColonToken();
                goto default;
            default:
                return ConsumeDelimToken(in currChar);
        }
    }

    private NumberToken ConsumeNumberToken() {
        _input.Reconsume();
        return NumberToklet.Instance.Consume(_input, this);
    }

    private Token ConsumeIdentToken() {
        _input.Reconsume();
        return IdentToklet.Instance.Consume(_input, this);
    }

    private ComplexStringToken ConsumeComplexStringToken() {
        _input.Reconsume();
        return ComplexStringToklet.Instance.Consume(_input, this);
    }

    private StringToken ConsumeStringToken() {
        _input.Reconsume();
        return StringToklet.Instance.Consume(_input, this);
    }

    private OperatorToken ConsumeOperatorToken() {
        _input.Reconsume();
        return OperatorToklet.Instance.Consume(_input, this);
    }

    private Token ConsumeSemicolonToken()
        => new(";", TokenKind.semicolon, _input.Position);

    private Token ConsumeDoubleColonToken() {
        Debug.Assert(_input.Consume() == ':');
        // FIXME: position should be 2-char wide here
        return new Token("::", TokenKind.delimiter, _input.Position);
    }

    private Token ConsumeDelimToken(in char c)
        => new(c.ToString(), TokenKind.delimiter, _input.Position);

    private Token ConsumeEOFToken(in char c)
        => new(c.ToString(), TokenKind.EOF, _input.Position) { IsValid = false };
}