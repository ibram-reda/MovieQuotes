namespace MovieQuotes.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class VideoController : BaseController
{ 

    [HttpGet("{MovieName}/{sequence:int}")]
    public async Task<IActionResult> Get(string MovieName,int sequence)
    {
        var query = new VideoClipQuery(MovieName, sequence);
        var result = await this.mediator.Send(query);
        if (result.IsError)
            return this.HandelErrors(result.Errors);

        var memory = new MemoryStream();

        using var videoClip = new FileStream(result.Payload ?? "", FileMode.Open, FileAccess.Read, FileShare.Read);

        await videoClip.CopyToAsync(memory);

        memory.Position = 0;

        return File(memory, "video/mp4", $"{MovieName}-{sequence}");
         
    }

    [HttpGet("{PhraseId:int}")]
    public async Task<IActionResult> GetById(int PhraseId)
    {
        var query = new VideoClipQuery(PhraseId);
        var result = await this.mediator.Send(query);
        if(result.IsError) 
            return this.HandelErrors(result.Errors);

        var memory = new MemoryStream();

        using var videoClip = new FileStream(result.Payload ?? "", FileMode.Open, FileAccess.Read, FileShare.Read);
        
        await videoClip.CopyToAsync(memory);

        memory.Position = 0; 

        return File(memory, "video/mp4", $"{PhraseId}");
         
    }


   
}
