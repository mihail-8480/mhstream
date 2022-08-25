# MhStream
A YouTube mp3 streaming proxy built using ASP.NET Core, youtube-dl and ffmpeg.

## API Reference

### Connection Check
```ts
GET /connection-chek
Content-Type: application/json
{
    "app": "mhstream"
}
```

Gives you the ability to check if the server is a MhStream server.

### Metadata
```ts
GET /metadata/{id:string}
Content-Type: application/json
{
  "name": string,
  "artist": string,
  "image": string
}
```

Get some processed metadata about a YouTube video.

The `name` and the `artist` are what `IMetadataParser` attempted to guess.

The `image` is a base64 encoded data URL of a cropped (to a square, centered) version of the thumbnail.

### Playlist
```ts
GET /playlist/{playlistId:string}
Content-Type: text/event-stream
data:
{
  "id: string,
  "name: string,
  "artist": string,
  "image": string
}
```

Get the processed metadata for every video in a YouTube playlist.

The response is the same as in the Metadata endpoint, just it has an extra property "id" which is the ID of the YouTube video.

A single event stream data event is sent per YouTube video.


### Stream
```ts
GET /stream/{id:string}
Content-Type: audio/mpeg
```

Get the audio of a YouTube video as an MP3 stream.

## Configuration & Dependencies
The only dependencies (other than ASP.NET Core and .NET 6) are `ffmpeg` and `youtube-dl`.

You need to specify the path to them in `appsettings.json` under the `Binaries` property:
```json
{
  "youtube-dl": "/usr/bin/youtube-dl",
  "ffmpeg": "/usr/bin/ffmpeg"
}
```

The default configuration (that tries to find them in `/usr/bin`) works for a normal ArchLinux install.


## Running
(this example is for ArchLinux)

### Installing the dependencies
```sh
sudo pacman -S dotnet-sdk aspnet-runtime ffmpeg youtube-dl git
```

### Cloning the repository
```sh
git clone https://github.com/mihail-8480/mhstream
```

### Running
```sh
cd mhstream
dotnet run
```
