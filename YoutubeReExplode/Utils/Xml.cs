using System.Xml.Linq;
using YoutubeReExplode.Utils.Extensions;

namespace YoutubeReExplode.Utils;

internal static class Xml
{
    public static XElement Parse(string source) =>
        XElement.Parse(source, LoadOptions.PreserveWhitespace).StripNamespaces();
}
