
namespace MovieQuotes.UI.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

public class MediaService 
{ 

    private LibVLC SoundLibVLC { get; } = new("--no-video", "--quiet");
    private LibVLC VideoLibVLC { get; } = new("--quiet");
    private MediaPlayer SoundPlayer { get; }
    public MediaPlayer VideoPlayer { get; }

    private Dictionary<string, Media> _loadedMedia = new();

    public MediaService()
    { 
        SoundPlayer = new(SoundLibVLC)
        {
            EnableHardwareDecoding = false,
            Volume = 100, 
        };
        VideoPlayer = new(VideoLibVLC)
        {
            EnableHardwareDecoding = false,
            Volume = 100,
        };
    }

    public void PlayVideo(string FilePath)
    {
        if (!File.Exists(FilePath))
            throw new FileNotFoundException($"Video file {FilePath} not found.");
        var media = new Media(VideoLibVLC, FilePath);
        VideoPlayer.Stop();
        VideoPlayer.Play(media);
    }

    public async Task PlayAudio(string FilePath)
    {
        if (!File.Exists(FilePath))
            throw new FileNotFoundException($"Sound file {FilePath} not found.");
        var media = new Media(SoundLibVLC, FilePath);
        SoundPlayer.Stop();
        await Task.Run(() => SoundPlayer.Play(media));
    }

    public async Task PlayCorrectAnswerSound()
    {
        await Task.Run(() => PlayTrack("correct.mp3"));
    }
    public async Task PlayWrongAnswerSound()
    {
        await Task.Run(() => PlayTrack("incorrect.mp3"));
    }

    private void PlayTrack(string trackName)
    {
        var media = GetMedia(trackName);
        SoundPlayer.Stop(); 
        SoundPlayer.Play(media);
    }

    private Media GetMedia(string trackName)
    {
        if (_loadedMedia.ContainsKey(trackName))
            return _loadedMedia[trackName];
        var filePath = GetFilePath(trackName);
        if (filePath == null)
            throw new FileNotFoundException($"Sound file {trackName} not found.");
        var media = new Media(SoundLibVLC, filePath);
        _loadedMedia.Add(trackName, media);
        return media;
    }

    private string? GetFilePath(string trackName)
    {
        //places to search for the sound
        // 1. {CurrentDirectory}/Assets/Sounds/
        // 2. {AppData}/MovieQuotes/Assets/Sounds/
        List<string> searchingDir = [
            Environment.CurrentDirectory,
                Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MovieQuotes")];
        foreach (var dir in searchingDir)
        {
            string endPath = "Assets/Sounds/";
            var filePath = Path.Combine(dir, endPath,trackName);
            if (File.Exists(filePath))
            {
                 return filePath;
            }
        }
        return null; 
    }
}