using System;
using System.Collections.Generic;
using YoutubeReExplode.Common;

namespace YoutubeReExplode.Videos;

/// <summary>
/// Properties shared by video metadata resolved from different sources.
/// </summary>
public interface IVideo
{
    /// <summary>
    /// Video ID.
    /// </summary>
    VideoId Id { get; }

    /// <summary>
    /// Video URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Video title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Video author.
    /// </summary>
    Author Author { get; }

    /// <summary>
    /// Video live status.
    /// </summary>
    bool IsLive { get; }

    /// <summary>
    /// Checks if video was/is a livestream. (Not available for search result and playlist videos)
    /// </summary>
    bool? IsLiveContent { get; }

    /// <summary>
    /// Video duration.
    /// </summary>
    /// <remarks>
    /// May be null if the video is a currently ongoing live stream.
    /// </remarks>
    TimeSpan? Duration { get; }

    /// <summary>
    /// Video thumbnails.
    /// </summary>
    IReadOnlyList<Thumbnail> Thumbnails { get; }
}

/// <summary>
/// Properties shared by video metadata (including music metadata) resolved from different sources.
/// </summary>
public interface IMusicVideo : IVideo
{
    /// <summary>
    /// Music details of video (if applicable).
    /// </summary>
    public Music? Music { get; }
}
