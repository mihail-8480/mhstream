using System.Threading.Tasks;
using MhStream.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MhStream.Web.Controllers;

[Route("play")]
public class TestController : Controller
{
    [Route("{id}"), HttpGet]
    public async Task<IActionResult> Play(string id)
    {
        // todo: use dependency injection & a hosted service
        var resourceProvider = new ProcessResourceProvider();
        var ytAudioFileProvider = new YtAudioFileProvider(resourceProvider);
        var mp3Convert = new FfmpegMp3Convert(resourceProvider);

        var converted = new ConvertedAudioFileProvider<string>(ytAudioFileProvider, mp3Convert);
        var audioFile = await converted.GetAudioFile($"https://www.youtube.com/watch?v={id}", Request.HttpContext.RequestAborted);
        using var resource = await audioFile.GetResource(Request.HttpContext.RequestAborted);
        var stream = await resource.GetStream(Request.HttpContext.RequestAborted);

        var res = File(stream, "audio/mp3");
        //res.EnableRangeProcessing = true;
        return res;
    }
}