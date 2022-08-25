using System;
using System.Diagnostics;
using MhStream.Abstract;
using MhStream.Data;
using MhStream.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Mvc.EnableRangeProcessing", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddResponseCompression();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMetadataParser<YtMetadata>, YtMedataParser>();
builder.Services.AddSingleton<IResourceProvider<ProcessStartInfo>, ProcessResourceProvider>();
builder.Services.AddSingleton<IAudioProvider<string>,YtAudioFileProvider>();
builder.Services.AddSingleton<IAudioConvert,FfmpegMp3Convert>();
builder.Services.AddSingleton<IPlaylistProvider, YtPlaylistProvider>();
builder.Services.AddSingleton<IDataUrlProvider, DataUrlProvider>();
builder.Services.AddSingleton<ConvertedAudioFileProvider<string>>();
var app = builder.Build();
app.UseResponseCompression();
app.MapControllers();
app.MapDefaultControllerRoute();
app.Run();
