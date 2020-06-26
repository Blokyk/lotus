using System;
using System.IO;
using System.Reflection;

#pragma warning disable
class Program
{
    static void Main(string[] _) {
#if VS
		// Lil hack for our visual studio (win and mac) users, whose IDE thinks it's a rebel
        // because it doesn't use the same working directory as literally every other
        // major IDE + the official fucking CLI. Used to love vs 2019, but honestly
        // I think I'm switching to vs code for most things and not loooking back.

        Directory.SetCurrentDirectory(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
				.Parent
				.Parent
				.FullName
        );
#endif
        var file = new FileInfo(Directory.GetCurrentDirectory() + "/test.txt");

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new LotusTokenizer(file);

        /*tokenizer = new LotusTokenizer(@"
        while (isOk(i)) {
            if (i % 30 == 0) continue;
            i = i + rnd.Next();
        }
        ");*/

        var parser = new LotusParser(tokenizer);

        var g = new Graph("AST");

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {parser.Position.filename}\\n\\n");
        g.AddGraphProp("labelloc", "top");

        while (parser.Consume(out StatementNode node)) {
            g.AddNode(node.ToGraphNode());
        }

        Console.Write(g.ToText());
    }
}
#pragma warning restore