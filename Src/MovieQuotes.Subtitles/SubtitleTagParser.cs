namespace MovieQuotes.Subtitles;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using MovieQuotes.Subtitles.Models;

internal static class SubtitleTagParser
{
    public static bool TryParse(
        string value,
        [NotNullWhen(true)] out SubtitleTagToken? tag)
    {
        tag = null;
        var index = 1;
        var end = value.Length - 1;

        var lastContentCharacter = end - 1;
        while (lastContentCharacter >= index &&
               char.IsWhiteSpace(value[lastContentCharacter]))
        {
            lastContentCharacter--;
        }

        var isSelfClosing = lastContentCharacter >= index &&
                            value[lastContentCharacter] == '/';
        if (isSelfClosing)
            end = lastContentCharacter;

        SkipWhitespace(value, ref index, end);

        var isClosing = index < end && value[index] == '/';
        if (isClosing)
        {
            index++;
            SkipWhitespace(value, ref index, end);
        }

        var nameStart = index;
        while (index < end && IsTagNameCharacter(value[index]))
            index++;

        if (nameStart == index)
            return false;

        var name = value[nameStart..index];
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        while (index < end)
        {
            SkipWhitespace(value, ref index, end);
            if (index >= end)
                break;

            if (value[index] == '/')
            {
                index++;
                SkipWhitespace(value, ref index, end);
                if (index != end)
                    return false;

                break;
            }

            var attributeStart = index;
            while (index < end &&
                   !char.IsWhiteSpace(value[index]) &&
                   value[index] is not '=' and not '/' and not '>')
            {
                index++;
            }

            if (attributeStart == index)
                return false;

            var attributeName = value[attributeStart..index];
            SkipWhitespace(value, ref index, end);

            var attributeValue = string.Empty;
            if (index < end && value[index] == '=')
            {
                index++;
                SkipWhitespace(value, ref index, end);

                if (index >= end)
                    return false;

                if (value[index] is '"' or '\'')
                {
                    var quote = value[index++];
                    var attributeValueStart = index;

                    while (index < end && value[index] != quote)
                        index++;

                    if (index >= end)
                        return false;

                    attributeValue = value[attributeValueStart..index];
                    index++;
                }
                else
                {
                    var attributeValueStart = index;
                    while (index < end && !char.IsWhiteSpace(value[index]))
                        index++;

                    attributeValue = value[attributeValueStart..index];
                    if (attributeValue.Length == 0)
                        return false;
                }
            }

            attributes[attributeName] = attributeValue;
        }

        if (isClosing && (isSelfClosing || attributes.Count > 0))
            return false;

        tag = new SubtitleTagToken(
            value,
            name,
            isClosing,
            isSelfClosing,
            new ReadOnlyDictionary<string, string>(attributes),
            Array.Empty<SubtitleToken>());
        return true;
    }

    private static bool IsTagNameCharacter(char value) =>
        char.IsLetterOrDigit(value) || value is ':' or '-' or '_';

    private static void SkipWhitespace(string value, ref int index, int end)
    {
        while (index < end && char.IsWhiteSpace(value[index]))
            index++;
    }
}
