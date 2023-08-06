using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace YoutubeReExplode.Tests.Utils;

public class YoutubeClientFactory : YoutubeClient
{
    private IReadOnlyList<Cookie> InitialCookies { get; }

    public YoutubeClientFactory()
    {
        InitialCookies = (Environment.GetEnvironmentVariable("COOKIES") ?? string.Empty)
            .Split(',')
            .Select(cookie =>
            {
                var splitCookie = cookie.Split(';');
                return new Cookie(splitCookie[0], splitCookie[1], splitCookie[2], splitCookie[3]);
            })
            .ToList();
    }

    public YoutubeClient Create() => new(InitialCookies);
}
