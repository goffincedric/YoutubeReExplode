using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Common;
using YoutubeReExplode.Exceptions;
using YoutubeReExplode.Videos.ClosedCaptions;
using YoutubeReExplode.Videos.Streams;

namespace YoutubeReExplode.Videos;

/// <summary>
/// Operations related to YouTube videos.
/// </summary>
public class VideoClient(HttpClient http)
{
    private readonly VideoController _controller = new(http);

    /// <summary>
    /// Operations related to media streams of YouTube videos.
    /// </summary>
    public StreamClient Streams { get; } = new(http);

    /// <summary>
    /// Operations related to closed captions of YouTube videos.
    /// </summary>
    public ClosedCaptionClient ClosedCaptions { get; } = new(http);

    /// <summary>
    /// Gets the metadata associated with the specified video.
    /// </summary>
    public async ValueTask<Video> GetAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default
    )
    {
        var watchPage = await _controller.GetVideoWatchPageAsync(videoId, cancellationToken);

        var playerResponse =
            watchPage.PlayerResponse
            ?? await _controller.GetPlayerResponseAsync(videoId, cancellationToken);

        var title =
            playerResponse.Title
            // Videos without title are legal
            // https://github.com/Tyrrrz/YoutubeExplode/issues/700
            ?? "";

        var channelTitle =
            playerResponse.Author
            ?? throw new YoutubeReExplodeException("Failed to extract the video author.");

        var channelId =
            playerResponse.ChannelId
            ?? throw new YoutubeReExplodeException("Failed to extract the video channel ID.");

        var isLive =
            playerResponse.IsLive
            ?? throw new YoutubeReExplodeException("Failed to extract the video live status.");

        var isLiveContent =
            playerResponse.IsLiveContent
            ?? throw new YoutubeReExplodeException(
                "Failed to extract the video livestream status."
            );

        var uploadDate =
            playerResponse.UploadDate
            ?? watchPage.UploadDate
            ?? throw new YoutubeReExplodeException("Failed to extract the video upload date.");

        var thumbnails = playerResponse
            .Thumbnails.Select(t =>
            {
                var thumbnailUrl =
                    t.Url
                    ?? throw new YoutubeReExplodeException("Failed to extract the thumbnail URL.");

                var thumbnailWidth =
                    t.Width
                    ?? throw new YoutubeReExplodeException(
                        "Failed to extract the thumbnail width."
                    );

                var thumbnailHeight =
                    t.Height
                    ?? throw new YoutubeReExplodeException(
                        "Failed to extract the thumbnail height."
                    );

                var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                return new Thumbnail(thumbnailUrl, thumbnailResolution);
            })
            .Concat(Thumbnail.GetDefaultSet(videoId))
            .ToArray();

        // ytInitialData
        string? channelName = null;
        if (watchPage.InitialData?.Contents is [_, { Runs.Count: >= 1 }, ..])
            channelName = watchPage.InitialData?.Contents[1].Runs[0].Text;

        Music? musicData = null;
        if (watchPage.InitialData?.MusicInfoRows.Length > 0)
        {
            var musicInfoRows = watchPage.InitialData.MusicInfoRows;
            musicData = new Music(
                musicInfoRows
                    .FirstOrDefault(row =>
                        string.Equals(row.Title, "song", StringComparison.OrdinalIgnoreCase)
                    )
                    ?.Values?.FirstOrDefault(),
                musicInfoRows
                    .FirstOrDefault(row =>
                        string.Equals(row.Title, "artist", StringComparison.OrdinalIgnoreCase)
                    )
                    ?.Values,
                musicInfoRows
                    .FirstOrDefault(row =>
                        string.Equals(row.Title, "album", StringComparison.OrdinalIgnoreCase)
                    )
                    ?.Values?.FirstOrDefault()
            );
        }

        return new Video(
            videoId,
            title,
            new Author(channelId, channelTitle, channelName),
            uploadDate,
            playerResponse.Description ?? "",
            isLive,
            isLiveContent,
            playerResponse.Duration,
            thumbnails,
            playerResponse.Keywords,
            // Engagement statistics may be hidden
            new Engagement(
                playerResponse.ViewCount ?? 0,
                watchPage.LikeCount ?? 0,
                watchPage.DislikeCount ?? 0
            ),
            musicData
        );
    }
}
