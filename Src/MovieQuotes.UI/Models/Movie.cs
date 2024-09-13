namespace MovieQuotes.UI.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;


public class ProcessingMovie
{

    public string MovieName { get; } = string.Empty;
    public Uri? VideoUri { get; }
    public Uri? SubtitleUri { get; }
    public List<SubtitlePhrase> ProcessedPhrases { get; } = new();
    public Queue<SubtitlePhrase> LeftPhrases { get; } = new();

    public bool IsDone { get; }


    private async Task StartProcessAsync(CancellationToken token = default)
    {
        var mediaInfo = await FFmpeg.GetMediaInfo(MovieName, token);
        var videostram = mediaInfo.VideoStreams.First();
        var audiosream = mediaInfo.AudioStreams.First();

        List<Task> tasks = new();

        while (LeftPhrases?.TryPeek(out var phrase) ?? false)
        {
            tasks.Add(ProcessPhraseFromVideo(videostram, audiosream, phrase));
            LeftPhrases?.Dequeue();

            if (token.IsCancellationRequested)
            {
                break;
            }
        }
        Console.WriteLine("Waiting for finsh");
        await Task.WhenAll(tasks);

    }

    private async Task ProcessPhraseFromVideo(IVideoStream videostram, IAudioStream audiosream, SubtitlePhrase phrase)
    {
        string outputFileName = $"{MovieName}/{phrase.Sequence}.MP4";

        await FFmpeg.Conversions.New()
            .AddStream(videostram.Split(phrase.StartTime, phrase.Duration))
            .AddStream(audiosream.Split(phrase.StartTime, phrase.Duration))
            .SetOutput(outputFileName)
            .Start();

        ProcessedPhrases.Add(phrase);
    }
}
