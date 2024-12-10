using System.CodeDom.Compiler;
using System.IO;

namespace Lotus.Semantics;

// todo: rewrite this to use IndentedStringBuilder
internal class SemanticFormatter : ISymbolVisitor<IndentedTextWriter>
{
    private readonly IndentedTextWriter _writer;

    private SemanticFormatter(TextWriter backingWriter)
        => _writer = new(backingWriter);

    public static string Format(SymbolInfo symbol) {
        using var strWriter = new StringWriter();
        var formatter = new SemanticFormatter(strWriter);
        _ = symbol.Accept(formatter);
        return strWriter.ToString();
    }

    public void Write(SymbolInfo symbol) => symbol.Accept(this);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(SymbolInfo symbol)
        => _writer;

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(NamespaceInfo ns) {
        _writer.WriteLine($"namespace {ns.Name} {{");

        using (_writer.Indent()) {
            foreach (var childNs in ns.Namespaces) {
                Write(childNs);
                _writer.WriteLineNoTabs();
            }

            foreach (var type in ns.Types) {
                Write(type);
                _writer.WriteLineNoTabs();
            }
        }

        _writer.WriteLine('}');
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(TypeInfo symbol)
        => _writer;

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ArrayTypeInfo symbol) => throw new NotImplementedException();
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UnionTypeInfo symbol) => throw new NotImplementedException();
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(MissingTypeInfo symbol) => throw new NotImplementedException();
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UserTypeInfo symbol)
        => _writer;

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(EnumTypeInfo symbol) {
        _writer.WriteLine($"enum {symbol.Name} {{");

        using (_writer.Indent()) {
            foreach (var value in symbol.Values)
                Write(value);
        }

        _writer.WriteLine('}');
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(EnumValueInfo symbol) {
        _writer.Write(symbol.Name);

        if (symbol.Value.HasValue)
            _writer.Write($" = {symbol.Value.Value}");

        _writer.WriteLine();
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(StructTypeInfo symbol) {
        _writer.WriteLine($"struct {symbol.Name} {{");

        using (_writer.Indent()) {
            foreach (var field in symbol.Fields)
                Write(field);
        }

        _writer.WriteLine('}');
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(FieldInfo symbol) {
        _writer.WriteLine($"{symbol.Name}: {symbol.Type}");
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(MethodInfo symbol) => throw new NotImplementedException();
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ParameterInfo symbol) => throw new NotImplementedException();
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(LocalInfo symbol) => throw new NotImplementedException();
}