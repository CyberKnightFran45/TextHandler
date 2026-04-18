using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
// JsonMap sorter

public static partial class LawnStringsSorter
{
// Sort LawnStringsMap

private static void Sort(LawnStringsMap jsonMap)
{
var dict = jsonMap.Objects[0].ObjData.LocStringValues;

if(dict.Count <= 1)
return;

var sorted = dict.ToList();
sorted.Sort( (a, b) => LawnStringsComparer.AlphanumCompare(a.Key, b.Key) );

foreach(var pair in sorted)
dict[pair.Key] = pair.Value;

}

// Sort JsonMap

private static void SortJMap(Stream input, Stream output)
{
var jsonMap = LawnStringsHelper.LoadJMap(input);

TraceLogger.WriteActionStart("Sorting strings...");
Sort(jsonMap);

TraceLogger.WriteActionEnd();

LawnStringsHelper.SaveJMap(output, jsonMap);
}

}

}