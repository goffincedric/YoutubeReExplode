using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using YoutubeReExplode.Utils.Extensions;
using YoutubeReExplode.Utils;

namespace YoutubeReExplode.Bridge;

internal partial class InitialData
{
    private readonly JsonElement _content;

    public IReadOnlyList<EngagementPanel> EngagementPanels => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("engagementPanels")?
            .EnumerateArrayOrNull()?
            .Select(j => new EngagementPanel(j))
            .ToArray() ??
        Array.Empty<EngagementPanel>()
    );

    public InitialData(JsonElement content) => _content = content;
}

internal partial class InitialData
{
    public class InfoRow
    {
        private readonly JsonElement _content;

        public InfoRow(JsonElement content) => _content = content;

        public string? Title => _content.GetPropertyOrNull("infoRowRenderer")?
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull();

        public string[]? Values => GetValues();

        private string[]? GetValues()
        {
            var value = _content.GetPropertyOrNull("infoRowRenderer")?
                .GetPropertyOrNull("defaultMetadata")?
                .GetPropertyOrNull("simpleText")?
                .GetStringOrNull();
            if (value != null) return new[] { value };
            return _content.GetPropertyOrNull("infoRowRenderer")?
                .GetPropertyOrNull("defaultMetadata")?
                .GetPropertyOrNull("runs")?
                .EnumerateArrayOrNull()?
                .Select(j => new Run(j).Text)
                .WhereNotNull()
                .ToArray();
        }
    }
}

internal partial class InitialData
{
    public class Run
    {
        private readonly JsonElement _content;

        public Run(JsonElement content) => _content = content;

        public string? Text =>
            _content
                .GetPropertyOrNull("text")?
                .GetStringOrNull();
    }
}

internal partial class InitialData
{
    public class CarouselLockup
    {
        private readonly JsonElement _content;

        public CarouselLockup(JsonElement content) => _content = content;

        public InfoRow[] InfoRows =>
            _content
                .GetPropertyOrNull("carouselLockupRenderer")?
                .GetPropertyOrNull("infoRows")?
                .EnumerateArrayOrNull()?
                .Select(infoRow => new InfoRow(infoRow))
                .ToArray() ??
            Array.Empty<InfoRow>();
    }
}

internal partial class InitialData
{
    public class Item
    {
        private readonly JsonElement _content;

        public Item(JsonElement content) => _content = content;

        public IReadOnlyList<CarouselLockup> CarouselLockups => Memo.Cache(this, () =>
            _content
                .GetPropertyOrNull("videoDescriptionMusicSectionRenderer")?
                .GetPropertyOrNull("carouselLockups")?
                .EnumerateArrayOrNull()?
                .Select(infoRow => new CarouselLockup(infoRow))
                .ToArray() ??
            Array.Empty<CarouselLockup>()
        );
    }
}

internal partial class InitialData
{
    public class EngagementPanel
    {
        private readonly JsonElement _content;

        public EngagementPanel(JsonElement content) => _content = content;

        public IReadOnlyList<Item> Items => Memo.Cache(this, () =>
            _content
                .GetPropertyOrNull("engagementPanelSectionListRenderer")?
                .GetPropertyOrNull("content")?
                .GetPropertyOrNull("structuredDescriptionContentRenderer")?
                .GetPropertyOrNull("items")?
                .EnumerateArrayOrNull()?
                .Select(j => new Item(j))
                .ToArray() ??
            Array.Empty<Item>()
        );
    }
}