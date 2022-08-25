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
    private readonly IDataUrlProvider _dataUrlProvider;
    private readonly IPlaylistProvider _playlistProvider;

    public AudioController(ConvertedAudioFileProvider<string> provider, IDataUrlProvider dataUrlProvider, IPlaylistProvider playlistProvider)
    {
        _provider = provider;
        _dataUrlProvider = dataUrlProvider;
        _playlistProvider = playlistProvider;
    }

    [Route("playlist/{playlistId}"), HttpGet]
    public async Task PlaylistMetadata(string playlistId)
    {
        await StartEventStream();
        await foreach (var audioFile in _playlistProvider.GetFiles(playlistId, Request.HttpContext.RequestAborted))
        {
            try
            {
                using var thumbnail = await audioFile.GetThumbnail(Request.HttpContext.RequestAborted);
                var url = await _dataUrlProvider.GetDataUrl(thumbnail, Request.HttpContext.RequestAborted);

                await SendData(JsonSerializer.Serialize(new
                {
                    id = audioFile.Id,
                    title = audioFile.Title,
                    artist = audioFile.Artist,
                    image = url
                }));
            }
            catch(Exception e)
            {
                await SendComment(e.ToString());
                return;
            }

        }
        
    }
    
    [Route("metadata/{id}"), HttpGet]
    public async Task<IActionResult> Metadata(string id)
    {
        try
        {
            var audioFile = await _provider.GetAudioFile(id, Request.HttpContext.RequestAborted);
            using var thumbnail = await audioFile.GetThumbnail(Request.HttpContext.RequestAborted);
            var url = await _dataUrlProvider.GetDataUrl(thumbnail, Request.HttpContext.RequestAborted);
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
            var audioFile = await _provider.GetAudioFile(id, Request.HttpContext.RequestAborted);
            using var resource = await audioFile.GetResource(Request.HttpContext.RequestAborted);
            var stream = await resource.GetStream(Request.HttpContext.RequestAborted);
            return File(stream, resource.GetContentType());

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);

        }
    }
}