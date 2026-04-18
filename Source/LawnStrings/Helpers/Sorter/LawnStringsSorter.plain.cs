using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
// Text sorter

public static partial class LawnStringsSorter
{
// Sort plaintext

private static void SortPlain(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var map = LawnStringsHelper.ReadDict(input, encodeFlags);

TraceLogger.WriteActionStart("Sorting strings...");

var sorted = map.ToList();
sorted.Sort( (a, b) => LawnStringsComparer.AlphanumCompare(a.Key, b.Key) );

TraceLogger.WriteActionEnd();

LawnStringsHelper.SaveTxt(output, sorted, encodeFlags);
}

}

}