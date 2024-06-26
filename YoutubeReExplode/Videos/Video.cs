using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Common;

namespace YoutubeReExplode.Videos;

/// <summary>
/// Metadata associated with a YouTube video.
/// </summary>
public class Video(
    VideoId id,
    string title,
    Author author,
    DateTimeOffset uploadDate,
    string description,
    bool isLive,
    bool? isLiveContent,
    TimeSpan? duration,
    IReadOnlyList<Thumbnail> thumbnails,
    IReadOnlyList<string> keywords,
    Engagement engagement,
    Music? music
) : IMusicVideo
{
    /// <inheritdoc />
    public VideoId Id { get; } = id;

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/watch?v={Id}";

    /// <inheritdoc />
    public string Title { get; } = title;

    /// <inheritdoc />
    public Author Author { get; } = author;

    /// <summary>
    /// Video upload date.
    /// </summary>
    public DateTimeOffset UploadDate { get; } = uploadDate;

    /// <summary>
    /// Video description.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool IsLive { get; } = isLive;

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool? IsLiveContent { get; } = isLiveContent;

    /// <inheritdoc />
    public TimeSpan? Duration { get; } = duration;

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; } = thumbnails;

    /// <summary>
    /// Available search keywords for the video.
    /// </summary>
    public IReadOnlyList<string> Keywords { get; } = keywords;

    /// <summary>
    /// Engagement statistics for the video.
    /// </summary>
    public Engagement Engagement { get; } = engagement;

    /// <inheritdoc />
    public Music? Music { get; } = music;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({Title})";
}
