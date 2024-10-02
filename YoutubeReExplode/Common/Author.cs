using System;
using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Channels;

namespace YoutubeReExplode.Common;

/// <summary>
/// Reference to a channel that owns a specific YouTube video or playlist.
/// </summary>
public class Author(ChannelId channelId, string channelTitle, string? channelName = null)
{
    /// <summary>
    /// Channel ID.
    /// </summary>
    public ChannelId ChannelId { get; } = channelId;

    /// <summary>
    /// Channel URL.
    /// </summary>
    public string ChannelUrl => $"https://www.youtube.com/channel/{ChannelId}";

    /// <summary>
    /// Channel title.
    /// </summary>
    public string ChannelTitle { get; } = channelTitle;

    /// <summary>
    /// Channel name.
    /// </summary>
    public string? ChannelName { get; } = channelName;

    /// <inheritdoc cref="ChannelTitle" />
    [Obsolete("Use ChannelTitle instead."), ExcludeFromCodeCoverage]
    public string Title => ChannelTitle;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => ChannelTitle;
}
