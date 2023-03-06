using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using YoutubeExplode.Tests.TestData;
using YoutubeReExplode;
using YoutubeReExplode.Common;
using YoutubeReExplode.Exceptions;

namespace YoutubeExplode.Tests;

public class VideoSpecs
{
    private readonly ITestOutputHelper _testOutput;

    public VideoSpecs(ITestOutputHelper testOutput) =>
        _testOutput = testOutput;

    [Fact]
    public async Task I_can_get_metadata_of_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(VideoIds.Normal);

        // Assert
        video.Id.Value.Should().Be(VideoIds.Normal);
        video.Url.Should().NotBeNullOrWhiteSpace();
        video.Title.Should().Be("Ed Sheeran - Shivers [Official Video]");
        video.Author.ChannelId.Value.Should().Be("UC0C-w0YjGpqDXGB8IHb662A");
        video.Author.ChannelUrl.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelTitle.Should().Be("Ed Sheeran");
        video.Author.ChannelName.Should().Be("Ed Sheeran");
        video.UploadDate.Date.Should().Be(new DateTime(2021, 09, 09));
        video.Description.Should().Contain("The official video for Ed Sheeran - Shivers");
        video.Duration.Should().BeCloseTo(TimeSpan.FromSeconds(237), TimeSpan.FromSeconds(1));
        video.Thumbnails.Should().NotBeEmpty();
        video.Keywords.Should().BeEquivalentTo(
            "ed sheeran", "shivers", "equals", "shivers song", "ed sheeran shivers", "edsheeran",
            "ed sheeran new single", "acoustic", "live", "cover", "official video", "lyrics", "session",
            "ed sheeran 2021", "ed sheeran shivers official", "ed sheeran songs", "ed sheeran live",
            "ed sheeran lyrics", "ed sheeran new song", "ed sheeran 2021 song", "pop", "pop music", "ed sheran",
            "shivars", "band", "performance", "pub", "vocalists", "drums", "keyboard", "guitar", "live performance",
            "band performance", "shivers lyrics", "ed sheeran elton john"
        );
        video.Engagement.ViewCount.Should().BeGreaterOrEqualTo(263_000_000);
        video.Engagement.LikeCount.Should().BeGreaterOrEqualTo(2_200_000);
        video.Engagement.DislikeCount.Should().BeGreaterOrEqualTo(0);
        video.Engagement.AverageRating.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task I_cannot_get_metadata_of_a_private_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act & assert
        var ex = await Assert.ThrowsAsync<VideoUnavailableException>(async () =>
            await youtube.Videos.GetAsync(VideoIds.Private)
        );

        _testOutput.WriteLine(ex.Message);
    }

    [Fact]
    public async Task I_cannot_get_metadata_of_a_non_existing_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act & assert
        var ex = await Assert.ThrowsAsync<VideoUnavailableException>(async () =>
            await youtube.Videos.GetAsync(VideoIds.NonExisting)
        );

        _testOutput.WriteLine(ex.Message);
    }

    [Theory]
    [InlineData(VideoIds.Normal)]
    [InlineData(VideoIds.Unlisted)]
    [InlineData(VideoIds.EmbedRestrictedByAuthor)]
    [InlineData(VideoIds.EmbedRestrictedByYouTube)]
    [InlineData(VideoIds.AgeRestrictedViolent)]
    [InlineData(VideoIds.AgeRestrictedEmbedRestricted)]
    public async Task I_can_get_metadata_of_any_available_video(string videoId)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.Id.Value.Should().Be(videoId);
        video.Url.Should().NotBeNullOrWhiteSpace();
        video.Title.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelId.Value.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelUrl.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelTitle.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelName.Should().NotBeNullOrWhiteSpace();
        video.UploadDate.Date.Should().NotBe(default);
        video.Description.Should().NotBeNull();
        video.Duration.Should().NotBe(default);
        video.Thumbnails.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(VideoIds.ContainsSongMetadata)]
    [InlineData(VideoIds.ContainsLinkedSongMetadata)]
    public async Task I_can_get_song_metadata_of_supported_music_videos(string videoId)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.Music?.Song.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(VideoIds.ContainsArtistMetadata)]
    [InlineData(VideoIds.ContainsLinkedArtistMetadata)]
    public async Task I_can_get_artist_metadata_of_supported_music_videos(string videoId)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.Music?.Artists.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(VideoIds.ContainsAlbumMetadata)] // TODO: Find video with album link
    public async Task I_can_get_album_metadata_of_supported_music_videos(string videoId)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.Music?.Album.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(VideoIds.LiveStream, true)]
    [InlineData(VideoIds.LiveStreamRecording, false)]
    public async Task I_can_get_metadata_of_livestream(string videoId, bool isLive)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.IsLive.Should().Be(isLive);
        video.IsLiveContent.Should().BeTrue();
    }

    [Fact]
    public async Task I_can_get_the_highest_resolution_thumbnail_from_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(VideoIds.Normal);
        var thumbnail = video.Thumbnails.GetWithHighestResolution();

        // Assert
        thumbnail.Url.Should().NotBeNullOrWhiteSpace();
    }
}