using System;

namespace YoutubeReExplode.Exceptions;

/// <summary>
/// Exception thrown within <see cref="YoutubeReExplode" />.
/// </summary>
public class YoutubeReExplodeException(string message) : Exception(message);
