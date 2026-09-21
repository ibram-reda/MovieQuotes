namespace MovieQuotes.Application.Common.Services;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;
using MovieQuotes.Subtitles.Abstractions;
using MovieQuotes.Subtitles.Srt;
using System.Diagnostics;
using System.Linq;
using System.Text;

internal static class SubtitleManager
{

    public static async Task<OperationResult<IQueryable<SubtitlePhrase>>> LoadAsync(string filePath, Encoding? encoding = null)
    {
        var result = new OperationResult<IQueryable<SubtitlePhrase>>();

        if (!File.Exists(filePath))
        {
            result.AddError(Enums.ErrorCode.NotFound, "File is not found");
            return result;
        }

        using var stream = File.OpenRead(filePath);

        ISubtitleParser parser = new SrtSubtitleParser();

        var subtitles = await parser.Parse(stream);

        // check for doplicate index
        var douplication = subtitles.GroupBy(m => m.Index).Where(group => group.Count() > 1);
        if (douplication.Any())
        {
            result.AddError(Enums.ErrorCode.ValidationError,"UnValide Subtitles File {0}",filePath);
            foreach(var index in douplication.Select(a=>a.Key))
                result.AddError(Enums.ErrorCode.ValidationError, "Douplicate Sequance {0}", index);
        }


        result.Payload = subtitles.Select(s =>
        SubtitlePhrase.CreateSubtitlePhrase(s.Index, s.Start, s.End, s.Text))
        .AsQueryable();

        return result;
    }

    public static async Task<OperationResult<Unit>> WriteToDeskAsync(this IEnumerable<SubtitlePhrase> subtitles, string filePath, Encoding? encoding = null)
    {
        var result = new OperationResult<Unit>();

        if (File.Exists(filePath))
        {
            result.AddError(Enums.ErrorCode.ValidationError, "File is already found");
            return result;
        }

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            result.AddError(Enums.ErrorCode.ValidationError, "Directory is not exist");
            return result;
        }

        using var stream = new StreamWriter(filePath, true, encoding ?? Encoding.UTF8);

        foreach (var s in subtitles)
            await stream.WriteAsync(s.ToString() + Environment.NewLine);

        return result;
    }

    public static void Shift(this IEnumerable<SubtitlePhrase> subtitles, int timeShiftInMs)
    {
        var shifting = TimeSpan.FromMilliseconds(timeShiftInMs);

        foreach (var s in subtitles)
            s.AddTimeShift(shifting);
    }


    public static void RemoveMarkupAndDuplicateSpaces(this IEnumerable<SubtitlePhrase> subtitles)
    {
        foreach (var s in subtitles)
            s.EditText(s.GetTextWithoutMarkupAndDuplicateSpaces());
    }



}
