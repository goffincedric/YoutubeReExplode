using System;
using AngleSharp.Html.Dom;
using Lazy;
using YoutubeReExplode.Utils;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Bridge;

internal partial class ChannelPage
{
    private readonly IHtmlDocument _content;

    [Lazy]
    public string? Url =>
        _content.QuerySelector("meta[property=\"og:url\"]")?.GetAttribute("content");

    [Lazy]
    public string? Id => Url?.SubstringAfter("channel/", StringComparison.OrdinalIgnoreCase);

    [Lazy]
    public string? Title =>
        _content.QuerySelector("meta[property=\"og:title\"]")?.GetAttribute("content");

    [Lazy]
    public string? LogoUrl =>
        _content.QuerySelector("meta[property=\"og:image\"]")?.GetAttribute("content");

    public ChannelPage(IHtmlDocument content) => _content = content;
}

internal partial class ChannelPage
{
    public static ChannelPage? TryParse(string raw)
    {
        var content = Html.Parse(raw);

        if (content.QuerySelector("meta[property=\"og:url\"]") is null)
            return null;

        return new ChannelPage(content);
    }
}
