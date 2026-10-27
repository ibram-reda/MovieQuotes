namespace MovieQuotes.Subtitles;

using MovieQuotes.Subtitles.Models;

public static class SubtitleTokenizer
{
    public static IReadOnlyList<SubtitleToken> Tokenize(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var scanner = new Scanner(input);
        return BuildTagTree(scanner.ReadTokens());
    }

    private static IReadOnlyList<SubtitleToken> BuildTagTree(
        IReadOnlyList<SubtitleToken> tokens)
    {
        var roots = new List<SubtitleToken>();
        var openTags = new List<TagFrame>();
        var openTagIndexes = new Dictionary<string, List<int>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var token in tokens)
        {
            if (token is not SubtitleTagToken tag)
            {
                AppendToken(token, roots, openTags);
                continue;
            }

            if (tag.IsClosing)
            {
                if (!openTagIndexes.TryGetValue(tag.Name, out var matchingIndexes))
                {
                    AppendToken(tag, roots, openTags);
                    continue;
                }

                var matchingIndex = matchingIndexes[^1];
                while (openTags.Count > matchingIndex)
                {
                    var frame = openTags[^1];
                    openTags.RemoveAt(openTags.Count - 1);
                    RemoveOpenTagIndex(frame.Tag.Name, openTagIndexes);
                    AppendToken(
                        frame.ToToken(
                            closingValue: openTags.Count == matchingIndex
                                ? tag.Value
                                : null),
                        roots,
                        openTags);
                }

                continue;
            }

            if (!tag.IsSelfClosing &&
                !tag.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
            {
                if (!openTagIndexes.TryGetValue(tag.Name, out var indexes))
                {
                    indexes = [];
                    openTagIndexes.Add(tag.Name, indexes);
                }

                indexes.Add(openTags.Count);
                openTags.Add(new TagFrame(tag));
                continue;
            }

            AppendToken(tag, roots, openTags);
        }

        while (openTags.Count > 0)
        {
            var frame = openTags[^1];
            openTags.RemoveAt(openTags.Count - 1);
            AppendToken(frame.ToToken(closingValue: null), roots, openTags);
        }

        return roots.AsReadOnly();
    }

    private static void RemoveOpenTagIndex(
        string name,
        Dictionary<string, List<int>> openTagIndexes)
    {
        var indexes = openTagIndexes[name];
        indexes.RemoveAt(indexes.Count - 1);

        if (indexes.Count == 0)
            openTagIndexes.Remove(name);
    }

    private static void AppendToken(
        SubtitleToken token,
        List<SubtitleToken> roots,
        List<TagFrame> openTags)
    {
        if (openTags.Count == 0)
            roots.Add(token);
        else
            openTags[^1].Children.Add(token);
    }

    private sealed class TagFrame(SubtitleTagToken tag)
    {
        public SubtitleTagToken Tag { get; } = tag;

        public List<SubtitleToken> Children { get; } = [];

        public SubtitleTagToken ToToken(string? closingValue) =>
            Tag with
            {
                Children = Array.AsReadOnly(Children.ToArray()),
                ClosingValue = closingValue
            };
    }

    private sealed class Scanner(string input)
    {
        private int _position;

        public IReadOnlyList<SubtitleToken> ReadTokens()
        {
            var tokens = new List<SubtitleToken>();

            while (_position < input.Length)
            {
                var current = input[_position];

                if (current == '<')
                    ReadTag(tokens);
                else if (current is '\r' or '\n')
                    ReadNewLine(tokens);
                else if (current == '[')
                    ReadInfo(tokens,ending:']');
                else if(current =='(')
                    ReadInfo(tokens,ending:')');
                else if(current =='{')
                    ReadInfo(tokens,ending:'}');
                else
                    ReadText(tokens);
            }

            return tokens;
        }

        private void ReadTag(List<SubtitleToken> tokens)
        {
            var start = _position++;
            var quote = '\0';

            while (_position < input.Length)
            {
                var character = input[_position];

                if (quote != '\0')
                {
                    if (character == quote)
                        quote = '\0';
                }
                else if (character is '"' or '\'')
                {
                    quote = character;
                }
                else if (character == '>')
                {
                    break;
                }

                _position++;
            }

            if (_position == input.Length)
            {
                tokens.Add(new SubtitleBadToken(
                    input[start..],
                    "Expecting '>' but not found"));
                _position = input.Length;
                return;
            }

            _position++;
            var value = input[start.._position];
            tokens.Add(SubtitleTagParser.TryParse(value, out var tag)
                ? tag
                : new SubtitleToken(SubtitleTokenType.Tag, value));
        }

        private void ReadNewLine(List<SubtitleToken> tokens)
        {
            if (input[_position] == '\r' &&
                _position + 1 < input.Length &&
                input[_position + 1] == '\n')
            {
                _position += 2;
            }
            else
            {
                _position++;
            }

            tokens.Add(new SubtitleToken(SubtitleTokenType.NewLine, "\n"));
        }

        private void ReadInfo(List<SubtitleToken> tokens,char ending=']')
        {
            var start = _position++;

            while (_position < input.Length && input[_position] != ending)
                _position++;

            if (_position == input.Length)
            {
                tokens.Add(new SubtitleBadToken(
                    input[start..],
                    $"Expecting '{ending}' but not found"));
                return;
            }

            _position++;
            tokens.Add(new SubtitleToken(
                SubtitleTokenType.Info,
                input[start.._position]));
        }

        private void ReadText(List<SubtitleToken> tokens)
        {
            var start = _position;

            while (_position < input.Length &&
                   input[_position] is not '<' and not '\r' and not '\n' and not '[')
            {
                _position++;
            }

            tokens.Add(new SubtitleToken(
                SubtitleTokenType.Text,
                input[start.._position]));
        }
    }
}
