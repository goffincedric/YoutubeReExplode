namespace YoutubeReExplode.Exceptions;

/// <summary>
/// Exception thrown when the requested playlist is unavailable.
/// </summary>
public class PlaylistUnavailableException(string message) : YoutubeReExplodeException(message);
