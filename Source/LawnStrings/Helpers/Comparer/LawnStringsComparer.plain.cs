using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// PlainText comparer

public static partial class LawnStringsComparer
{
// Compare PlainText

private static void CompareTxt(Stream a, Stream b, Stream diff,
                               LawnStringsCompareMode compareMode,
                               LawnStringsEncoding encodeFlags,
							   HashSet<string> excludeList)
{
LawnStringsHelper.LoadTxts(a, b, encodeFlags, out var dictA, out var dictB);

TraceLogger.WriteActionStart("Comparing text...");
var result = GetDiff(dictA, dictB, compareMode, excludeList);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Saving diff...");
int ids = LawnStringsPlain.WriteKvp(diff, result, encodeFlags);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(ids);
}

}

}
