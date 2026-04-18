using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// Rton updater

public static partial class LawnStringsUpdater
{
// Update Rton

private static void UpdateRton(Stream oldStream, Stream newStream, Stream patch,
							   bool useMap, HashSet<string> excludeList)
{
LawnStringsHelper.LoadRtons(oldStream, newStream, out var jsonA, out var jsonB);

if(useMap)
UpdateJMap(jsonA, jsonB, patch, excludeList);

else
UpdateJList(jsonA, jsonB, patch, excludeList);

jsonA.Dispose();
jsonB.Dispose();
}

}

}