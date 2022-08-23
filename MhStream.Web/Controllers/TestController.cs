using System.Threading.Tasks;
using MhStream.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MhStream.Web.Controllers;

[Route("")]
public class TestController : Controller
{
    private readonly IAudioProvider<string> _audioProvider;
    public TestController(IAudioProvider<string> audioProvider)
    {
        _audioProvider = audioProvider;

    }
    
    
    [Route("thumb/{id}")]
    public async Task<IActionResult> Thumb(string id)
    {
        var audioFile = await _audioProvider.GetAudioFile($"https://www.youtube.com/watch?v={id}",
            Request.HttpContext.RequestAborted);
        var resource = await audioFile.GetThumbnail(Request.HttpContext.RequestAborted);
        var stream = await resource.GetStream(Request.HttpContext.RequestAborted);
        var res = File(stream, resource.GetContentType());
        res.EnableRangeProcessing = true;
        return res;
    }
    
    [Route("play/{id}"), HttpGet]
    public async Task<IActionResult> Play(string id)
    {
        var audioFile = await _audioProvider.GetAudioFile($"https://www.youtube.com/watch?v={id}", Request.HttpContext.RequestAborted);
        using var resource = await audioFile.GetResource(Request.HttpContext.RequestAborted);
        var stream = await resource.GetStream(Request.HttpContext.RequestAborted);

        var res = File(stream, resource.GetContentType());
        res.EnableRangeProcessing = true;
        return res;
    }
}