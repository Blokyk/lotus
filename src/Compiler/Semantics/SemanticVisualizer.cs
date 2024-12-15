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
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ErrorSymbolInfo symbol) => Default(symbol);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(TypedSymbolInfo symbol) => Default(symbol);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(NamespaceInfo ns) {
        _writer.WriteLine($"namespace {ns.Name} {{");

        using (_writer.Indent()) {
            foreach (var childNs in ns.Namespaces) {
                Write(childNs);
                _writer.WriteLine();
            }

            foreach (var type in ns.Types.Where(t => t.SpecialType is SpecialType.None)) {
                Write(type);
                _writer.WriteLine();
            }

            foreach (var func in ns.Functions) {
                Write(func);
                _writer.WriteLine();
            }
        }

        _writer.WriteLine($"}} // {ns.Name}");
        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(TypeInfo symbol) => Default(symbol);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ArrayTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UnionTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ErrorTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(UserTypeInfo symbol) => Default(symbol);

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(BoolTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(CharTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(NumberTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(StringTypeInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(VoidTypeInfo symbol) => Default(symbol);


    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(EnumTypeInfo symbol) {
        _writer.WriteLine($"enum {symbol.Name} {{");

        using (_writer.Indent()) {
            foreach (var value in symbol.Values)
                Write(value);
        }

        _writer.WriteLine($"}} // {symbol.Name}");
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

        _writer.WriteLine("} // " + symbol.Name);
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
            _writer.WriteLine();
            using(_writer.Indent()) {
                foreach (var param in symbol.Parameters)
                    Write(param);
            }
        }

        _writer.Write("): ");

        ArgumentNullException.ThrowIfNull(symbol.ReturnType);
        _writer.Write(SymbolFormatter.Format(symbol.ReturnType));

        _writer.WriteLine();

        return _writer;
    }

    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(ParameterInfo symbol) => Default(symbol);
    IndentedTextWriter ISymbolVisitor<IndentedTextWriter>.Visit(LocalInfo symbol) => Default(symbol);
}