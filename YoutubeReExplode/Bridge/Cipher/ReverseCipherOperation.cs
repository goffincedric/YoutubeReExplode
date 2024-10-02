using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Bridge.Cipher;

internal class ReverseCipherOperation : ICipherOperation
{
    public string Decipher(string input) => input.Reverse();

    [ExcludeFromCodeCoverage]
    public override string ToString() => "Reverse";
}
