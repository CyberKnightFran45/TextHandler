using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;

namespace TextHandler.LawnStrings
{
/** <summary> Represents a List of Strings used in PvZ 2 <b>(v8.9.1 and on)</b>. </summary>

<remarks> <c>NOTE: Serialization doesn't work on multiple herence,
use JsonSerializer directly. </c> </remarks> */

public class LawnStrings : SexyObjTable<LawnStringsJsonData>
{
#region CTOR

/// <summary> Creates a new Instance of the <c>LawnStrings</c>. </summary>

public LawnStrings()
{
Objects ??= new();

Objects.Add( new() );

Objects[0].ObjData = new()
{
LocStringValues = new()
};

}

// Check for null Fields

public override void CheckObjs()
{
Objects ??= new();

if(Objects.Count == 0)
Objects.Add( new() );

Objects[0] ??= new();
Objects[0].ObjData ??= new();

Objects[0].ObjData.LocStringValues ??= new();
}

#endregion


#region CONVERTER

/// <summary> Creates a new Instance of the <c>LawnStrings</c> from PlainText. </summary>

public static LawnStrings FromPlainText(Stream source, LawnStringsEncoding encoding)
{
LawnStrings lawnStrs = new();

var lst = lawnStrs.Objects[0].ObjData.LocStringValues;
LawnStringsPlain.ReadList(source, lst, encoding);

return lawnStrs;
}

/// <summary> Converts this Instance into PlainText. </summary>

public void ToPlainText(Stream target, LawnStringsEncoding encodeFlags)
{
CheckObjs();

var lst = Objects[0].ObjData.LocStringValues;
LawnStringsPlain.WriteList(target, lst, encodeFlags);
}

/** <summary> Converts this Instance into a Dictionary of Strings. </summary>

<returns> The LawnStrings converted as a Dictionary of Strings </returns> */

public LawnStringsMap ToMap()
{
CheckObjs();

LawnStringsMap strMap = new();
HashSet<string> seen = new();

var lst = Objects[0].ObjData.LocStringValues;
var dict = strMap.Objects[0].ObjData.LocStringValues;

var strCount = Objects[0].ObjData.LocStringValues.Count - 1;

for(int i = 0; i < strCount; i += 2)
{
string key = lst[i];

if(seen.Contains(key) )
continue;

seen.Add(key);

string val = lst[i + 1];
dict.Add(key, val);
}

seen.Clear();

return strMap;
}

#endregion


#region SORTER

public void Sort()
{
CheckObjs();

var list = Objects[0].ObjData.LocStringValues;
var comparer = CultureInfo.InvariantCulture.CompareInfo;

QuickSort(list, 0, list.Count / 2 - 1, comparer);
}

private static void QuickSort(List<string> list, int left, int right, CompareInfo comparer)
{

if(left >= right)
return;

int pivotIndex = Partition(list, left, right, comparer);

QuickSort(list, left, pivotIndex - 1, comparer);
QuickSort(list, pivotIndex + 1, right, comparer);
}

private static int Partition(List<string> list, int left, int right, CompareInfo comparer)
{
string pivotKey = list[right * 2];
int i = left - 1;

var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols 
               | CompareOptions.IgnoreNonSpace | CompareOptions.StringSort;

for(int j = left; j < right; j++)
{
string currentKey = list[j * 2];
int cmp = comparer.Compare(currentKey, pivotKey, options);

if(cmp <= 0)
{
i++;
SwapPairs(list, i, j);
}

}

SwapPairs(list, i + 1, right);

return i + 1;
}

private static void SwapPairs(List<string> list, int indexA, int indexB)
{
int iA = indexA * 2;
int iB = indexB * 2;

string tempKey = list[iA];
string tempVal = list[iA + 1];

list[iA] = list[iB];
list[iA + 1] = list[iB + 1];

list[iB] = tempKey;
list[iB + 1] = tempVal;
}

#endregion


#region COMPARER

// Get new Strings between two LawnStrings

public static List<string> FindAdded(LawnStrings a, LawnStrings b, HashSet<string> excludeList = null)
{
var listA = a.Objects[0].ObjData.LocStringValues;
var listB = b.Objects[0].ObjData.LocStringValues;

HashSet<string> baseKeys = new();

for(int i = 0; i < listA.Count - 1; i += 2)
baseKeys.Add(listA[i]);

List<string> newStrs = new();

for(int i = 0; i < listB.Count - 1; i += 2)
{
string key = listB[i];
string val = listB[i + 1];

if( (excludeList?.Contains(key) ?? false) || baseKeys.Contains(key) )
continue;

newStrs.Add(key);
newStrs.Add(val);
}

return newStrs;
}

// Get changed Strings between two LawnStrings

public static List<string> FindChanged(LawnStrings a, LawnStrings b, HashSet<string> excludeList = null)
{
Dictionary<string, string> baseDict = new();

var listA = a.Objects[0].ObjData.LocStringValues;
var listB = b.Objects[0].ObjData.LocStringValues;

for(int i = 0; i < listA.Count - 1; i += 2)
baseDict[listA[i]] = listA[i + 1];

HashSet<string> seen = new();
List<string> changedStrs = new();

for(int i = 0; i < listB.Count - 1; i += 2)
{
string key = listB[i];
string val = listB[i + 1];

if( (excludeList?.Contains(key) ?? false) || !baseDict.ContainsKey(key) )
continue;

if(seen.Contains(key) )
continue;

if(!string.Equals(baseDict[key], val, StringComparison.Ordinal) )
{
changedStrs.Add(key);
changedStrs.Add(val);

seen.Add(key);
}

}

return changedStrs;
}

// Get Strings diff between two LawnStrings

public static List<string> FindFullDiff(LawnStrings a, LawnStrings b, HashSet<string> excludeList = null)
{
Dictionary<string, string> baseDict = new();

var listA = a.Objects[0].ObjData.LocStringValues;
var listB = b.Objects[0].ObjData.LocStringValues;

for(int i = 0; i < listA.Count - 1; i += 2)
baseDict[listA[i]] = listA[i + 1];

HashSet<string> seen = new();
List<string> changedStrs = new();

for(int i = 0; i < listB.Count - 1; i += 2)
{
string key = listB[i];
string val = listB[i + 1];

if(excludeList?.Contains(key) ?? false)
continue;

if(seen.Contains(key) )
continue;

if(!baseDict.TryGetValue(key, out var baseVal) || !string.Equals(baseVal, val, StringComparison.Ordinal) )
{
changedStrs.Add(key);
changedStrs.Add(val);

seen.Add(key);
}

}

return changedStrs;
}

#endregion

public static readonly LawnStrContext Context = new(JsonSerializer.Options);
}

// Context for serialization

[JsonSerializable(typeof(LawnStringsData) ) ]
[JsonSerializable(typeof(SexyObj<LawnStringsData>) ) ]

[JsonSerializable(typeof(LawnStringsJsonData) ) ]
[JsonSerializable(typeof(List<LawnStringsJsonData>) ) ]
[JsonSerializable(typeof(SexyObjTable<LawnStringsJsonData>) ) ]

[JsonSerializable(typeof(LawnStrings) ) ]

public partial class LawnStrContext : JsonSerializerContext
{
}

}