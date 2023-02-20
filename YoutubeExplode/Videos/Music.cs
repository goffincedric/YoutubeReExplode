using System.Diagnostics.CodeAnalysis;

namespace YoutubeExplode.Videos;

/// <summary>
/// Music details of video.
/// </summary>
public class Music
{
    /// <summary>
    /// Song name.
    /// </summary>
    public string? Song { get; }

    /// <summary>
    /// Name of artists.
    /// </summary>
    public string[]? Artists { get; }

    /// <summary>
    /// Album name.
    /// </summary>
    public string? Album { get; }

    /// <summary>
    /// Initializes an instance of <see cref="Music" />.
    /// </summary>
    public Music(string? song, string[]? artists, string? album)
    {
        Song = song;
        Artists = artists;
        Album = album;
    }
}