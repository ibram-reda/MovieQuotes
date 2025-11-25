namespace MovieQuotes.UI.Features.Movies.WatchMovie;

using System;
using System.Collections.Generic;
using System.Linq;

internal record SubtitleEntry(int Id,TimeSpan StartTime, TimeSpan EndTime, string Text);

internal class SubtitleManager
{
    public Action<SubtitleEntry?>? OnSubtitleChanged;

    public SubtitleEntry? CurrentSubtitle { get; private set; }
    private readonly IOrderedEnumerable<SubtitleEntry> subtitles;

    public SubtitleManager(IEnumerable<SubtitleEntry> subs)  
    {
        subtitles = subs.OrderBy(a => a.StartTime);
    }

    /// <summary>
    /// Updates the current subtitle based on the specified playback time.
    /// </summary>
    /// <remarks>If the active subtitle changes as a result of this update, the subtitle change event is
    /// triggered. This method should be called whenever the playback time advances to ensure subtitles remain in sync
    /// with media playback.</remarks>
    /// <param name="currentTime">The current playback position used to determine which subtitle phrase should be active.</param>
    public void Update(TimeSpan currentTime)
    {
        var phrase = subtitles.FirstOrDefault(s => s.StartTime <= currentTime && currentTime <= s.EndTime   );

        if(phrase == CurrentSubtitle)
            return;

        CurrentSubtitle = phrase;
        OnSubtitleChanged?.Invoke(CurrentSubtitle);
    }

    /// <summary>
    /// Retrieves the most recent subtitle phrase that ends before the specified time.
    /// </summary>
    /// <remarks>If multiple subtitle phrases end before the specified time, the one with the latest end time
    /// is returned. This method is useful for determining the last displayed subtitle prior to a given playback
    /// position.</remarks>
    /// <param name="currentTime">The point in time used to search for the previous subtitle phrase. Represents the current playback position.</param>
    /// <returns>The previous <see cref="SubtitleEntry"/> that ends before <paramref name="currentTime"/>; or <see
    /// langword="null"/> if no such phrase exists.</returns>
    public SubtitleEntry? GetPreviousPhrase(TimeSpan currentTime) =>
        subtitles.LastOrDefault(s => s.EndTime < currentTime);

     
    /// <summary>
    /// Retrieves the next subtitle phrase that begins after the specified time.
    /// </summary>
    /// <param name="currentTime">The current playback position. The method returns the first subtitle entry whose start time is greater than this
    /// value.</param>
    /// <returns>A <see cref="SubtitleEntry"/> representing the next subtitle phrase, or <see langword="null"/> if there are no
    /// subsequent phrases.</returns>
    public SubtitleEntry? GetNextPhrase(TimeSpan currentTime) =>
        subtitles.FirstOrDefault(s => s.StartTime > currentTime);

}
