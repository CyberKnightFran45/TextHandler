using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
/// <summary> Compares LawnStrings Text and IDs </summary>

public static partial class LawnStringsComparer
{
// Check if char is a numeric Digit

private static bool IsDigit(char c) => c >= '0' && c <= '9';

// Append digit to value

private static long AppendDigit(long val, char digit) => val * 10 + (digit - '0');

// Alphanumeric Comparer

internal static int AlphanumCompare(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
{
int i, j = i = 0;
int lenA = a.Length, lenB = b.Length;

while(i < lenA && j < lenB)
{
char cA = a[i];
char cB = b[j];

bool isDigitA = IsDigit(cA);
bool isDigitB = IsDigit(cB);

if(isDigitA && isDigitB)
{
long numA, numB = numA = 0;

while(i < lenA && (cA = a[i] ) >= '0' && cA <= '9')
{
numA = AppendDigit(numA, cA);

i++;
}

while(j < lenB && (cB = b[j]) >= '0' && cB <= '9')
{
numB = AppendDigit(numB, cB);

j++;
}

int diff = numA.CompareTo(numB);

if(diff != 0)
return diff;

}

else
{
int diff = cA.CompareTo(cB);

if(diff != 0)
return diff;

i++;
j++;
}

}

return lenA.CompareTo(lenB);
}

// New str filter

private static bool NewStringFilter(string id, Dictionary<string, string> src, HashSet<string> excludeList)
{
return !excludeList.Contains(id) && !src.ContainsKey(id);
}

// Find new strings between two Collections

private static IEnumerable<KeyValuePair<string, string>> FindAdded(Dictionary<string, string> a,
                                                                   Dictionary<string, string> b,
																   HashSet<string> excludeList)
{
return b.Where(q => NewStringFilter(q.Key, a, excludeList) );
}

// Changed str filter

private static bool ChangedStringFilter(string id, string text, Dictionary<string, string> src,
                                        HashSet<string> excludeList)
{

return src.TryGetValue(id, out string content) 
       && !excludeList.Contains(id)
       && !string.Equals(content, text, StringComparison.Ordinal);

}

// Find strings changed between two Collections

private static IEnumerable<KeyValuePair<string, string>> FindChanged(Dictionary<string, string> a,
                                                                     Dictionary<string, string> b,
																     HashSet<string> excludeList)
{
return b.Where(q => ChangedStringFilter(q.Key, q.Value, a, excludeList) );
}

// Get full difference between two Collections

private static IEnumerable<KeyValuePair<string, string>> FullDiff(Dictionary<string, string> a,
                                                                  Dictionary<string, string> b,
																  HashSet<string> excludeList)
{
var addedStrs = FindAdded(a, b, excludeList);
var changedStrs = FindChanged(a, b, excludeList);

return addedStrs.Concat(changedStrs);
}

// Compare raw text

internal static IEnumerable<KeyValuePair<string, string>> GetDiff(Dictionary<string, string> a,
                                                                  Dictionary<string, string> b,
																  LawnStringsCompareMode mode,
																  HashSet<string> excludeList)
{

return mode switch
{
LawnStringsCompareMode.Changed => FindChanged(a, b, excludeList),
LawnStringsCompareMode.FullDiff => FullDiff(a, b, excludeList),
_ => FindAdded(a, b, excludeList),
};

}

// Compare LawnStrings

public static void Compare(Stream a, Stream b, Stream diff,
                           LawnStringsFormat format,
                           LawnStringsCompareMode compareMode,
						   HashSet<string> excludeList = null,
                           LawnStringsEncoding encodeFlags = default)
{
excludeList ??= new();

switch(format)
{
case LawnStringsFormat.JsonList:
CompareJList(a, b, diff, compareMode, excludeList);
break;

case LawnStringsFormat.JsonMap:
CompareJMap(a, b, diff, compareMode, excludeList);
break;

case LawnStringsFormat.RtonList:
CompareRton(a, b, diff, compareMode, false, excludeList);
break;

case LawnStringsFormat.RtonMap:
CompareRton(a, b, diff, compareMode, true, excludeList);
break;

default:
CompareTxt(a, b, diff, compareMode, encodeFlags, excludeList);
break;
}

}

}

}