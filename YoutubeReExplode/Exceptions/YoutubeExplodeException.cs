using System;

namespace YoutubeReExplode.Exceptions;

/// <summary>
/// Exception thrown within <see cref="YoutubeReExplode" />.
/// </summary>
public class YoutubeExplodeException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="YoutubeExplodeException" />.
    /// </summary>
    /// <param name="message"></param>
    public YoutubeExplodeException(string message) : base(message)
    {
    }
}