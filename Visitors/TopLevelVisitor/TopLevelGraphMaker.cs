internal class TopLevelGraphMaker : ITopLevelVisitor<GraphNode>
{

    protected readonly (string tooltip, string color) From = ("from statement", "navy");
    protected readonly (string tooltip, string color) FromOrigin = ("origin name", "");

    protected readonly (string tooltip, string color) Import = ("import statement", "fuchsia");
    protected readonly (string tooltip, string color) ImportNames = ("import names", "peru");

    protected readonly (string tooltip, string color) Namespace = ("namespace declaration", "cornflowerblue");
    protected readonly (string tooltip, string color) NamespaceName = ("namespace name", "");

    protected readonly (string tooltip, string color) Using = ("UsingNode", "");


    protected readonly (string tooltip, string color) TopLevel = ("TopLevelNode", "black");


    public GraphNode Default(TopLevelNode node)
        =>  new GraphNode(node.GetHashCode(), node.Token.Representation)
                .SetColor(TopLevel.color)
                .SetTooltip(TopLevel.tooltip);

    public GraphNode Visit(TopLevelStatementNode node)
        => ASTHelper.ToGraphNode(node.Statement);

    public GraphNode Visit(FromNode node)
        => new GraphNode(node.GetHashCode(), "from") {
               ASTHelper.ToGraphNode(node.OriginName)
                   .SetTooltip("origin name")
           }.SetColor(From.color)
            .SetTooltip(From.tooltip);

    public GraphNode Visit(ImportNode node) {
        var root = new GraphNode(node.GetHashCode(), "import") {
            ASTHelper.ToGraphNode(node.FromStatement)
        }.SetColor(Import.color)
         .SetTooltip(Import.tooltip);

        var importsNode = new GraphNode(node.ImportsName.GetHashCode(), "import\\nnames")
            .SetColor(ImportNames.color)
            .SetTooltip(ImportNames.tooltip);

        foreach (var import in node.ImportsName) {
            importsNode.Add(ASTHelper.ToGraphNode(import));
        }

        root.Add(importsNode);

        return root;
    }

    public GraphNode Visit(NamespaceNode node)
        => new GraphNode(node.GetHashCode(), "namespace") {
                ASTHelper.ToGraphNode(node.NamespaceName).SetTooltip("namespace name")
            }.SetColor(Namespace.color)
             .SetTooltip(Namespace.tooltip);

    public GraphNode Visit(UsingNode node)
        => new GraphNode(node.GetHashCode(), "using") {
                ASTHelper.ToGraphNode(node.ImportName)
            }.SetColor(Using.color)
             .SetTooltip(Using.tooltip);

    public GraphNode ToGraphNode(TopLevelNode node) => node.Accept(this);
}