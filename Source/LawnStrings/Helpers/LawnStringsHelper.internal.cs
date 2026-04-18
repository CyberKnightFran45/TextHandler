using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SexyParsers.ReflectiveTypeObjectNotation;

namespace TextHandler.LawnStrings
{
// LawnStrings helper (internal code)

public static partial class LawnStringsHelper
{
// Encode RTON

internal static void EncodeRton(Stream input, Stream output)
{

if(input.Position > 0 && input.CanSeek)
input.Seek(0, SeekOrigin.Begin);

RtonParser.EncodeStream(input, output);
}

// Decode RTON

internal static void DecodeRton(Stream input, Stream output) => RtonParser.DecodeStream(input, output);

// Decode RTON (ram)

internal static ChunkedMemoryStream DecodeRton(Stream input)
{
ChunkedMemoryStream jsonStream = new();

DecodeRton(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

return jsonStream;
}

// Log strings count

private static void DebugStrLoad(int count) => TraceLogger.WriteInfo($"Strings loaded: {count}");

// Log strings count (two tables)

private static void DebugStrLoad(int countA, int countB)
{
TraceLogger.WriteInfo($"Strings loaded: {countA} (old) - {countB} (new)");
}

// Log strings diff

internal static void LogStrDif(int count) => TraceLogger.WriteInfo($"Strings added: {count}");

// Read dictionary of strings

internal static Dictionary<string, string> ReadDict(Stream reader, LawnStringsEncoding encodeFlags)
{
TraceLogger.WriteActionStart("Loading strings...");
var map = LawnStringsPlain.ReadDict(reader, encodeFlags);

TraceLogger.WriteActionEnd();

DebugStrLoad(map.Count);

return map;
}

// Read list of strings

internal static List<string> ReadList(Stream reader, LawnStringsEncoding encodeFlags)
{
TraceLogger.WriteActionStart("Loading strings...");
var lst = LawnStringsPlain.ReadList(reader, encodeFlags);

TraceLogger.WriteActionEnd();

DebugStrLoad(lst.Count);

return lst;
}

// Load plaintext for comparisson

internal static void LoadTxts(Stream a, Stream b, LawnStringsEncoding encodeFlags,
                              out Dictionary<string, string> dictA,
							  out Dictionary<string, string> dictB)
{
TraceLogger.WriteActionStart("Loading text files...");

dictA = LawnStringsPlain.ReadDict(a, encodeFlags);
dictB = LawnStringsPlain.ReadDict(b, encodeFlags);

TraceLogger.WriteActionEnd();

DebugStrLoad(dictA.Count, dictB.Count);
}

// Load JsonList

internal static LawnStrs LoadJList(Stream reader)
{
TraceLogger.WriteActionStart("Loading strings from JsonList...");

var jsonList = JsonSerializer.DeserializeObject<LawnStrs>(reader, LawnStrs.Context);
jsonList.CheckObjs();

TraceLogger.WriteActionEnd();

int count = jsonList.Objects[0].ObjData.LocStringValues.Count;
DebugStrLoad(count);

return jsonList;
}

// Load JLists for comparisson

internal static void LoadJLists(Stream a, Stream b, out LawnStrs lawnA, out LawnStrs lawnB)
{
TraceLogger.WriteActionStart("Loading lists of strings...");

lawnA = JsonSerializer.DeserializeObject<LawnStrs>(a, LawnStrs.Context);
lawnB = JsonSerializer.DeserializeObject<LawnStrs>(b, LawnStrs.Context);

lawnA.CheckObjs();
lawnB.CheckObjs();

TraceLogger.WriteActionEnd();

int strCountA = lawnA.Objects[0].ObjData.LocStringValues.Count;
int strCountB = lawnB.Objects[0].ObjData.LocStringValues.Count;

DebugStrLoad(strCountA, strCountB);
}

// Load JsonMap

internal static LawnStringsMap LoadJMap(Stream reader)
{
TraceLogger.WriteActionStart("Loading strings from JsonMap...");

var jsonMap = JsonSerializer.DeserializeObject<LawnStringsMap>(reader, LawnStringsMap.Context);
jsonMap.CheckObjs();

TraceLogger.WriteActionEnd();

int count = jsonMap.Objects[0].ObjData.LocStringValues.Count;
DebugStrLoad(count);

return jsonMap;
}

// Load JMaps for comparisson

internal static void LoadJMaps(Stream a, Stream b, out LawnStringsMap lawnA, out LawnStringsMap lawnB)
{
TraceLogger.WriteActionStart("Loading dictionaries of strings...");

lawnA = JsonSerializer.DeserializeObject<LawnStringsMap>(a, LawnStringsMap.Context);
lawnB = JsonSerializer.DeserializeObject<LawnStringsMap>(b, LawnStringsMap.Context);

lawnA.CheckObjs();
lawnB.CheckObjs();

TraceLogger.WriteActionEnd();

int strCountA = lawnA.Objects[0].ObjData.LocStringValues.Count;
int strCountB = lawnB.Objects[0].ObjData.LocStringValues.Count;

DebugStrLoad(strCountA, strCountB);
}

// Load Rton streams for comparisson

internal static void LoadRtons(Stream a, Stream b,
                               out ChunkedMemoryStream jsonA,
                               out ChunkedMemoryStream jsonB)
{
TraceLogger.WriteLine("RTON load started:");

TraceLogger.WriteLine("• Decode old stream");
jsonA = DecodeRton(a);

TraceLogger.WriteLine("• Decode new stream");
jsonB = DecodeRton(b);
}

// Save plaintext

internal static void SaveTxt(Stream writer, List<KeyValuePair<string, string>> content,
                             LawnStringsEncoding encodeFlags)
{
TraceLogger.WriteActionStart("Writting text...");
LawnStringsPlain.WriteKvp(writer, content, encodeFlags);

TraceLogger.WriteActionEnd();
}

// Save JsonList

internal static void SaveJList(Stream writer, LawnStrs jsonList)
{
TraceLogger.WriteActionStart("Saving json...");
JsonSerializer.SerializeObject(jsonList, writer, LawnStrs.Context);

TraceLogger.WriteActionEnd();
}

// Save JsonMap

internal static void SaveJMap(Stream writer, LawnStringsMap jsonMap)
{
TraceLogger.WriteActionStart("Saving json...");
JsonSerializer.SerializeObject(jsonMap, writer, LawnStringsMap.Context);

TraceLogger.WriteActionEnd();
}

}

}