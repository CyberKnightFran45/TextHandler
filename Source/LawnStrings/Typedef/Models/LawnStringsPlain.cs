using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextHandler.LawnStrings
{
/** <summary> Represents PlainText used in some PvZ Games:
<b>PvZ Free and PvZ 2 (v1.0.0 - v6.7.1)</b> </summary> */

public static class LawnStringsPlain
{
// BOM identifier

private const char BOM = '\uFEFF';

#region ============  READER  ============

/** <summary> Determines if a Line is a Section header based on the presence of Brackets: [ and ]. </summary>

<param name = "line"> The line to check. </param>

<returns> <c>true</c> if line is a header; otherwise, <c>false</c> </returns> */
 
private static bool IsSectionHeader(ReadOnlySpan<char> line)
{
line = line.TrimEnd();

return line.Length >= 2 && line[0] == '[' && line[^1] == ']';
}

// Grow ptr if needed

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

while(true)
{
using var line = reader.ReadLine(encoding);

if(line is null)
break;

ReadOnlySpan<char> rawLine = line.AsSpan();
ReadOnlySpan<char> trimmed = rawLine.Trim();

if(usesBom)
{
trimmed = trimmed.TrimStart(BOM);
usesBom = false;
}

if(trimmed.Length > 0 && IsSectionHeader(trimmed) )
{

if(blockLength > 0)
{
var span = currentBlock.AsSpan(0, blockLength);

if(span.Length > 0 && span[^1] == '\n')
span = span[.. ^1];

lst.Add(span.TrimEnd().ToString() );

blockLength = 0;
hasContent = false;
}

string header = trimmed[1..^1].ToString();

if(seen.Contains(header) )
{
using var nextLine = reader.ReadLine(encoding);

if(nextLine is null)
continue;

ReadOnlySpan<char> nextTrimmed = nextLine.AsSpan().Trim();

if(nextTrimmed.Length > 0 && IsSectionHeader(nextTrimmed) )
header = nextTrimmed[1..^1].ToString();

else continue;
}

lst.Add(header);
seen.Add(header);
}

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
span = span[.. ^1];

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

bool usesBom = encodeFlags == LawnStringsEncoding.UTF8_BOM;
bool hasContent = false;

while(true)
{
using var line = reader.ReadLine(encoding);

if(line is null)
break;

ReadOnlySpan<char> rawLine = line.AsSpan();
ReadOnlySpan<char> trimmed = rawLine.Trim();

if(usesBom)
{
trimmed = trimmed.TrimStart(BOM);
usesBom = false;
}

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

#region ============  WRITER  ============

// Write key as [HEADER]

private static void WriteKey(Stream writer, ReadOnlySpan<char> key, EncodingType encoding)
{
writer.WriteByte(0x5B); // '['

writer.WriteString(key, encoding);
writer.WriteByte(0x5D); // ']'

writer.WriteString(Environment.NewLine, encoding);
}

// Encode char with UTF8-BOM or UTF-16 LE

private static void WriteChar(Stream writer, char c, EncodingType encodeFlags)
{
Span<char> charSpan = [ c ];
Span<byte> buffer = stackalloc byte[4]; // UTF-8 worst case

var encoding = encodeFlags.GetEncoding();
int written = encoding.GetBytes(charSpan, buffer);

writer.Write(buffer[.. written] );
}

// Write value

private static void WriteValue(Stream writer, ReadOnlySpan<char> val, EncodingType encoding)
{
bool endsWithNewLine = false;

for(int i = 0; i < val.Length; i++)
{
char c = val[i];

if(c == '\r')
continue;

if(c == '\\' && i + 1 < val.Length && val[i + 1] == 'n')
{
WriteChar(writer, '\n', encoding);
endsWithNewLine = true;

i++;
continue;
}

WriteChar(writer, c, encoding);
endsWithNewLine = c == '\n';
}

if(endsWithNewLine)
WriteChar(writer, '\n', encoding);

else
writer.WriteString("\n\n", encoding);

}

// Write Lines from list to PlainText

public static void WriteList(Stream writer, List<string> lst, LawnStringsEncoding encodeFlags)
{

if(encodeFlags == LawnStringsEncoding.UTF8_BOM)
writer.Write(Encoding.UTF8.GetPreamble() ); // Write BOM identifier

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

WriteKey(writer, key, encoding);
WriteValue(writer, lst[i + 1], encoding);
}

}

// Write Lines from dict to PlainText

public static void WriteDict(Stream writer, Dictionary<string, string> dict,
                             LawnStringsEncoding encodeFlags)
{

if(encodeFlags == LawnStringsEncoding.UTF8_BOM)
writer.Write(Encoding.UTF8.GetPreamble() ); // Write BOM identifier

var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

foreach(var pair in dict)
{
WriteKey(writer, pair.Key, encoding);
WriteValue(writer, pair.Value, encoding);
}

}

// Write Lines from List<KVP> to PlainText

public static void WriteKvp(Stream writer, IEnumerable<KeyValuePair<string, string>> map,
                            LawnStringsEncoding encodeFlags)
{

if(encodeFlags == LawnStringsEncoding.UTF8_BOM)
writer.Write(Encoding.UTF8.GetPreamble() ); // Write BOM identifier

var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

foreach(var pair in map)
{
WriteKey(writer, pair.Key, encoding);
WriteValue(writer, pair.Value, encoding);
}

}

#endregion


#region ============  SORTER  ============

public static void Sort(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
Dictionary<string, string> map = new();
ReadDict(input, map, encodeFlags);

List<KeyValuePair<string, string>> sorted = [.. map];
sorted.Sort( (a, b) => LawnStringsComparer.AlphanumCompare(a.Key, b.Key) );

WriteKvp(output, sorted, encodeFlags);
}

#endregion


#region ============  COMPARER  ============

// Load text from single file into Dictionary

private static Dictionary<string, string> LoadText(Stream reader, LawnStringsEncoding encodeFlags)
{
Dictionary<string, string> dict = new();
ReadDict(reader, dict, encodeFlags);

return dict;
}

// Load files for Comparisson

private static void LoadFiles(Stream a, Stream b, LawnStringsEncoding encodeFlags,
                             out Dictionary<string, string> dictA,
                             out Dictionary<string, string> dictB)
{
dictA = LoadText(a, encodeFlags);
dictB = LoadText(b, encodeFlags);
}

// Get new Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindAdded(Stream a, Stream b,
                                                                  LawnStringsEncoding encodeFlags,
																  HashSet<string> excludeList = null)
{
LoadFiles(a, b, encodeFlags, out var dictA, out var dictB);

return LawnStringsComparer.FindAdded(dictA, dictB, excludeList);
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindChanged(Stream a, Stream b,
                                                                    LawnStringsEncoding encodeFlags,
                                                                    HashSet<string> excludeList = null)
{
LoadFiles(a, b, encodeFlags, out var dictA, out var dictB);

return LawnStringsComparer.FindChanged(dictA, dictB, excludeList);
}

// Get changed Strings between two LawnStrings

public static IEnumerable<KeyValuePair<string, string>> FindFullDiff(Stream a, Stream b,
                                                                     LawnStringsEncoding encodeFlags, 
                                                                     HashSet<string> excludeList = null)
{
LoadFiles(a, b, encodeFlags, out var dictA, out var dictB);

return LawnStringsComparer.FullDiff(dictA, dictB, excludeList);
}

#endregion
}

}