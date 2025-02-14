﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Utils;

// Like DelegatingHandler, but wraps an HttpClient instead of an HttpMessageHandler.
// Used to extend an externally provided HttpClient with additional behavior.
internal abstract class ClientDelegatingHandler(HttpClient http, bool disposeClient = false)
    : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // Clone the request to reset its completion status, which is required
        // in order to pass the request from one HttpClient to another.
        using var clonedRequest = request.Clone();

        return await http.SendAsync(
            clonedRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && disposeClient)
            http.Dispose();

        base.Dispose(disposing);
    }
}
