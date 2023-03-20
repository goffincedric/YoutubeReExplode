﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Bridge;
using YoutubeReExplode.Exceptions;
using YoutubeReExplode.Utils;
using YoutubeReExplode.Videos;

namespace YoutubeReExplode.Playlists;

internal class PlaylistController
{
    private readonly HttpClient _http;

    public PlaylistController(HttpClient http) => _http = http;

    // Works only with user-made playlists
    public async ValueTask<PlaylistBrowseResponse> GetPlaylistBrowseResponseAsync(
        PlaylistId playlistId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://www.youtube.com/youtubei/v1/browse")
        {
            // ReSharper disable VariableHidesOuterVariable
            Content = new StringContent(Json.Create(x => x.Object(x => x
                .Property("browseId", x => x.String("VL" + playlistId))
                .Property("context", x => x.Object(x => x
                    .Property("client", x => x.Object(x => x
                        .Property("clientName", x => x.String("WEB"))
                        .Property("clientVersion", x => x.String("2.20210408.08.00"))
                        .Property("hl", x => x.String("en"))
                        .Property("gl", x => x.String("US"))
                        .Property("utcOffsetMinutes", x => x.Number(0))
                    ))
                ))
            )))
            // ReSharper restore VariableHidesOuterVariable
        };

        using var response = await _http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var playlistResponse = PlaylistBrowseResponse.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken)
        );

        if (!playlistResponse.IsAvailable)
            throw new PlaylistUnavailableException($"Playlist '{playlistId}' is not available.");

        return playlistResponse;
    }

    // Works on all playlists, but contains limited metadata
    public async ValueTask<PlaylistNextResponse> GetPlaylistNextResponseAsync(
        PlaylistId playlistId,
        VideoId? videoId = null,
        int index = 0,
        string? visitorData = null,
        CancellationToken cancellationToken = default)
    {
        for (var retriesRemaining = 5;; retriesRemaining--)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://www.youtube.com/youtubei/v1/next")
            {
                // ReSharper disable VariableHidesOuterVariable
                Content = new StringContent(Json.Create(x => x.Object(x => x
                    .Property("playlistId", x => x.String(playlistId))
                    .Property("videoId", x => x.String(videoId?.Value))
                    .Property("playlistIndex", x => x.Number(index))
                    .Property("context", x => x.Object(x => x
                        .Property("client", x => x.Object(x => x
                            .Property("clientName", x => x.String("WEB"))
                            .Property("clientVersion", x => x.String("2.20210408.08.00"))
                            .Property("hl", x => x.String("en"))
                            .Property("gl", x => x.String("US"))
                            .Property("utcOffsetMinutes", x => x.Number(0))
                            .Property("visitorData", x => x.String(visitorData))
                        ))
                    ))
                )))
                // ReSharper restore VariableHidesOuterVariable
            };

            using var response = await _http.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var playlistResponse = PlaylistNextResponse.Parse(
                await response.Content.ReadAsStringAsync(cancellationToken)
            );

            if (!playlistResponse.IsAvailable)
            {
                // Retry if this is not the first request, meaning that the previous requests were successful,
                // and that the playlist is probably not actually unavailable.
                if (index > 0 && !string.IsNullOrWhiteSpace(visitorData) && retriesRemaining > 0)
                    continue;

                throw new PlaylistUnavailableException($"Playlist '{playlistId}' is not available.");
            }

            return playlistResponse;
        }
    }

    public async ValueTask<IPlaylistData> GetPlaylistResponseAsync(
        PlaylistId playlistId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetPlaylistBrowseResponseAsync(playlistId, cancellationToken);
        }
        catch (PlaylistUnavailableException)
        {
            return await GetPlaylistNextResponseAsync(playlistId, null, 0, null, cancellationToken);
        }
    }
}