using System.Threading.Tasks;

using System.CommandLine;

using Lotus.Extras;
using Lotus.Extras.Graphs;
using Lotus.Semantics;
using Lotus.Syntax;
using System.Threading;
using Lotus.Error;

partial class Program
{
    static readonly FileInfo _sourceCodeFile;

    static RootCommand BuildRootCommand() {
        var forceOption = new Option<bool>("--force", "Ignore compilation errors before executing commands");
        forceOption.AddAlias("-f");

        var fileArgument = new Argument<FileInfo>("input", "The file to read code from, or stdin if '-'");
        fileArgument.SetDefaultValue(_sourceCodeFile);
        fileArgument.Arity = ArgumentArity.ZeroOrOne;
        fileArgument.AddValidator(result => {
            if (result.Tokens.Count == 0)
                return;

            var filename = result.Tokens[0].Value;

            if (filename != "-" && !File.Exists(filename)) {
                result.ErrorMessage = result.LocalizationResources.FileDoesNotExist(filename);
            }
        });

        /*
        *   lotus silent [file.txt]
        */

        var silentVerb = new Command("silent", "Don't print anything to stdout (errors go to stderr)");
        silentVerb.AddArgument(fileArgument);
        silentVerb.SetHandler(
            (file, _) => Task.FromResult(HandleParsing(file, out var _)),
            fileArgument,
            forceOption
        );

        /*
        *   lotus print [file.txt]
        */

        var printVerb = new Command("print", "Reconstruct the source file from the AST and print it");
        printVerb.AddArgument(fileArgument);
        printVerb.SetHandler(PrintHandler, fileArgument, forceOption);

        /*
        *   lotus hash [file.txt]
        */

        var hashVerb = new Command("hash", "Print the hash of the AST graph") {
            fileArgument
        };

        hashVerb.SetHandler(
            GraphHandlerFactory(
                g => Console.WriteLine(Graph.StructuralComparer.GetHashCode(g))
            ),
            fileArgument,
            forceOption
        );

        /*
        *   lotus graph [file.txt]
        */

        var graphVerb = new Command("graph", "Print graphviz code for the AST graph") {
            fileArgument
        };

        graphVerb.SetHandler(
            GraphHandlerFactory(
                g => Console.WriteLine(g.ToText())
            ),
            fileArgument,
            forceOption
        );

        var bindVerb = new Command("bind") {
            fileArgument
        };

        bindVerb.SetHandler(BindHandler, fileArgument);

        /*
        *   lotus [--force] {silent, print, hash, graph}
        */

        var rootCommand = new RootCommand("A lotus parser/typechecker") {
            silentVerb,
            printVerb,
            hashVerb,
            graphVerb,
            bindVerb
        };

        rootCommand.AddGlobalOption(forceOption);

        rootCommand.Name = "lotus";
        rootCommand.TreatUnmatchedTokensAsErrors = true;

        return rootCommand;
    }

    static Func<FileInfo, bool, Task<int>> GraphHandlerFactory(Action<Graph> act)
        => (file, force) => {
            var g = MakeGraph(file, force, out int exitCode);

            if (exitCode != 0 && !force) {
                return Task.FromResult(exitCode);
            }

            act(g);
            return Task.FromResult(0);
        };

    static Task<int> BindHandler(FileInfo file) {
        using var stream = GetStreamForFile(file);
        var tree = new SyntaxTree(stream);

        if (!tree.IsValid) {
            Logger.PrintAllErrors();
            return Task.FromResult(1);
        }

        var sem = new SemanticUnit([tree]);

        if (!sem.IsValid) {
            Logger.PrintAllErrors();
            return Task.FromResult(1);
        }

        Console.WriteLine(ExtraUtils.PrintDeclarations(sem));

        return Task.FromResult(0);
    }

    static Task<int> PrintHandler(FileInfo file, bool force) {
        // var stream = GetStreamForFile(file);

        // while (stream.TryConsumeChar(out char c))
        //     Console.Write(c);
        // return Task.FromResult(0);

        var tokenizer = GetTokenizerForFile(file);
        var exitCode = HandleParsing(tokenizer, out var tlNodes);

        if (exitCode != 0 && !force) {
            return Task.FromResult(exitCode);
        }

        foreach (var node in tlNodes) {
            Console.Write(ExtraUtils.PrintTopLevel(node));
        }

        return Task.FromResult(0);
    }
}