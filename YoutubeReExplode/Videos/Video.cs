using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Common;

namespace YoutubeReExplode.Videos;

/// <summary>
/// Metadata associated with a YouTube video.
/// </summary>
public class Video : IMusicVideo
{
    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/watch?v={Id}";

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public Author Author { get; }

    /// <summary>
    /// Video upload date.
    /// </summary>
    public DateTimeOffset UploadDate { get; }

    /// <summary>
    /// Video description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool IsLive { get; }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool? IsLiveContent { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <summary>
    /// Available search keywords for the video.
    /// </summary>
    public IReadOnlyList<string> Keywords { get; }

    /// <summary>
    /// Engagement statistics for the video.
    /// </summary>
    public Engagement Engagement { get; }

    /// <inheritdoc />
    public Music? Music { get; }

    /// <summary>
    /// Initializes an instance of <see cref="Video" />.
    /// </summary>
    public Video(
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
        Music? music)
    {
        Id = id;
        Title = title;
        Author = author;
        UploadDate = uploadDate;
        Description = description;
        IsLive = isLive;
        IsLiveContent = isLiveContent;
        Duration = duration;
        Thumbnails = thumbnails;
        Keywords = keywords;
        Engagement = engagement;
        Music = music;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({Title})";
}