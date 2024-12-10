using System.Text;

namespace Lotus.Text;

internal sealed class MarkupBuilder
{
    private readonly MarkupChain _list;

    private static readonly Markup.StyleMarker STYLE_POP = Markup.StyleMarker.Reset;
    private static readonly Markup.ColorMarker FGCOLOR_POP = Markup.ColorMarker.ResetForeground;
    private static readonly Markup.ColorMarker BGCOLOR_POP = Markup.ColorMarker.ResetBackground;
    private static readonly Markup.TextFormatMarker FMT_POP = Markup.TextFormatMarker.None;

    internal MarkupBuilder() {
        _list = new MarkupChain();
    }

    internal MarkupBuilder(string text) : this() {
        Append(text);
    }

    public MarkupBuilder PushStyle(Style style) {
        _list.AddLast(new Markup.StyleMarker(style));
        return this;
    }

    public MarkupBuilder PopStyle() {
        _list.AddLast(STYLE_POP);
        return this;
    }

    public MarkupBuilder PushTextFormat(TextFormat format) {
        _list.AddLast(new Markup.TextFormatMarker(format));
        return this;
    }

    public MarkupBuilder PopTextFormat() {
        _list.AddLast(FMT_POP);
        return this;
    }

    public MarkupBuilder PushColor(TextColor color, bool asBackground) {
        _list.AddLast(new Markup.ColorMarker(color, asBackground));
        return this;
    }

    public MarkupBuilder PopColor(bool asBackground) {
        _list.AddLast(asBackground ? BGCOLOR_POP : FGCOLOR_POP);
        return this;
    }

    public MarkupBuilder PushForeground(TextColor color)
        => PushColor(color, false);

    public MarkupBuilder PopForeground()
        => PopColor(false);

    public MarkupBuilder PushBackground(TextColor color)
        => PushColor(color, true);

    public MarkupBuilder PopBackground()
        => PopColor(true);

    public MarkupBuilder Append(string? text) {
        if (text is not null)
            _list.AddLast(new Markup.Text(text));

        return this;
    }

    private MarkupBuilder Append(MarkupChain.MarkupNode node) {
        _list.AddLast(node);
        return this;
    }

    public MarkupBuilder Append(MarkupBuilder markups) {
        if (markups._list.First is null)
            return this;

        _list.Concat(markups._list);

        return this;
    }

    public MarkupBuilder Append(string? text, Style style)
        => text is not null
            ? PushStyle(style)
                .Append(text)
                .PopStyle()
            : this;

    public MarkupBuilder Append(MarkupBuilder markups, Style style)
        => markups._list.First is not null
            ? PushStyle(style)
                .Append(markups)
                .PopStyle()
            : this;

    public MarkupBuilder Append(string? text, TextColor color, bool asBackground = false)
        => text is not null
            ? PushColor(color, asBackground)
                .Append(text)
                .PopColor(asBackground)
            : this;

    public MarkupBuilder Append(MarkupBuilder markups, TextColor color, bool asBackground = false)
        => markups._list.First is not null
            ? PushColor(color, asBackground)
                .Append(markups)
                .PopColor(asBackground)
            : this;

    public MarkupBuilder Append(string? text, TextFormat format)
        => text is not null
            ? PushTextFormat(format)
                .Append(text)
                .PopTextFormat()
            : this;

    public MarkupBuilder Append(MarkupBuilder markups, TextFormat format)
        => markups._list.First is not null
            ? PushTextFormat(format)
                .Append(markups)
                .PopTextFormat()
            : this;

    public MarkupBuilder Append(char text) => Append(text.ToString());
    public MarkupBuilder Append(object obj) => Append(obj.ToString());

    public MarkupBuilder Append(char text, Style style) => Append(text.ToString(), style);
    public MarkupBuilder Append(object obj, Style style) => Append(obj.ToString(), style);

    public MarkupBuilder Append(char text, TextColor color) => Append(text.ToString(), color);
    public MarkupBuilder Append(object obj, TextColor color) => Append(obj.ToString(), color);

