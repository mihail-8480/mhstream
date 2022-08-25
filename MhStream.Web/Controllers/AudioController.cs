using System;
using System.Text.Json;
using System.Threading.Tasks;
using MhStream.Abstract;
using MhStream.Impl;
using Microsoft.AspNetCore.Mvc;
using SportDetect.Controllers;

namespace MhStream.Web.Controllers;

[Route("")]
public class AudioController : EventStreamController
{
    private readonly ConvertedAudioFileProvider<string> _provider;
    private readonly IAudioProvider<string> _normalProvider;
    private readonly IDataUrlProvider _dataUrlProvider;
    private readonly IPlaylistProvider _playlistProvider;

    public AudioController(ConvertedAudioFileProvider<string> provider, IDataUrlProvider dataUrlProvider,
        IPlaylistProvider playlistProvider, IAudioProvider<string> normalProvider)
    {
        _provider = provider;
        _dataUrlProvider = dataUrlProvider;
        _playlistProvider = playlistProvider;
        _normalProvider = normalProvider;
    }

    [Route("playlist/{playlistId}"), HttpGet]
    public async Task PlaylistMetadata(string playlistId)
    {
        await StartEventStream();
        await foreach (var (audioFile, disposables) in _playlistProvider.GetFiles(playlistId, Request.HttpContext.RequestAborted))
        {
            try
            {
                if (audioFile == null)
                {
                    continue;
                }
                
                var thumbnail = Consume(await audioFile.GetThumbnail(Request.HttpContext.RequestAborted));
                var url = Consume(await _dataUrlProvider.GetDataUrl(thumbnail, Request.HttpContext.RequestAborted));
                
                await SendData(JsonSerializer.Serialize(new
                {
                    id = audioFile.Id,
                    title = audioFile.Title,
                    artist = audioFile.Artist,
                    image = url
                }));
            }
            catch (Exception e)
            {
                await SendComment(e.ToString());
                return;
            }
            finally
            {
                foreach (var disposable in disposables)
                {
                    HttpContext.Response.RegisterForDispose(disposable);
                }
            }
        }
    }

    [Route("metadata/{id}"), HttpGet]
    public async Task<IActionResult> Metadata(string id)
    {
        try
        {
            var audioFile = Consume(await _normalProvider.GetAudioFile(id, Request.HttpContext.RequestAborted));
            await using var thumbnail = Consume(await audioFile.GetThumbnail(Request.HttpContext.RequestAborted));
            var url = Consume(await _dataUrlProvider.GetDataUrl(thumbnail, Request.HttpContext.RequestAborted));
            return Json(new
            {
                title = audioFile.Title,
                artist = audioFile.Artist,
                image = url
            });
        }
        catch (Exception e)
        {
            HttpContext.Response.StatusCode = 400;
            return Json(e.Message);
        }
    }


    [Route("stream/{id}"), HttpGet]
    public async Task<IActionResult> RequestStream(string id)
    {
        try
        {
            var audioFile = Consume(await _provider.GetAudioFile(id, Request.HttpContext.RequestAborted));
            var resource = Consume(await audioFile.GetResource(Request.HttpContext.RequestAborted));
            return File(resource, "audio/mpeg");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Route("connection-check"), HttpGet]
    public IActionResult Check()
    {
        return Json(new
        {
            app = "mhstream"
        });
    }
}