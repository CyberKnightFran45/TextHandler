using System;
using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// Json List comparer

public static partial class LawnStringsComparer
{
// Find elements that match predicate

private static List<string> FindCore(List<string> srcList, HashSet<string> excludeList,
                                     Func<string, string, bool> predicate)
{
List<string> result = new();
HashSet<string> seen = new();

for(int i = 0; i < srcList.Count - 1; i += 2)
{
string key = srcList[i];
string val = srcList[i + 1];

bool shouldIgnore = seen.Contains(key) || excludeList.Contains(key);

if(shouldIgnore)
continue;

if(predicate(key, val) )
{
result.Add(key);
result.Add(val);

seen.Add(key);
}

}

return result;
}

// Get new Strings between two LawnStrings

private static List<string> FindAdded(LawnStrs a, LawnStrs b, HashSet<string> excludeList)
{
HashSet<string> baseKeys = new();

var listA = a.Objects[0].ObjData.LocStringValues;
var listB = b.Objects[0].ObjData.LocStringValues;

for(int i = 0; i < listA.Count - 1; i += 2)
baseKeys.Add(listA[i] );

bool isNew(string key, string val) => !baseKeys.Contains(key);

return FindCore(listB, excludeList, isNew);
}

// Get changed Strings between two LawnStrings

private static List<string> FindChanged(LawnStrs a, LawnStrs b, HashSet<string> excludeList)
{
Dictionary<string, string> baseDict = new();

var listA = a.Objects[0].ObjData.LocStringValues;
var listB = b.Objects[0].ObjData.LocStringValues;

for(int i = 0; i < listA.Count - 1; i += 2)
baseDict[listA[i] ] = listA[i + 1];

bool isChanged(string key, string val)
{

return baseDict.TryGetValue(key, out var baseStr)
       && !string.Equals(baseStr, val, StringComparison.Ordinal);
   
};

return FindCore(listB, excludeList, isChanged);
}

// Get Strings diff between two LawnStrings

private static List<string> FullDiff(LawnStrs a, LawnStrs b, HashSet<string> excludeList)
{
var diff = FindChanged(a, b, excludeList);
var added = FindAdded(a, b, excludeList);

diff.AddRange(added);

return diff;
}

// Compare json list

internal static List<string> GetDiff(LawnStrs a, LawnStrs b, LawnStringsCompareMode mode,
								     HashSet<string> excludeList)
{

return mode switch
{
LawnStringsCompareMode.Changed => FindChanged(a, b, excludeList),
LawnStringsCompareMode.FullDiff => FullDiff(a, b, excludeList),
_ => FindAdded(a, b, excludeList),
};

}

// Compare JList

private static void CompareJList(Stream a, Stream b, Stream diff,
                                 LawnStringsCompareMode compareMode,
                                 HashSet<string> excludeList)
{
LawnStringsHelper.LoadJLists(a, b, out var jListA, out var jListB);

TraceLogger.WriteActionStart("Comparing text...");
var result = GetDiff(jListA, jListB, compareMode, excludeList);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Saving diff...");
JsonSerializer.SerializeObject(result, diff);

TraceLogger.WriteActionEnd();

LawnStringsHelper.LogStrDif(result.Count);
}

}

}