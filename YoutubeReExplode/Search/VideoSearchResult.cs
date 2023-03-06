using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Common;
using YoutubeReExplode.Videos;

namespace YoutubeReExplode.Search;

/// <summary>
/// Metadata associated with a YouTube video returned by a search query.
/// </summary>
public class VideoSearchResult : ISearchResult, IVideo
{
    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc cref="IVideo.Url" />
    public string Url => $"https://www.youtube.com/watch?v={Id}";

    /// <inheritdoc cref="IVideo.Title" />
    public string Title { get; }

    /// <inheritdoc />
    public Author Author { get; }

    /// <inheritdoc />
    public bool IsLive { get; }

    /// <inheritdoc />
    public bool? IsLiveContent { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <summary>
    /// Initializes an instance of <see cref="VideoSearchResult" />.
    /// </summary>
    public VideoSearchResult(
        VideoId id,
        string title,
        Author author,
        bool isLive,
        bool? isLiveContent,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        Id = id;
        Title = title;
        Author = author;
        Duration = duration;
        IsLive = isLive;
        IsLiveContent = isLiveContent;
        Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({Title})";
}