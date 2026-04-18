using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
// Json Map comparer

public static partial class LawnStringsComparer
{
// Compare json list

internal static List<KeyValuePair<string, string>> GetDiff(LawnStringsMap a, LawnStringsMap b,
                                                           LawnStringsCompareMode mode,
									                       HashSet<string> excludeList)
{
var dictA = a.Objects[0].ObjData.LocStringValues;
var dictB = b.Objects[0].ObjData.LocStringValues;

var diff = mode switch
{
LawnStringsCompareMode.Changed => FindChanged(dictA, dictB, excludeList),
LawnStringsCompareMode.FullDiff => FullDiff(dictA, dictB, excludeList),
_ => FindAdded(dictA, dictB, excludeList),
};

return diff.ToList();
}

// Compare JsonMap

private static void CompareJMap(Stream a, Stream b, Stream diff,
                                LawnStringsCompareMode compareMode,
                                HashSet<string> excludeList)
{
LawnStringsHelper.LoadJMaps(a, b, out var jMapA, out var jMapB);

TraceLogger.WriteActionStart("Comparing text...");
var result = GetDiff(jMapA, jMapB, compareMode, excludeList);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Saving diff...");
JsonSerializer.SerializeObject(result, diff);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(result.Count);
}

}

}