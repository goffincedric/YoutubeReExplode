namespace YoutubeReExplode.Exceptions;

/// <summary>
/// Exception thrown when YouTube denies a request because the client has exceeded rate limit.
/// </summary>
public class RequestLimitExceededException(string message) : YoutubeReExplodeException(message);
