namespace MovieQuotes.UI.Models;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

public class SubtitlePhrase : IParsable<SubtitlePhrase>
{ 
    public int Sequence { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string Text { get; private set; } = string.Empty;

    public static string RegexText => @"(?<Order>\d+)\r\n(?<StartTime>(\d\d:){2}\d\d,\d{3}) --> (?<EndTime>(\d\d:){2}\d\d,\d{3})\r\n(?<Sub>(.|[\r\n])+?(?=\r\n\r\n|$))";
    public static Regex SubtitleBlockRegex { get; private set; } = new Regex(RegexText);
    
    public static SubtitlePhrase Parse(Match m)
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
        if (!CanParse(s))
            throw new ArgumentNullException(nameof(s),"Can't Pars the Strig!!");
        
        var m = SubtitleBlockRegex.Match(s);     
        return Parse(m);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SubtitlePhrase result)
    {
        if (CanParse(s ?? ""))
        {
            result = Parse(s ?? "");
            return true;
        }
        result = null;
        return false;      

    }

    public static bool CanParse(string TextBlock)
    { 
        return SubtitleBlockRegex.IsMatch(TextBlock);
    }
}
