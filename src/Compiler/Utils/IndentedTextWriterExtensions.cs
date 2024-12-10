using System.CodeDom.Compiler;

namespace Lotus.Utils;
internal static class IndentedTextWriterExtensions
{
    public struct IndentationScope : IDisposable
    {
        private IndentedTextWriter? _writer;

        public IndentationScope(IndentedTextWriter writer) {
            _writer = writer;
            _writer.Indent++;
        }

        public void Dispose() {
            if (_writer is null)
                return;

            _writer.Indent--;
            _writer = null;
        }
    }

    public static IndentationScope Indent(this IndentedTextWriter writer)
        => new(writer);

    public static void WriteLineNoTabs(this IndentedTextWriter writer)
        => writer.WriteLineNoTabs("");
}
