using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Lazy;
using YoutubeReExplode.Utils;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Bridge;

internal partial class InitialData
{
    private readonly JsonElement _content;

    [Lazy]
    public IReadOnlyList<Content> Contents =>
        _content
            .GetPropertyOrNull("contents")
            ?.GetPropertyOrNull("twoColumnWatchNextResults")
            ?.GetPropertyOrNull("results")
            ?.GetPropertyOrNull("results")
            ?.GetPropertyOrNull("contents")
            ?.EnumerateArrayOrNull()
            ?.Select(j => new Content(j))
            .ToArray() ?? Array.Empty<Content>();

    [Lazy]
    public InfoRow[] MusicInfoRows =>
        EngagementPanels
            .FirstOrDefault(
                panel =>
                    string.Equals(panel.PanelIdentifier, "engagement-panel-structured-description")
            )
            ?.Items.FirstOrDefault(
                item => string.Equals(item.Title, "music", StringComparison.OrdinalIgnoreCase)
            )
            ?.CarouselLockups.ElementAtOrDefault(0)
            ?.InfoRows ?? Array.Empty<InfoRow>();

    [Lazy]
    private IReadOnlyList<EngagementPanel> EngagementPanels =>
        _content
            .GetPropertyOrNull("engagementPanels")
            ?.EnumerateArrayOrNull()
            ?.Select(j => new EngagementPanel(j))
            .ToArray() ?? Array.Empty<EngagementPanel>();

    public InitialData(JsonElement content) => _content = content;
}

internal partial class InitialData
{
    public class InfoRow
    {
        private readonly JsonElement _content;

        public InfoRow(JsonElement content) => _content = content;

        public string? Title =>
            _content
                .GetPropertyOrNull("infoRowRenderer")
                ?.GetPropertyOrNull("title")
                ?.GetPropertyOrNull("simpleText")
                ?.GetStringOrNull();

        public string[]? Values => GetValues();

        private string[]? GetValues()
        {
            var value = _content
                .GetPropertyOrNull("infoRowRenderer")
                ?.GetPropertyOrNull("defaultMetadata")
                ?.GetPropertyOrNull("simpleText")
                ?.GetStringOrNull();
            if (value != null)
                return new[] { value };
            return _content
                .GetPropertyOrNull("infoRowRenderer")
                ?.GetPropertyOrNull("defaultMetadata")
                ?.GetPropertyOrNull("runs")
                ?.EnumerateArrayOrNull()
                ?.Select(j => new Run(j).Text)
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

        public string? Text => _content.GetPropertyOrNull("text")?.GetStringOrNull();
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
                .GetPropertyOrNull("carouselLockupRenderer")
                ?.GetPropertyOrNull("infoRows")
                ?.EnumerateArrayOrNull()
                ?.Select(infoRow => new InfoRow(infoRow))
                .ToArray() ?? Array.Empty<InfoRow>();
    }
}

internal partial class InitialData
{
    public class Item
    {
        private readonly JsonElement _content;

        public Item(JsonElement content) => _content = content;

        [Lazy]
        public string? Title =>
            _content
                .GetPropertyOrNull("videoDescriptionMusicSectionRenderer")
                ?.GetPropertyOrNull("sectionTitle")
                ?.GetPropertyOrNull("simpleText")
                ?.GetStringOrNull();

        [Lazy]
        public IReadOnlyList<CarouselLockup> CarouselLockups =>
            _content
                .GetPropertyOrNull("videoDescriptionMusicSectionRenderer")
                ?.GetPropertyOrNull("carouselLockups")
                ?.EnumerateArrayOrNull()
                ?.Select(infoRow => new CarouselLockup(infoRow))
                .ToArray() ?? Array.Empty<CarouselLockup>();
    }
}

internal partial class InitialData
{
    public class EngagementPanel
    {
        private readonly JsonElement _content;

        public EngagementPanel(JsonElement content) => _content = content;

        [Lazy]
        public string? PanelIdentifier =>
            _content
                .GetPropertyOrNull("engagementPanelSectionListRenderer")
                ?.GetPropertyOrNull("panelIdentifier")
                ?.GetStringOrNull();

        [Lazy]
        public IReadOnlyList<Item> Items =>
            _content
                .GetPropertyOrNull("engagementPanelSectionListRenderer")
                ?.GetPropertyOrNull("content")
                ?.GetPropertyOrNull("structuredDescriptionContentRenderer")
                ?.GetPropertyOrNull("items")
                ?.EnumerateArrayOrNull()
                ?.Select(j => new Item(j))
                .ToArray() ?? Array.Empty<Item>();
    }
}

internal partial class InitialData
{
    public class Content
    {
        private readonly JsonElement _content;

        public Content(JsonElement content) => _content = content;

        [Lazy]
        public IReadOnlyList<Run> Runs =>
            _content
                .GetPropertyOrNull("videoSecondaryInfoRenderer")
                ?.GetPropertyOrNull("owner")
                ?.GetPropertyOrNull("videoOwnerRenderer")
                ?.GetPropertyOrNull("title")
                ?.GetPropertyOrNull("runs")
                ?.EnumerateArrayOrNull()
                ?.Select(j => new Run(j))
                .ToArray() ?? Array.Empty<Run>();
    }
}
