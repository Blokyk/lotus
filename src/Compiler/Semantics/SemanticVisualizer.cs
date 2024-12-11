using System.CodeDom.Compiler;
using System.IO;

namespace Lotus.Semantics;

// todo: rewrite this to use IndentedStringBuilder
internal class SemanticVisualizer : ISymbolVisitor<IndentedTextWriter>
{
    private readonly IndentedTextWriter _writer;

    private SemanticVisualizer(TextWriter backingWriter)
        => _writer = new(backingWriter);

    public static string Format(SymbolInfo symbol) {
        using var strWriter = new StringWriter();
        var formatter = new SemanticVisualizer(strWriter);
        _ = symbol.Accept(formatter);
        return strWriter.ToString();
    }

    private void Write(SymbolInfo symbol) => symbol.Accept(this);

    IndentedTextWriter Default(SymbolInfo symbol) {
        _writer.WriteLine(SymbolFormatter.Format(symbol));
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(SymbolInfo symbol) => Default(symbol);

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

            foreach (var func in ns.Functions) {
                Write(func);
                _writer.WriteLineNoTabs();
            }
        }

        _writer.WriteLine('}');
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(TypeInfo symbol) => Default(symbol);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ArrayTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UnionTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(MissingTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UserTypeInfo symbol) => Default(symbol);

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
        _writer.WriteLine($"struct {SymbolFormatter.Format(symbol)} {{");

        using (_writer.Indent()) {
            foreach (var field in symbol.Fields)
                Write(field);
        }

        _writer.WriteLine('}');
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(FieldInfo symbol) {
        _writer.Write(symbol.Name + ": " + SymbolFormatter.Format(symbol.Type));
        if (symbol.HasDefaultValue)
            _writer.Write(" = " + symbol.DefaultValue);
        _writer.WriteLine();
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(FunctionInfo symbol) {
        _writer.Write("func " + symbol.Name + "(");
        if (symbol.Parameters.Count != 0) {
            _writer.WriteLineNoTabs();
            using(_writer.Indent()) {
                foreach (var param in symbol.Parameters)
                    Write(param);
            }
        }
        _writer.Write("): ");
        Write(symbol.ReturnType);

        _writer.WriteLineNoTabs();

        return _writer;
    }
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ParameterInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(LocalInfo symbol) => Default(symbol);
}