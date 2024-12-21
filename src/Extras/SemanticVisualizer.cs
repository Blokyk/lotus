using System.CodeDom.Compiler;

namespace Lotus.Semantics;

// todo: rewrite this to use IndentedStringBuilder
internal class SemanticVisualizer
{
    private readonly IndentedTextWriter _writer;

    private SemanticVisualizer(TextWriter backingWriter)
        => _writer = new(backingWriter);

    public static string Print(SemanticUnit unit) {
        using var strWriter = new StringWriter();
        var formatter = new SemanticVisualizer(strWriter);
        formatter.Write(unit.Global);
        return strWriter.ToString();
    }

    private void Write(SymbolInfo symbol) {
        switch (symbol) {
            case NamespaceInfo ns:
                Write(ns);
                break;
            case EnumTypeInfo en:
                Write(en);
                break;
            case StructTypeInfo st:
                Write(st);
                break;
            case FunctionInfo fn:
                Write(fn);
                break;
            case ErrorTypeInfo er:
                Write(er);
                break;
            default:
                _writer.WriteLine(SymbolFormatter.Format(symbol));
                break;
        }
    }

    void Write(NamespaceInfo ns) {
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
    }

    void Write(ErrorTypeInfo symbol) {

    }

    void Write(EnumTypeInfo symbol) {
        _writer.WriteLine($"enum {symbol.Name} {{");

        using (_writer.Indent()) {
            foreach (var value in symbol.Values) {
                _writer.Write(value.Name);

                if (value.Value.HasValue)
                    _writer.Write($" = {value.Value.Value}");

                _writer.WriteLine();
            }
        }

        _writer.WriteLine($"}} // {symbol.Name}");
    }

    void Write(StructTypeInfo symbol) {
        _writer.WriteLine($"struct {symbol.Name} {{");

        using (_writer.Indent()) {
            foreach (var field in symbol.Fields) {
                _writer.Write(field.Name + ": " + SymbolFormatter.Format(field.Type));
                if (field.HasDefaultValue)
                    _writer.Write(" = " + field.DefaultValue);
                _writer.WriteLine();
            }
        }

        _writer.WriteLine("} // " + symbol.Name);
    }

    void Write(FunctionInfo symbol) {
        _writer.Write("func " + symbol.Name + "(");
        if (symbol.Parameters.Count != 0) {
            _writer.WriteLine();
            using(_writer.Indent()) {
                foreach (var param in symbol.Parameters)
                    Write(param);
            }
        }

        _writer.Write("): " + SymbolFormatter.Format(symbol.ReturnType));

        _writer.WriteLine();
    }
}