using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Bridge;
using YoutubeReExplode.Exceptions;
using YoutubeReExplode.Utils;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Videos;

internal class VideoController(HttpClient http)
{
    private string? _visitorData;

    protected HttpClient Http { get; } = http;

    private async ValueTask<string> ResolveVisitorDataAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(_visitorData))
            return _visitorData;

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.youtube.com/sw.js_data"
        );

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Headers.Add(
            "User-Agent",
            "com.google.ios.youtube/19.45.4 (iPhone16,2; U; CPU iOS 18_1_0 like Mac OS X; US)"
        );

        using var response = await Http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        // TODO: move this to a bridge wrapper
        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
        if (jsonString.StartsWith(")]}'"))
            jsonString = jsonString[4..];

        var json = Json.Parse(jsonString);

        // This is just an ordered (but unstructured) blob of data
        var value = json[0][2][0][0][13].GetStringOrNull();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new YoutubeReExplodeException("Failed to resolve visitor data.");
        }

        return _visitorData = value;
    }

    public async ValueTask<VideoWatchPage> GetVideoWatchPageAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default
    )
    {
        for (var retriesRemaining = 5; ; retriesRemaining--)
        {
            var watchPage = VideoWatchPage.TryParse(
                await Http.GetStringAsync(
                    $"https://www.youtube.com/watch?v={videoId}&bpctr=9999999999",
                    cancellationToken
                )
            );

            if (watchPage is null)
            {
                if (retriesRemaining > 0)
                    continue;

                throw new YoutubeReExplodeException(
                    "Video watch page is broken. Please try again in a few minutes."
                );
            }

            if (!watchPage.IsAvailable)
                throw new VideoUnavailableException($"Video '{videoId}' is not available.");

            return watchPage;
        }
    }

    public async ValueTask<PlayerResponse> GetPlayerResponseAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default
    )
    {
        var visitorData = await ResolveVisitorDataAsync(cancellationToken);

        // The most optimal client to impersonate is any mobile client, because they
        // don't require signature deciphering (for both normal and n-parameter signatures).
        // However, we can't use the ANDROID client because it has a limitation, preventing it
        // from downloading multiple streams from the same manifest (or the same stream multiple times).
        // https://github.com/Tyrrrz/YoutubeExplode/issues/705
        // Previously, we were using ANDROID_TESTSUITE as a workaround, which appeared to offer the same
        // functionality, but without the aforementioned limitation. However, YouTube discontinued this
        // client, so now we have to use IOS instead.
        // https://github.com/Tyrrrz/YoutubeExplode/issues/817
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.youtube.com/youtubei/v1/player"
        );

        request.Content = new StringContent(
            // lang=json
            $$"""
            {
              "videoId": {{Json.Serialize(videoId)}},
              "contentCheckOk": true,
              "context": {
                "client": {
                  "clientName": "IOS",
                  "clientVersion": "19.45.4",
                  "deviceMake": "Apple",
                  "deviceModel": "iPhone16,2",
                  "platform": "MOBILE",
                  "osName": "IOS",
                  "osVersion": "18.1.0.22B83",
                  "visitorData": {{Json.Serialize(visitorData)}},
                  "hl": "en",
                  "gl": "US",
                  "utcOffsetMinutes": 0
                }
              }
            }
            """
        );

        // User agent appears to be sometimes required when impersonating Android
        // https://github.com/iv-org/invidious/issues/3230#issuecomment-1226887639
        request.Headers.Add(
            "User-Agent",
            "com.google.ios.youtube/19.45.4 (iPhone16,2; U; CPU iOS 18_1_0 like Mac OS X; US)"
        );

        using var response = await Http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var playerResponse = PlayerResponse.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken)
        );

        if (!playerResponse.IsAvailable)
            throw new VideoUnavailableException($"Video '{videoId}' is not available.");

        return playerResponse;
    }

    public async ValueTask<PlayerResponse> GetPlayerResponseAsync(
        VideoId videoId,
        string? signatureTimestamp,
        CancellationToken cancellationToken = default
    )
    {
        var visitorData = await ResolveVisitorDataAsync(cancellationToken);

        // The only client that can handle age-restricted videos without authentication is the
        // TVHTML5_SIMPLY_EMBEDDED_PLAYER client.
        // This client does require signature deciphering, so we only use it as a fallback.
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.youtube.com/youtubei/v1/player"
        );

        request.Content = new StringContent(
            // lang=json
            $$"""
            {
              "videoId": {{Json.Serialize(videoId)}},
              "context": {
                "client": {
                  "clientName": "TVHTML5_SIMPLY_EMBEDDED_PLAYER",
                  "clientVersion": "2.0",
                  "visitorData": {{Json.Serialize(visitorData)}},
                  "hl": "en",
                  "gl": "US",
                  "utcOffsetMinutes": 0
                },
                "thirdParty": {
                  "embedUrl": "https://www.youtube.com"
                }
              },
              "playbackContext": {
                "contentPlaybackContext": {
                  "signatureTimestamp": {{Json.Serialize(signatureTimestamp)}}
                }
              }
            }
            """
        );

        using var response = await Http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var playerResponse = PlayerResponse.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken)
        );

        if (!playerResponse.IsAvailable)
            throw new VideoUnavailableException($"Video '{videoId}' is not available.");

        return playerResponse;
    }
}
