using System.IO;

namespace TextHandler.LawnStrings
{
// RTON sorter

public static partial class LawnStringsSorter
{
// Sort RTON

private static void SortRton(Stream input, Stream output, bool useMap)
{
using var rawUnsorted = LawnStringsHelper.DecodeRton(input);
using ChunkedMemoryStream rawSorted = new();

if(useMap)
SortJMap(rawUnsorted, rawSorted);

else
SortJList(rawUnsorted, rawSorted);

LawnStringsHelper.EncodeRton(rawSorted, output);
}

}

}