using System;
using System.Collections.Generic;
using System.Linq;

namespace TextHandler.LawnStrings
{
/// <summary> Compares LawnStrings Text and IDs </summary>

public static class LawnStringsComparer
{
// Check if char is a numeric Digit

private static bool IsDigit(char c) => c >= '0' && c <= '9';

// Append digit to value

private static long AppendDigit(long val, char digit) => val * 10 + (digit - '0');

// Alphanumeric Comparer

public static int AlphanumCompare(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
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

// Find new strings between two Collections

public static IEnumerable<KeyValuePair<string, string>> FindAdded(Dictionary<string, string> a,
                                                                  Dictionary<string, string> b,
																  HashSet<string> excludeList)
{
excludeList ??= new();

return b.Where(q => !excludeList.Contains(q.Key) && !a.ContainsKey(q.Key) );
}

// Find strings changed between two Collections

public static IEnumerable<KeyValuePair<string, string>> FindChanged(Dictionary<string, string> a,
                                                                    Dictionary<string, string> b,
																    HashSet<string> excludeList)
{
excludeList ??= new();

return b.Where(q => !excludeList.Contains(q.Key) &&
               a.TryGetValue(q.Key, out var strA) &&
               !string.Equals(strA, q.Value, StringComparison.Ordinal) );

}

// Get full difference between two Collctions

public static IEnumerable<KeyValuePair<string, string>> FullDiff(Dictionary<string, string> a,
                                                                 Dictionary<string, string> b,
																 HashSet<string> excludeList)
{
excludeList ??= new();

var addedStrs = FindAdded(a, b, excludeList);
var changedStrs = FindChanged(a, b, excludeList);

return addedStrs.Concat(changedStrs);
}

}

}