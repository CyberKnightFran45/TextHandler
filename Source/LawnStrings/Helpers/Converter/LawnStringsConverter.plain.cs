using System.IO;

namespace TextHandler.LawnStrings
{
// Text converter

public static partial class LawnStringsConverter
{
// Convert Raw Txt from UTF8 (BOM) to UTF16 (Little Endian)

private static void Utf8BomToUtf16(Stream input, Stream output)
{
TraceLogger.WriteActionStart("Reading raw txt...");

using var rawTxt = input.ReadString(EncodingType.UTF8);
rawTxt.TrimStart("\uFEFF"); // Remove BOM

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Converting to UTF-16...");
output.WriteString(rawTxt.AsSpan(), EncodingType.UTF16);

TraceLogger.WriteActionEnd();
}

// Create instance of LawnStrings from plaintxt

private static LawnStrs FromPlainList(Stream input, LawnStringsEncoding encodeFlags)
{
var list = LawnStringsHelper.ReadList(input, encodeFlags);

TraceLogger.WriteActionStart("Converting PlainText to JsonList...");

LawnStrs jsonList = new();
jsonList.Objects[0].ObjData.LocStringValues = list;

TraceLogger.WriteActionEnd();

return jsonList;
}

// Convert Txt to JsonList

private static void Txt2JList(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonList = FromPlainList(input, encodeFlags);

LawnStringsHelper.SaveJList(output, jsonList);
}

// Create instance of LawnStringsMap from plaintxt

private static LawnStringsMap FromPlainDict(Stream input, LawnStringsEncoding encodeFlags)
{
var dict = LawnStringsHelper.ReadDict(input, encodeFlags);

TraceLogger.WriteActionStart("Converting PlainText to JsonMap...");

LawnStringsMap jsonMap = new();
jsonMap.Objects[0].ObjData.LocStringValues = dict;

TraceLogger.WriteActionEnd();

return jsonMap;
}

// Convert Txt to JsonMap

private static void Txt2JMap(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonMap = FromPlainDict(input, encodeFlags);

LawnStringsHelper.SaveJMap(output, jsonMap);
}

// Convert Txt to RTON

private static void Txt2Rton(Stream input, Stream output,
                             LawnStringsEncoding encodeFlags,
							 bool useMap)
{
using ChunkedMemoryStream jsonStream = new();

if(useMap)
Txt2JMap(input, jsonStream, encodeFlags);

else
Txt2JList(input, jsonStream, encodeFlags);

LawnStringsHelper.EncodeRton(jsonStream, output);
}

// PlainText Conversion

private static void FromPlain(Stream input, Stream output, LawnStringsFormat destFormat,
                              LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
Txt2JList(input, output, encodeFlags);
break;

case LawnStringsFormat.JsonMap:
Txt2JMap(input, output, encodeFlags);
break;

case LawnStringsFormat.RtonList:
Txt2Rton(input, output, encodeFlags, false);
break;

case LawnStringsFormat.RtonMap:
Txt2Rton(input, output, encodeFlags, true);
break;

default:
Utf8BomToUtf16(input, output);
break;
}

}

}

}