    public MarkupBuilder Append(char text, TextFormat format) => Append(text.ToString(), format);
    public MarkupBuilder Append(object obj, TextFormat format) => Append(obj.ToString(), format);

    public MarkupBuilder AppendLine() => Append("\n");
    public MarkupBuilder AppendLine(string text) => Append(text + '\n');
    public MarkupBuilder AppendLine(char text) => Append(text + "\n");
    public MarkupBuilder AppendLine(object obj) => Append(obj.ToString() + '\n');
    public MarkupBuilder AppendLine(MarkupBuilder markups) => Append(markups.AppendLine());

    public MarkupBuilder AppendLine(string text, Style style) => Append(text + '\n', style);
    public MarkupBuilder AppendLine(char text, Style style) => Append(text + "\n", style);
    public MarkupBuilder AppendLine(object obj, Style style) => Append(obj.ToString() + '\n', style);
    public MarkupBuilder AppendLine(MarkupBuilder markups, Style style) => Append(markups.AppendLine(), style);

    public MarkupBuilder AppendLine(string text, TextColor color) => Append(text + '\n', color);
    public MarkupBuilder AppendLine(char text, TextColor color) => Append(text + "\n", color);
    public MarkupBuilder AppendLine(object obj, TextColor color) => Append(obj.ToString() + '\n', color);
    public MarkupBuilder AppendLine(MarkupBuilder markups, TextColor color) => Append(markups.AppendLine(), color);

    public MarkupBuilder AppendLine(string text, TextFormat format) => Append(text + '\n', format);
    public MarkupBuilder AppendLine(char text, TextFormat format) => Append(text + "\n", format);
    public MarkupBuilder AppendLine(object obj, TextFormat format) => Append(obj.ToString() + '\n', format);
    public MarkupBuilder AppendLine(MarkupBuilder markups, TextFormat format) => Append(markups.AppendLine(), format);

    public override string ToString() {
        var sb = new StringBuilder();

        foreach (var node in _list.OfType<Markup.Text>()) {
            sb.Append(node.Content);
        }

        return sb.ToString();
    }

    public string Render() {
        var sb = new StringBuilder();

        var styleStack = new Stack<Markup.StyleMarker>();
        var fgColorStack = new Stack<Markup.ColorMarker>();
        var bgColorStack = new Stack<Markup.ColorMarker>();
        var fmtStack = new Stack<Markup.TextFormatMarker>();

        void ReapplyStacks() {
            // reset all styles
            sb.Append(Markup.StyleMarker.Reset);

            if (styleStack.TryPeek(out var s)) {
                sb.Append(s);
            }

            if (fgColorStack.TryPeek(out var fc)) {
                sb.Append(fc);
            }

            if (bgColorStack.TryPeek(out var bc)) {
                sb.Append(bc);
            }

            if (fmtStack.TryPeek(out var f)) {
                sb.Append(f);
            }
        }

        foreach (var node in _list) {
            if (node is Markup.Text) {
                sb.Append(node);
                continue;
            }

            if (node is Markup.ColorMarker cm) {
                if (cm.IsBackground) {
                    if (cm != BGCOLOR_POP) {
                        bgColorStack.Push(cm);
                        sb.Append(cm);
                        continue;
                    }

                    _ = bgColorStack.TryPop(out _);
                    ReapplyStacks();
                } else {
                    if (cm != FGCOLOR_POP) {
                        fgColorStack.Push(cm);
                        sb.Append(cm);
                        continue;
                    }

                    _ = fgColorStack.TryPop(out _);
                    ReapplyStacks();
                }
            } else if (node is Markup.StyleMarker sm) {
                if (sm != STYLE_POP) {
                    styleStack.Push(sm);
                    sb.Append(sm);
                    continue;
                }

                _ = styleStack.TryPop(out _);
                ReapplyStacks();
            } else if (node is Markup.TextFormatMarker tfm) {
                if (tfm != FMT_POP) {
                    fmtStack.Push(tfm);
                    sb.Append(tfm);
                    continue;
                }

                _ = fmtStack.TryPop(out _);
                ReapplyStacks();
            }
        }

        return sb.ToString();
    }
}