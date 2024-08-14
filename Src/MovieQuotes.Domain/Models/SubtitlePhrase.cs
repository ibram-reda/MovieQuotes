namespace MovieQuotes.Domain.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class SubtitlePhrase
{
    private SubtitlePhrase()
    {
    }
    public int MovieId { get; private set; }
    public int Sequence { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? VideoClipPath { get; private set; }  
    public TimeSpan Duration => EndTime - StartTime;

    public virtual Movie? Movie { get; private set; }

    private static string RegexText => @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>(.|[\r\n])+?(?=\r\n\r\n|$))";
    private static Regex SubtitleBlockRegex { get; } = new(RegexText);

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
        return matches.Select(m => Parse(m)).ToList();
    }

    private static SubtitlePhrase Parse(Match m)
    {
        var result = new SubtitlePhrase();
        result.Sequence = int.Parse(m.Groups["Order"].Value);
        result.StartTime = TimeSpan.Parse(m.Groups["StartTime"].Value.Replace(',', '.'));
        result.EndTime = TimeSpan.Parse(m.Groups["EndTime"].Value.Replace(',', '.'));
        result.Text = m.Groups["Sub"].Value;
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
