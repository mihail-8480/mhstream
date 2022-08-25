using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SportDetect.Controllers;

public class EventStreamController : Controller
{
    [NonAction]
    protected async Task SendData(string data)
    {
        foreach (var line in data.Split('\n'))
            await HttpContext.Response.WriteAsync("data: " + line + "\n");

        await HttpContext.Response.WriteAsync("\n");
        await HttpContext.Response.Body.FlushAsync();
    }


    [NonAction]
    protected async Task SendComment(string comment)
    {
        foreach (var line in comment.Split('\n'))
            await HttpContext.Response.WriteAsync(": " + line + "\n");

        await HttpContext.Response.WriteAsync("\n");
        await HttpContext.Response.Body.FlushAsync();
    }

    [NonAction]
    protected async Task StartEventStream()
    {
        HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");
        await HttpContext.Response.Body.FlushAsync();
    }
}