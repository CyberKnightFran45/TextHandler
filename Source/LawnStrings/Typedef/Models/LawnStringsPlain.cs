using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextHandler.LawnStrings
{
/** <summary> Represents PlainText used in some PvZ Games:
<b>PvZ Free and PvZ 2 (v1.0.0 - v6.7.1)</b> </summary> */

public static class LawnStringsPlain
{
#region READER

/** <summary> Determines if a Line is a Section header based on the presence of Brackets: [ and ]. </summary>

// <param name = "line"> The line to check. </param>

<returns> True if the line is a section header; otherwise, false. </returns> */
 
private static bool IsSectionHeader(ReadOnlySpan<char> line)
{
line = line.TrimEnd();

return line.Length >= 2 && line[0] == '[' && line[^1] == ']';
}

private static void EnsureCapacity(ref NativeString str, int required)
{

if(required > str.Length)
str.Realloc(Math.Max(required, str.Length * 2) );

}

// Read Lines from PlainText and add them to a List

public static void ReadList(Stream reader, List<string> lst, LawnStringsEncoding encodeFlags)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);
HashSet<string> seen = new();

int bufferSize = MemoryManager.GetBufferSize(reader);
NativeString currentBlock = new(bufferSize);

int blockLength = 0;
bool usesBom = encodeFlags == LawnStringsEncoding.UTF8_BOM;

bool hasContent = false;
NativeString line;

while( (line = reader.ReadLine(encoding) ) is not null)
{
ReadOnlySpan<char> rawLine = line.AsSpan();
ReadOnlySpan<char> trimmed = rawLine.Trim();

if(usesBom)
{
trimmed = trimmed.TrimStart('\uFEFF');
usesBom = false;
}

#region [HEADER] handling

if(trimmed.Length > 0 && IsSectionHeader(trimmed) )
{

if(blockLength > 0)
{
var span = currentBlock.AsSpan(0, blockLength);

if(span.Length > 0 && span[^1] == '\n')
span = span[..^1];

lst.Add(span.TrimEnd().ToString() );

blockLength = 0;
hasContent = false;
}

string header = trimmed[1..^1].ToString();

if(seen.Contains(header) )
{
using var nextLine = reader.ReadLine(encoding);

if(nextLine == null)
continue;

ReadOnlySpan<char> nextTrimmed = nextLine.AsSpan().Trim();

if(nextTrimmed.Length > 0 && IsSectionHeader(nextTrimmed) )
header = nextTrimmed[1..^1].ToString();

else continue;
}

lst.Add(header);
seen.Add(header);
}

#endregion


#region Text handling

else
{

if(hasContent)
{
EnsureCapacity(ref currentBlock, blockLength + 1);

currentBlock[blockLength++] = '\n';
}

EnsureCapacity(ref currentBlock, blockLength + rawLine.Length);

currentBlock.CopyFrom(rawLine, blockLength);

blockLength += rawLine.Length;
hasContent = true;
}

#endregion
}

// Remove '\n' between lines and Add str to List

if(blockLength > 0)
{
var span = currentBlock.AsSpan(0, blockLength);

if(span.Length > 0 && span[^1] == '\n')
span = span.Slice(0, span.Length - 1);

lst.Add(span.TrimEnd().ToString() );
}

currentBlock.Dispose();
}

// Read Lines from a Stream and add them to a Dictionary

public static void ReadDict(Stream reader, Dictionary<string, string> dict, LawnStringsEncoding encodeFlags)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

int bufferSize = MemoryManager.GetBufferSize(reader);
NativeString currentValue = new(bufferSize);

int strLen = 0;
string currentKey = null;

bool hasContent = false;
bool usesBom = encodeFlags == LawnStringsEncoding.UTF8_BOM;

NativeString line;

while( (line = reader.ReadLine(encoding) ) is not null)
{
ReadOnlySpan<char> rawLine = line.AsSpan();
ReadOnlySpan<char> trimmed = rawLine.Trim();

if(usesBom)
{
trimmed = trimmed.TrimStart('\uFEFF');
usesBom = false;
}

#region [HEADER] handling

if(trimmed.Length > 0 && IsSectionHeader(trimmed) )
{

if(currentKey != null && strLen > 0)
{
var span = currentValue.AsSpan(0, strLen);

if (span.Length > 0 && span[^1] == '\n')
span = span[..^1];

dict[currentKey] = span.TrimEnd().ToString();

strLen = 0;
hasContent = false;
}

string header = trimmed[1..^1].ToString();

if (dict.ContainsKey(header))
{
var nextLine = reader.ReadLine(encoding);

if(nextLine == null)
continue;

ReadOnlySpan<char> nextTrimmed = nextLine.AsSpan().Trim();

if(nextTrimmed.Length > 0 && IsSectionHeader(nextTrimmed) )
header = nextTrimmed[1..^1].ToString();

else continue;
}

currentKey = header;
}

#endregion


#region Text handling

else
{

if(hasContent)
{
EnsureCapacity(ref currentValue, strLen + 1);

currentValue[strLen++] = '\n';
}

EnsureCapacity(ref currentValue, strLen + rawLine.Length);

currentValue.CopyFrom(rawLine, strLen);

strLen += rawLine.Length;
hasContent = true;
}

#endregion
}

