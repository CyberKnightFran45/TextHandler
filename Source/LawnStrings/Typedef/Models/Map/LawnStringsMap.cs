using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace TextHandler.LawnStrings
{
/** <summary> Represents a Dictionary of Strings used in PvZ 2 <b>(v6.8.1 - 8.8.1)</b>. </summary>

<remarks> <c>NOTE: Serialization doesn't work on multiple herence,
use JsonSerializer directly.</c> </remarks> */

public class LawnStringsMap : SexyObjTable<LawnStringsJsonMap>
{
#region CTOR

/// <summary> Creates a new Instance of the <c>LawnStrings</c>. </summary>

public LawnStringsMap()
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

public static LawnStringsMap FromPlainText(Stream source, LawnStringsEncoding encodeFlags)
{
LawnStringsMap lawnStrs = new();

var dict = lawnStrs.Objects[0].ObjData.LocStringValues;
LawnStringsPlain.ReadDict(source, dict, encodeFlags);

return lawnStrs;
}

/// <summary> Converts this Instance into PlainText. </summary>

public void ToPlainText(Stream target, LawnStringsEncoding encodeFlags)
{
CheckObjs();

var dict = Objects[0].ObjData.LocStringValues;
LawnStringsPlain.WriteDict(target, dict, encodeFlags);
}

/** <summary> Converts this Instance into a List of Strings. </summary>

<returns> The LawnStrings converted as a List of Strings */

public LawnStrings ToList()
{
CheckObjs();

LawnStrings strList = new();

var dict = Objects[0].ObjData.LocStringValues;
var lst = strList.Objects[0].ObjData.LocStringValues;

foreach(var pair in dict)
{
lst.Add(pair.Key);
lst.Add(pair.Value);
}

return strList;
}

#endregion


#region SORTER

public void Sort()
{
CheckObjs();

var dict = Objects[0].ObjData.LocStringValues;

if(dict.Count <= 1)
return;

List<KeyValuePair<string, string>> sorted = [.. dict];

sorted.Sort( (a, b) => LawnStringsHelper.AlphanumCompare(a.Key, b.Key) );

dict.Clear();

foreach(var pair in sorted)
dict[pair.Key] = pair.Value;

}

#endregion


#region COMPARER

// Get new Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindAdded(LawnStringsMap a, LawnStringsMap b,
HashSet<string> excludeList = null)
{
excludeList ??= new();

var dictA = a.Objects[0].ObjData.LocStringValues;
var dictB = b.Objects[0].ObjData.LocStringValues;

var addedStrs = dictB.Where(q => !excludeList.Contains(q.Key) && !dictA.ContainsKey(q.Key) );

return addedStrs;
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindChanged(LawnStringsMap a, LawnStringsMap b,
HashSet<string> excludeList = null)
{
excludeList ??= new();

var dictA = a.Objects[0].ObjData.LocStringValues;
var dictB = b.Objects[0].ObjData.LocStringValues;

var changedStrs = dictB.Where(q => !excludeList.Contains(q.Key) &&
dictA.TryGetValue(q.Key, out var valA) &&
!string.Equals(valA, q.Value, StringComparison.Ordinal) );

return changedStrs;
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindFullDiff(LawnStringsMap a, LawnStringsMap b,
HashSet<string> excludeList = null)
{
excludeList ??= new();

var addedStrs = FindAdded(a, b, excludeList);
var changedStrs = FindChanged(a, b, excludeList);

return addedStrs.Concat(changedStrs);
}

#endregion


#region SERIALIZER

public static readonly LawnStrMapContext Context = new(JsonSerializer.Options);

#endregion
}

// Context for serialization

[JsonSerializable(typeof(LawnStringsMapData) ) ]
[JsonSerializable(typeof(SexyObj<LawnStringsMapData>) ) ]

[JsonSerializable(typeof(LawnStringsJsonMap) ) ]
[JsonSerializable(typeof(List<LawnStringsJsonMap>) ) ]
[JsonSerializable(typeof(SexyObjTable<LawnStringsJsonMap>) ) ]

[JsonSerializable(typeof(LawnStringsMap) ) ]

public partial class LawnStrMapContext : JsonSerializerContext
{
}

}