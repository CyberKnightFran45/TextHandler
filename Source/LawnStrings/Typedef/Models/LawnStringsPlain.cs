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

private const string BOM = "\uFEFF";

#region ============  READER  ============

/** <summary> Determines if a Line is a Section header based on the presence of Brackets: [ and ]. </summary>

<param name = "line"> The line to check. </param>

<returns> <c>true</c> if line is a header; otherwise, <c>false</c> </returns> */
 
private static bool IsSectionHeader(NativeString line)
{
return line.Length >= 2 && line[0] == '[' && line[line.Length - 1] == ']';
}

// Grow ptr if needed

private static void EnsureCapacity(ref NativeString str, long required)
{

if(required > str.Length)
str.Realloc(Math.Max(required, str.Length * 2) );

}

// Section reader

private static void ReadSections(Stream reader, LawnStringsEncoding encodeFlags,
                                 Action<string> onHeader,
                                 Action<NativeString, int> onContent)
{
var encoding = LawnStringsHelper.GetEncoding(encodeFlags);

int bufferSize = MemoryManager.GetBufferSize(reader);
NativeString currentBlock = new(bufferSize);

int blockLength = 0;

bool hasLine = false;
bool hasText = false;

bool usesBom = encodeFlags == LawnStringsEncoding.UTF8_BOM;

while(true)
{
using var line = reader.ReadLine(encoding);

if(line is null)
break;

line.Trim();

if(usesBom)
{
line.TrimStart(BOM);

usesBom = false;
}

// [HEADER]

if(line.Length > 0 && IsSectionHeader(line) )
{

if(hasText)
onContent(currentBlock, blockLength);

blockLength = 0;

hasLine = false;
hasText = false;

string header = line.Substring(1, (int)(line.Length - 2) );
onHeader(header);

continue;
}

// Content

if(hasLine)
{
EnsureCapacity(ref currentBlock, blockLength + 1);

currentBlock[blockLength++] = '\n';
}

EnsureCapacity(ref currentBlock, blockLength + line.Length);

currentBlock.CopyFrom(line, blockLength);
blockLength += (int)line.Length;

hasLine = true;

if(line.Length > 0)
hasText = true;

}

// Last block

if(hasText)
onContent(currentBlock, blockLength);

currentBlock.Dispose();
}

// Read Lines from PlainText and add them to a List

public static void ReadList(Stream reader, List<string> lst, LawnStringsEncoding encodeFlags)
{
 HashSet<string> seen = new();

void OnHeader(string header)
{

if(seen.Add(header) )
lst.Add(header);

};

void OnContent(NativeString buffer, int len)
{

if(len <= 0)
return;

bool endsWithNewLine = buffer[len - 1] == '\n';
var text = buffer.Substring(0, endsWithNewLine ? len - 1 : len);

lst.Add(text);
};

ReadSections(reader, encodeFlags, OnHeader, OnContent);
}

// Read Lines from a Stream and add them to a Dictionary

public static void ReadDict(Stream reader, Dictionary<string, string> dict, LawnStringsEncoding encodeFlags)
{
string currentKey = null;

void OnHeader(string header) => currentKey = header;

void OnContent(NativeString buffer, int len)
{

if(currentKey is null || len <= 0)
return;

bool endsWithNewLine = buffer[len - 1] == '\n';

dict[currentKey] = buffer.Substring(0, endsWithNewLine ? len - 1 : len);
}

ReadSections(reader, encodeFlags, OnHeader, OnContent);
}

#endregion


#region ============  WRITER  ============

// Write key as [HEADER]

private static void WriteKey(Stream writer, ReadOnlySpan<char> key, EncodingType encoding)
{
writer.WriteByte(0x5B); // '['

writer.WriteString(key, encoding);
writer.WriteByte(0x5D); // ']'

writer.WriteString(Environment.NewLine, encoding);
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
writer.WriteChar('\n', encoding);
endsWithNewLine = true;

i++;
continue;
}

writer.WriteChar(c, encoding);
endsWithNewLine = c == '\n';
}

if(endsWithNewLine)
writer.WriteChar('\n', encoding);

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