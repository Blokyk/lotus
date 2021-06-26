internal sealed class TopLevelPrinter : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node) => ASTHelper.PrintToken(node.Token);


    public string Visit(FromNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.OriginName);

    public string Visit(TopLevelStatementNode node)
        => ASTHelper.PrintStatement(node.Statement);

    public string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTHelper.PrintToken(node.Token)
         + Utilities.Join(",", ASTHelper.PrintValue, node.ImportsName);

    public string Visit(NamespaceNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.NamespaceName);

    public string Visit(UsingNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.ImportName);


    public string Print(TopLevelNode node) => node.Accept(this);
}