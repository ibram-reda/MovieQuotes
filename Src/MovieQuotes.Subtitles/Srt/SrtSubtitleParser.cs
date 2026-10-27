namespace MovieQuotes.Subtitles.Srt;

using System.Globalization;
using System.Net;
using MovieQuotes.Subtitles.Abstractions;
using MovieQuotes.Subtitles.Models;

public sealed class SrtSubtitleParser : ISubtitleParser
{
    public SubtitleDocument Parse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var document = new SubtitleDocument();

        ProcessTokens(SubtitleTokenizer.Tokenize(input), document);

        return document;
    }

    private static void ProcessTokens(
        IReadOnlyList<SubtitleToken> tokens,
        SubtitleDocument document)
    {
        var pendingTokens = new Stack<PendingToken>();
        PushTokens(tokens, new RunStyle(), pendingTokens);

        while (pendingTokens.TryPop(out var pending))
        {
            switch (pending.Token)
            {
                case SubtitleToken textToken when textToken.Type is
                    SubtitleTokenType.Text or
                    SubtitleTokenType.Info or
                    SubtitleTokenType.BadToken:
                    AddTextRun(
                        document,
                        WebUtility.HtmlDecode(textToken.Value),
                        pending.Style);
                    break;

                case SubtitleToken { Type: SubtitleTokenType.NewLine }:
                    document.Runs.Add(new SubtitleRun(string.Empty, IsLineBreak: true));
                    break;

                case SubtitleTagToken tag:
                    ProcessTag(tag, pending.Style, document, pendingTokens);
                    break;

                case SubtitleToken { Type: SubtitleTokenType.Tag } tagToken:
                    AddTextRun(
                        document,
                        WebUtility.HtmlDecode(tagToken.Value),
                        pending.Style);
                    break;
            }
        }
    }

    private static void ProcessTag(
        SubtitleTagToken tag,
        RunStyle style,
        SubtitleDocument document,
        Stack<PendingToken> pendingTokens)
    {
        if (tag.IsClosing)
        {
            AddTextRun(document, WebUtility.HtmlDecode(tag.Value), style);
            return;
        }

        var name = tag.Name.ToLowerInvariant();

        if (name is "br")
        {
            document.Runs.Add(new SubtitleRun(string.Empty, IsLineBreak: true));
            return;
        }

        if (TryApplyStyle(name, tag.Attributes, style, out var updatedStyle))
        {
            if (tag.IsSelfClosing)
                return;

            PushTokens(tag.Children, updatedStyle, pendingTokens);
            return;
        }

        AddTextRun(document, WebUtility.HtmlDecode(tag.Value), style);

        if (tag.ClosingValue is not null)
        {
            pendingTokens.Push(new PendingToken(
                new SubtitleToken(SubtitleTokenType.Text, tag.ClosingValue),
                style));
        }

        PushTokens(tag.Children, style, pendingTokens);
    }

    private static void PushTokens(
        IReadOnlyList<SubtitleToken> tokens,
        RunStyle style,
        Stack<PendingToken> pendingTokens)
    {
        for (var index = tokens.Count - 1; index >= 0; index--)
            pendingTokens.Push(new PendingToken(tokens[index], style));
    }

    private static bool TryApplyStyle(
        string name,
        IReadOnlyDictionary<string, string> attributes,
        RunStyle style,
        out RunStyle updatedStyle)
    {
        updatedStyle = name switch
        {
            "b" or "strong" => style with { Bold = true },
            "i" or "em" => style with { Italic = true },
            "u" => style with { Underline = true },
            "font" => ApplyFontStyle(attributes, style),
            _ => style
        };

        return name is "b" or "strong" or "i" or "em" or "u" or "font";
    }

    private static RunStyle ApplyFontStyle(
        IReadOnlyDictionary<string, string> attributes,
        RunStyle style)
    {
        var fontSize = attributes.GetValueOrDefault("size");

        return style with
        {
            FontFamily = attributes.GetValueOrDefault("face") ?? style.FontFamily,
            Foreground = attributes.GetValueOrDefault("color") ?? style.Foreground,
            FontSize = fontSize is not null &&
                       double.TryParse(
                           fontSize,
                           NumberStyles.Float,
                           CultureInfo.InvariantCulture,
                           out var parsedSize) &&
                       double.IsFinite(parsedSize) &&
                       parsedSize > 0
                ? parsedSize
                : style.FontSize
        };
    }

    private static void AddTextRun(
        SubtitleDocument document,
        string text,
        RunStyle style)
    {
        if (text.Length == 0)
            return;

        if (document.Runs.Count > 0)
        {
            var previous = document.Runs[^1];

            if (!previous.IsLineBreak &&
                previous.Bold == style.Bold &&
                previous.Italic == style.Italic &&
                previous.Underline == style.Underline &&
                previous.FontFamily == style.FontFamily &&
                previous.FontSize == style.FontSize &&
                previous.Foreground == style.Foreground)
            {
                document.Runs[^1] = previous with { Text = previous.Text + text };
                return;
            }
        }

        document.Runs.Add(new SubtitleRun(
            text,
            style.Bold,
            style.Italic,
            style.Underline,
            style.FontFamily,
            style.FontSize,
            style.Foreground));
    }

    private sealed record RunStyle(
        bool Bold = false,
        bool Italic = false,
        bool Underline = false,
        string? FontFamily = null,
        double? FontSize = null,
        string? Foreground = null);

    private sealed record PendingToken(SubtitleToken Token, RunStyle Style);
}