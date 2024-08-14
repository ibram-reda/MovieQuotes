﻿namespace MovieQuotes.UI.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class SubtitlePhrase : IParsable<SubtitlePhrase>
{
    public int Sequence { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public TimeSpan Duration => EndTime - StartTime;

    private static string RegexText => @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>(.|[\r\n])+?(?=\r\n\r\n|$))";
    private static Regex SubtitleBlockRegex { get; } = new(RegexText);

    public static async Task<List<SubtitlePhrase>> GetPhrasesFromStreamAsync(StreamReader reader, CancellationToken token = default)
    {
        var FileTextContent = await reader.ReadToEndAsync(token);
        var matches = SubtitleBlockRegex.Matches(FileTextContent);
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

    public static SubtitlePhrase Parse(string s, IFormatProvider? provider = null)
    {
        if (!SubtitleBlockRegex.IsMatch(s))
            throw new ArgumentNullException(nameof(s), "Can't Pars the Strig!!");

        var m = SubtitleBlockRegex.Match(s);
        return Parse(m);
    }

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
