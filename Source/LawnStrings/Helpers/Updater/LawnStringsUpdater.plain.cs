using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// PlainText updater

public static partial class LawnStringsUpdater
{
// Update PlainText

private static void UpdateTxt(Stream oldStream, Stream newStream, Stream patch,
                              LawnStringsEncoding encodeFlags,
							  HashSet<string> excludeList)
{
LawnStringsHelper.LoadTxts(oldStream, newStream, encodeFlags, out var dictA, out var dictB);

var diff = LawnStringsComparer.GetDiff(dictA, dictB, LawnStringsCompareMode.Added, excludeList);

TraceLogger.WriteActionStart("Updating strings...");

int added = LawnStringsPlain.WriteKvp(patch, diff, encodeFlags);
LawnStringsPlain.WriteDict(patch, dictA, encodeFlags);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(added);
}

}

}