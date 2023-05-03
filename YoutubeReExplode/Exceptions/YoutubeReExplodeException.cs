using System;

namespace YoutubeReExplode.Exceptions;

/// <summary>
/// Exception thrown within <see cref="YoutubeReExplode" />.
/// </summary>
public class YoutubeReExplodeException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="YoutubeReExplodeException" />.
    /// </summary>
    /// <param name="message"></param>
    public YoutubeReExplodeException(string message) : base(message)
    {
    }
}