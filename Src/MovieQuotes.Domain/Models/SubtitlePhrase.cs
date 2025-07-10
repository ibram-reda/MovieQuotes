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
    public int Id { get; private set; }
    public int MovieId { get; private set; }
    public int Sequence { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? VideoClipPath { get; private set; }
    public TimeSpan Duration => EndTime - StartTime;

    public virtual Movie? Movie { get; private set; }

    public List<PhraseWords> PhraseWords { get; private set; } = [];

    private static string RegexText => @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>(.|[\r\n])+?(?=\r\n\r\n|$))";
    private static Regex SubtitleBlockRegex { get; } = new(RegexText);

    public override string ToString()
    {
        return $"""
            {Sequence}
            {StartTime.ToString(@"hh\:mm\:ss\,fff")} --> {EndTime.ToString(@"hh\:mm\:ss\,fff")}
            {Text}

            """;
    }
    /// <summary>
    /// Shift time with the same duration.
    /// </summary>
    /// <param name="time">shift time.</param>
    public void AddTimeShift(TimeSpan time)
    {
        this.StartTime += time;
        this.EndTime += time;
    }

    /// <summary>
    /// Edit the Text of the phrase.
    /// </summary>
    /// <param name="text">the new text.</param>
    /// <returns>true if text changed and false otherwise.</returns>
    public bool EditText(string text)
    {
        if (this.Text == text) return false;
        this.Text = text;
        return true;
    }

    /// <summary>
    /// Edit the start and end time of phrase.
    /// </summary>
    /// <param name="start">the new start time.</param>
    /// <param name="end">the new End time.</param>
    /// <returns>true if time changed or false otherwise.</returns>
    public bool EditDuration(TimeSpan start, TimeSpan end)
    {
        var Edited = false;
        if (this.StartTime != start)
        {
            this.StartTime = start;
            Edited = true;
        }

        if (this.EndTime != end)
        {
            this.EndTime = end;
            Edited = true;
        }
        return Edited;
    }

    /// <summary>
    /// Retrieves the actual path to the video clip associated with <see cref="SubtitlePhrase"/>.
    /// </summary>
    /// <remarks>This method replaces occurrences of predefined placeholders in the video clip path with their
    /// corresponding values. If the <c>VideoClipPath</c> property is null, empty, or contains only whitespace, the
    /// method returns <see langword="null"/>.</remarks>
    /// <returns>The video clip actual path, or <see langword="null"/> if the original path is empty
    /// or consists only of whitespace.</returns>
    public string? GetVideoClipPath()
    {
        if (string.IsNullOrWhiteSpace(this.VideoClipPath))
            return null;
        return this.VideoClipPath.Replace(Constants.CashTemplate, Constants.CashPath);
    }

    /// <summary>
    /// Deletes the video clip file associated with the current instance and reset the <see cref="VideoClipPath"/> to null.
    /// </summary>
    /// <remarks>This method removes the file located at the path specified by <see cref="VideoClipPath"/> and
    /// resets the property to <see langword="null"/>. If the file does not exist or <see cref="VideoClipPath"/> is null
    /// or whitespace, the method returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the video clip file was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool DeleteVideoClip()
    {
        if (string.IsNullOrWhiteSpace(this.VideoClipPath))
            return false;
        var actualPath = this.GetVideoClipPath();
        if (File.Exists(actualPath))
            File.Delete(actualPath);
        this.VideoClipPath = null; // reset the video clip path to force recreation
        return true;
    }

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
                throw new Exception("can not read the subtitle content");
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
            .Select((a, index) => new SubtitlePhrase
            {
                Sequence = index,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Text = a.Text,
            })
            .ToList();
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