// Remove '\n' between lines and Add item to Dict

if(currentKey != null && strLen > 0)
{
var span = currentValue.AsSpan(0, strLen);

if(span.Length > 0 && span[^1] == '\n')
span = span[..^1];

dict[currentKey] = span.TrimEnd().ToString();
}

currentValue.Dispose();
}


#endregion


#region WRITER

// Wrap Key as [HEADER]

private static NativeMemoryOwner<char> WrapKey(ReadOnlySpan<char> key)
{
NativeMemoryOwner<char> hOwner = new(key.Length + 2);
var header = hOwner.AsSpan();

header[0] = '[';
key.CopyTo(header[1..]);
header[^1] = ']';

return hOwner;
}

// Write Lines from list to PlainText

public static void WriteList(Stream writer, List<string> lst, LawnStringsEncoding encodeFlags)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

HashSet<string> seen = new();
var strCount = lst.Count;

for(int i = 0; i < strCount; i += 2)
{

if(i + 1 >= strCount)
break;

string key = lst[i];

if(seen.Contains(key) )
continue;

seen.Add(key);

using var kOwner = WrapKey(key);
writer.WriteLine(kOwner.AsSpan(), encoding);

using var vOwner = LawnStringsHelper.CleanLine(lst[i + 1] );
writer.WriteLine(vOwner.AsSpan(), encoding);
}

seen.Clear();
}

// Write Lines from dict to PlainText

public static void WriteDict(Stream writer, Dictionary<string, string> dict,
LawnStringsEncoding encodeFlags)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

foreach(var pair in dict)
{
using var kOwner = WrapKey(pair.Key);
writer.WriteLine(kOwner.AsSpan(), encoding);

using var vOwner = LawnStringsHelper.CleanLine(pair.Value);
writer.WriteLine(vOwner.AsSpan(), encoding);
}

}

// Write Lines from List<KVP> to PlainText

public static void WriteKvp(Stream writer, IEnumerable<KeyValuePair<string, string>> map,
LawnStringsEncoding encodeFlags)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

foreach(var pair in map)
{
using var kOwner = WrapKey(pair.Key);
writer.WriteLine(kOwner.AsSpan(), encoding);

using var vOwner = LawnStringsHelper.CleanLine(pair.Value);
writer.WriteLine(vOwner.AsSpan(), encoding);
}

}

#endregion


#region SORTER

public static void Sort(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
Dictionary<string, string> map = new();
ReadDict(input, map, encodeFlags);

List<KeyValuePair<string, string>> sorted = [.. map];
map.Clear();

sorted.Sort( (a, b) => LawnStringsHelper.AlphanumCompare(a.Key, b.Key) );
WriteKvp(output, sorted, encodeFlags);
}

#endregion


#region COMPARER

// Get new Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindAdded(Stream a, Stream b,
LawnStringsEncoding encodeFlags, HashSet<string> excludeList = null)
{
excludeList ??= new();

Dictionary<string, string> dictA = new();
ReadDict(a, dictA, encodeFlags);

Dictionary<string, string> dictB = new();
ReadDict(b, dictB, encodeFlags);

return dictB.Where(q => !excludeList.Contains(q.Key) && !dictA.ContainsKey(q.Key) );
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindChanged(Stream a, Stream b,
LawnStringsEncoding encodeFlags, HashSet<string> excludeList = null)
{
excludeList ??= new();

Dictionary<string, string> dictA = new();
ReadDict(a, dictA, encodeFlags);

Dictionary<string, string> dictB = new();
ReadDict(b, dictB, encodeFlags);

var changedStrs = dictB.Where(q => !excludeList.Contains(q.Key) &&
dictA.TryGetValue(q.Key, out var valA) &&
!string.Equals(valA, q.Value, StringComparison.Ordinal) );

return changedStrs;
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindFullDiff(Stream a, Stream b,
LawnStringsEncoding encodeFlags, HashSet<string> excludeList = null)
{
excludeList ??= new();

var addedStrs = FindAdded(a, b, encodeFlags, excludeList);
var changedStrs = FindChanged(a, b, encodeFlags, excludeList);

return addedStrs.Concat(changedStrs);
}

#endregion
}

}