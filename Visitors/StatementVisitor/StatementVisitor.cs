public interface IStatementVisitor<T>
{
    T Default(StatementNode node);


    T Visit(StatementNode node) => Default(node);

    T Visit(BreakNode node) => Default(node);
    T Visit(ContinueNode node) => Default(node);
    T Visit(DeclarationNode node) => Default(node);
    T Visit(ElseNode node) => Default(node);
    T Visit(ForeachNode node) => Default(node);
    T Visit(ForNode node) => Default(node);
    T Visit(FunctionDeclarationNode node) => Default(node);
    T Visit(IfNode node) => Default(node);
    T Visit(PrintNode node) => Default(node);
    T Visit(ReturnNode node) => Default(node);
    T Visit(StatementExpressionNode node) => Default(node as StatementNode);
    T Visit(WhileNode node) => Default(node);


    T Visit(SimpleBlock block);
}