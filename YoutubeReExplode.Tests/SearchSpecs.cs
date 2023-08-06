using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using YoutubeReExplode.Common;
using YoutubeReExplode.Tests.Utils;

namespace YoutubeReExplode.Tests;

public class SearchSpecs
{
    [Fact]
    public async Task I_can_get_results_from_a_search_query()
    {
        // Arrange
        var youtube = new YoutubeClientFactory().Create();

        // Act
        var results = await youtube.Search.GetResultsAsync("undead corporation");

        // Assert
        results.Should().HaveCountGreaterOrEqualTo(50);
    }

    [Fact]
    public async Task I_can_get_results_from_a_search_query_that_contains_special_characters()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var results = await youtube.Search.GetResultsAsync("\"dune 2\" ending");

        // Assert
        results.Should().HaveCountGreaterOrEqualTo(50);
    }

    [Fact]
    public async Task I_can_get_video_results_from_a_search_query()
    {
        // Arrange
        var youtube = new YoutubeClientFactory().Create();

        // Act
        var videos = await youtube.Search.GetVideosAsync("undead corporation");

        // Assert
        videos.Should().HaveCountGreaterOrEqualTo(50);
    }

    [Fact]
    public async Task I_can_get_live_video_results_from_a_search_query()
    {
        // Arrange
        var youtube = new YoutubeClientFactory().Create();

        // Act
        var videos = await youtube.Search.GetVideosAsync("lofi");

        // Assert
        videos.Should().HaveCountGreaterOrEqualTo(1);
        videos.Should().Contain(searchResult => searchResult.IsLive);
        videos.Should().NotContain(searchResult => searchResult.IsLiveContent.HasValue);
    }

    [Fact]
    public async Task I_can_get_playlist_results_from_a_search_query()
    {
        // Arrange
        var youtube = new YoutubeClientFactory().Create();

        // Act
        var playlists = await youtube.Search.GetPlaylistsAsync("undead corporation");

        // Assert
        playlists.Should().NotBeEmpty();
    }

    [Fact]
    public async Task I_can_get_channel_results_from_a_search_query()
    {
        // Arrange
        var youtube = new YoutubeClientFactory().Create();

        // Act
        var channels = await youtube.Search.GetChannelsAsync("undead corporation");

        // Assert
        channels.Should().NotBeEmpty();
    }
}
