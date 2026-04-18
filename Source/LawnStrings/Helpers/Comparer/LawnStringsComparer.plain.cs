using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// PlainText comparer

public static partial class LawnStringsComparer
{
// Compare plaintext

private static IEnumerable<KeyValuePair<string, string>> GetDiff(Stream a, Stream b,
                                                                 LawnStringsCompareMode mode,
																 LawnStringsEncoding encoding,
									                             HashSet<string> excludeList)
{
LawnStringsHelper.LoadTxts(a, b, encoding, out var dictA, out var dictB);

return GetDiff(dictA, dictB, mode, excludeList);
}

// Compare PlainText

private static void CompareTxt(Stream a, Stream b, Stream diff,
                               LawnStringsCompareMode compareMode,
                               LawnStringsEncoding encodeFlags,
							   HashSet<string> excludeList)
{
TraceLogger.WriteActionStart("Comparing text...");
var result = GetDiff(a, b, compareMode, encodeFlags, excludeList);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Saving diff...");
int ids = LawnStringsPlain.WriteKvp(diff, result, encodeFlags);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(ids);
}

}

}