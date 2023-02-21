using System.Text.Json;
using YoutubeReExplode.Utils.Extensions;
using YoutubeReExplode.Utils;

namespace YoutubeReExplode.Bridge;

internal class ThumbnailData
{
    private readonly JsonElement _content;

    public ThumbnailData(JsonElement content) => _content = content;

    public string? Url => Memo.Cache(this, () =>
        _content.GetPropertyOrNull("url")?.GetStringOrNull()
    );

    public int? Width => Memo.Cache(this, () =>
        _content.GetPropertyOrNull("width")?.GetInt32OrNull()
    );

    public int? Height => Memo.Cache(this, () =>
        _content.GetPropertyOrNull("height")?.GetInt32OrNull()
    );
}