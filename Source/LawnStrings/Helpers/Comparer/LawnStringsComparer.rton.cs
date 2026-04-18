using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// Rton comparer

public static partial class LawnStringsComparer
{
// Compare RTON

private static void CompareRton(Stream a, Stream b, Stream diff,
                                LawnStringsCompareMode compareMode,
                                bool useMap,
								HashSet<string> excludeList)
{
LawnStringsHelper.LoadRtons(a, b, out var jsonA, out var jsonB);

using ChunkedMemoryStream jDiff = new();

if(useMap)
CompareJMap(jsonA, jsonB, jDiff, compareMode, excludeList);

else
CompareJList(jsonA, jsonB, jDiff, compareMode, excludeList);

LawnStringsHelper.EncodeRton(jDiff, diff);

jsonA.Dispose();
jsonB.Dispose();
}

}

}