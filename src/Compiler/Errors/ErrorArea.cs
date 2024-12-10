namespace Lotus.Error;

[Flags]
public enum ErrorArea {
    Unknown = 0,
    Tokenizer = 1,
    Parser = 2,
    ASTHelper = 4,
    Semantics = 8,
    Binder = 16,
}

internal static class ErrorAreaExtension
{
    public static string ToDisplayName(this ErrorArea area)
        => area switch {
            ErrorArea.Semantics => "Semantic checking",
            ErrorArea.Binder => "Type checking",
            ErrorArea.ASTHelper => "AST Utilities",
            _ => Enum.GetName(area) ?? "unknown"
        };
}