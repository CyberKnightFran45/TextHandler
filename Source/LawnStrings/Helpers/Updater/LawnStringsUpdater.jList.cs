using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// JList updater

public static partial class LawnStringsUpdater
{
// Update JsonList

private static void UpdateJList(Stream oldStream, Stream newStream, Stream patch,
							    HashSet<string> excludeList)
{
LawnStringsHelper.LoadJLists(oldStream, newStream, out var jListA, out var jListB);

var newStrs = LawnStringsComparer.GetDiff(jListA, jListB, LawnStringsCompareMode.Added, excludeList);
int added = newStrs.Count;

TraceLogger.WriteActionStart("Updating strings...");

var oldStrs = jListA.Objects[0].ObjData.LocStringValues;
newStrs.AddRange(oldStrs);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(added);

LawnStrs updated = new();
updated.Objects[0].ObjData.LocStringValues = newStrs;

LawnStringsHelper.SaveJList(patch, updated);
}

}

}