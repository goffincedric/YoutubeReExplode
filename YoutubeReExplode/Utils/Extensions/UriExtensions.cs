using System;

namespace YoutubeReExplode.Utils.Extensions;

internal static class UriExtensions
{
    public static string GetDomain(this Uri uri) => uri.Scheme + Uri.SchemeDelimiter + uri.Host;
}
