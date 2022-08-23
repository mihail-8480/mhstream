using System.Threading.Tasks;
using MhStream.Abstract;
using MhStream.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MhStream.Web.Controllers;

[Route("")]
public class TestController : Controller
{
    private readonly IAudioProvider<string> _audioProvider;
    private readonly ITagger _tagger;

    public TestController(ConvertedAudioFileProvider<string> audioProvider, ITagger tagger)
    {
        _audioProvider = audioProvider;
        _tagger = tagger;
    }
    

    [Route("download/{id}"), HttpGet]
    public async Task<IActionResult> Play(string id)
    {
        var audioFile = await _audioProvider.GetAudioFile($"https://www.youtube.com/watch?v={id}", Request.HttpContext.RequestAborted);
        using var resource = await audioFile.GetResource(Request.HttpContext.RequestAborted);
        var stream = await resource.GetStream(Request.HttpContext.RequestAborted);
        var taggedResource = await _tagger.Tag(audioFile, stream, Request.HttpContext.RequestAborted);
        
        var res = File(await taggedResource.GetStream(Request.HttpContext.RequestAborted), taggedResource.GetContentType());
        res.EnableRangeProcessing = true;
        res.FileDownloadName = audioFile.Title + ".mp3";
        return res;
    }
}