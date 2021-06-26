public interface ITopLevelVisitor<T>
{
    T Default(TopLevelNode node);


    T Visit(TopLevelNode node) => Default(node);
    T Visit(TopLevelStatementNode node) => Default(node);
    T Visit(FromNode node) => Default(node);
    T Visit(ImportNode node) => Default(node);
    T Visit(NamespaceNode node) => Default(node);
    T Visit(UsingNode node) => Default(node);
}