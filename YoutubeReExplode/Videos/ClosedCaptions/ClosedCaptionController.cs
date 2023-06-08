using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Bridge;
using YoutubeReExplode.Utils;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Videos.ClosedCaptions;

internal class ClosedCaptionController : VideoController
{
    public ClosedCaptionController(HttpClient http) : base(http)
    {
    }

    public async ValueTask<ClosedCaptionTrackResponse> GetClosedCaptionTrackResponseAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        // Enforce known format
        var urlWithFormat = url
            .Pipe(s => UrlEx.SetQueryParameter(s, "format", "3"))
            .Pipe(s => UrlEx.SetQueryParameter(s, "fmt", "3"));

        return ClosedCaptionTrackResponse.Parse(
            await Http.GetStringAsync(urlWithFormat, cancellationToken)
        );
    }
}