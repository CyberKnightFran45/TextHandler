using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
// JMap updater

public static partial class LawnStringsUpdater
{
// Update JsonMap

private static void UpdateJMap(Stream oldStream, Stream newStream, Stream patch,
							   HashSet<string> excludeList)
{
LawnStringsHelper.LoadJMaps(oldStream, newStream, out var jMapA, out var jMapB);

var newStrs = LawnStringsComparer.GetDiff(jMapA, jMapB, LawnStringsCompareMode.Added, excludeList);
int added = newStrs.Count;

TraceLogger.WriteActionStart("Updating strings...");

var oldStrs = jMapA.Objects[0].ObjData.LocStringValues;
newStrs.AddRange(oldStrs);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(added);

LawnStringsMap updated = new();
updated.Objects[0].ObjData.LocStringValues = newStrs.ToDictionary();

LawnStringsHelper.SaveJMap(patch, updated);
}

}

}