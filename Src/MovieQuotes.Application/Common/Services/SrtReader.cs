namespace MovieQuotes.Application.Common.Services;

using MovieQuotes.Domain.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


internal class SrtReader
{
    private static string RegexText => @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>(.|[\r\n])+?(?=\r\n\r\n|$))";
    private static Regex SubtitleBlockRegex { get; } = new(RegexText);
    //private static readonly Regex SubtitleBlockRegex = new(
    //    @"(?<Order>\d+)\r?\n" +
    //    @"(?<StartTime>\d{2}:\d{2}:\d{2},\d{3})\s*-->\s*(?<EndTime>\d{2}:\d{2}:\d{2},\d{3})\r?\n" +
    //    @"(?<Sub>.*?)(?:\r?\n\r?\n|\r?\n$)",
    //    RegexOptions.Compiled | RegexOptions.Singleline);


    /// <summary>
    /// Extract Phrases from streamReader contains "srt" formatted content.
    /// </summary>
    /// <param name="reader">source of Subtitle.</param>
    /// <param name="token">Cancelation Token</param>
    /// <returns>Async Task that resolve for list of <see cref="SubtitlePhrase"/></returns>
    public static async Task<List<SubtitlePhrase>> GetPhrasesFromStreamAsync(StreamReader reader, CancellationToken token = default)
    {
        var FileTextContent = await reader.ReadToEndAsync(token);
        return GetPhrases(FileTextContent);
    }

    /// <summary>
    /// Extract Phrases from "srt" formatted content.
    /// </summary>
    /// <param name="content">subtitle.</param>
    /// <returns>list of <see cref="SubtitlePhrase"/>.</returns>
    public static List<SubtitlePhrase> GetPhrases(string content)
    {
        var matches = SubtitleBlockRegex.Matches(content);
        if (matches.Count == 0)
        {
            matches = SubtitleBlockRegex.Matches(content.Replace("\n", "\r\n"));
            if (matches.Count == 0)
                throw new Exception("can not read the subtitle content, because it's not in SRT standard formate");
        }
        var phrases = matches.Select(m => Parse(m)).ToList();

        if (phrases.GroupBy(m => m.Sequence).Where(group => group.Count() > 1).Any())
        {
            return ReSequence(phrases);
        }
        return phrases;
    }

    private static List<SubtitlePhrase> ReSequence(List<SubtitlePhrase> list)
    {
        return list.OrderBy(a => a.StartTime)
            .Select((a, newSequence) =>
                SubtitlePhrase.CreateSubtitlePhrase(
                newSequence + 1, // start from 1
                a.StartTime,
                a.EndTime,
                a.Text))
            .ToList();
    }
    private static SubtitlePhrase Parse(Match m)
    {
        var sequence = int.Parse(m.Groups["Order"].Value);
        var startTime = TimeSpan.Parse(m.Groups["StartTime"].Value.Replace(',', '.'));
        var endTime = TimeSpan.Parse(m.Groups["EndTime"].Value.Replace(',', '.'));
        var text = m.Groups["Sub"].Value;
        var result = SubtitlePhrase.CreateSubtitlePhrase(sequence, startTime, endTime, text);
        return result;
    }

    /// <summary>
    /// parse string to <see cref="SubtitlePhrase"/>.
    /// 
    /// string should be on form 
    /// <code> 
    /// [sequence Number]
    /// [hh:mm:ss,ms] --> [hh:mm:ss,ms]
    /// - [Text]
    /// </code>
    /// </summary>
    /// <param name="s">string</param>
    /// <param name="provider"></param>
    /// <returns>instance of <see cref="SubtitlePhrase"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SubtitlePhrase Parse(string s, IFormatProvider? provider = null)
    {
        if (!SubtitleBlockRegex.IsMatch(s))
            throw new ArgumentNullException(nameof(s), "Can't Pars the Strig!!");

        var m = SubtitleBlockRegex.Match(s);
        return Parse(m);
    }

    /// <summary>
    /// try parse string to <see cref="SubtitlePhrase"/>
    /// <br/><br/>
    /// see also <seealso cref="Parse(string, IFormatProvider?)"/>.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="provider"></param>
    /// <param name="result"></param>
    /// <returns>true if successful and false otherwise.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SubtitlePhrase result)
    {
        if (SubtitleBlockRegex.IsMatch(s ?? ""))
        {
            result = Parse(s ?? "");
            return true;
        }
        result = null;
        return false;
    }
}
