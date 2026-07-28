namespace MovieQuotes.Domain.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

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


    public static SubtitlePhrase CreateSubtitlePhrase(int sequence, TimeSpan startTime, TimeSpan endTime, string text)
    {
        var phrase = new SubtitlePhrase
        {
            Sequence = sequence,
            StartTime = startTime,
            EndTime = endTime,
            Text = text
        };
        return phrase;
    }
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

    public async Task<bool> GenerateVideoClipAsync(string CashPath)
    {
        // add 200ms small time tolerance to the phrase
        var sTime = this.StartTime.Subtract(TimeSpan.FromMilliseconds(100));
        var dTime = this.Duration.Add(TimeSpan.FromMilliseconds(200)); ;

        if(this.Movie is null)
            throw new InvalidOperationException("Movie is not loaded for this phrase.");

        var outputLocation = Path.Combine(CashPath, this.Movie?.Title ?? "", $"{this.Sequence}.MP4");
        EnsureDirectoryExist(outputLocation);
        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-ss {sTime} -y -i \"{this.Movie?.GetVideoPath()}\" -t {dTime} \"{outputLocation}\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        // from documents  
        // To avoid deadlocks, always read the output stream first and then wait.  
        var error = await process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(60000)) // 1 min timeout
        {
            process.Kill();
            return false;
        }
        this.VideoClipPath = outputLocation.Replace(CashPath, Constants.CashTemplate);
        return process.ExitCode == 0;
    }

    private void EnsureDirectoryExist(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }


    public string GetTextWithoutMarkupAndDuplicateSpaces()
    {
        Regex MarkUpRegex = new Regex(@"<i>|</ i>|</i>|<b>|</b>|<u>|</u>|<font ([A-Za-z]+="".*?"")*>|</font>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex SpaceRegex = new Regex(@"[^\S\n]+", RegexOptions.Compiled);
        var withoutMarkUp = MarkUpRegex.Replace(Text, string.Empty);
        var withoutExtraSpaces = SpaceRegex.Replace(withoutMarkUp, " ").Trim();
        return withoutExtraSpaces;
    }

    public string[] GetWords()
    {
        // Split the text into words, removing markup and duplicate spaces
        // and normalizing each word.
        // keep link as one word
        return GetTextWithoutMarkupAndDuplicateSpaces()
             .Split(new[] { ' ', '\n', ',', '!', '?', '.', '"', '(', ')', '-', ':', ';' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(w => Normalize(w))
             .ToArray();


    }

    Regex NormalizedRgx = new Regex("^[^a-zA-Z0-9]+|[^a-zA-Z0-9]+$", RegexOptions.Compiled);
    private string Normalize(string word)
    {
        return NormalizedRgx.Replace(word, "");
    }
}
