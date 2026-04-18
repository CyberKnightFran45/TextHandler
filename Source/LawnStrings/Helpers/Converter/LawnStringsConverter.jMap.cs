using System.IO;

namespace TextHandler.LawnStrings
{
// JsonMap converter

public static partial class LawnStringsConverter
{
// Convert map to plaintext

private static void JMap2Txt(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonMap = LawnStringsHelper.LoadJMap(input);

TraceLogger.WriteActionStart("Converting JsonMap to PlainText...");

var dict = jsonMap.Objects[0].ObjData.LocStringValues;
LawnStringsPlain.WriteDict(output, dict, encodeFlags);

TraceLogger.WriteActionEnd();
}

// Convert map to list

private static LawnStrs ToList(LawnStringsMap jsonMap)
{
LawnStrs strList = new();

var dict = jsonMap.Objects[0].ObjData.LocStringValues;
var lst = strList.Objects[0].ObjData.LocStringValues;

foreach(var pair in dict)
{
lst.Add(pair.Key);
lst.Add(pair.Value);
}

return strList;
}

// Convert JsonMap to JsonList

private static void JMap2JList(Stream input, Stream output)
{
var jsonMap = LawnStringsHelper.LoadJMap(input);

TraceLogger.WriteActionStart("Converting JsonMap to JsonList...");
var jsonList = ToList(jsonMap);

TraceLogger.WriteActionEnd();

LawnStringsHelper.SaveJList(output, jsonList);
}

// Convert JsonMap to RtonList

private static void JMap2RList(Stream input, Stream output)
{
using ChunkedMemoryStream jsonStream = new();
JMap2JList(input, jsonStream);

LawnStringsHelper.EncodeRton(jsonStream, output);
}

// JsonMap Conversion

private static void FromJMap(Stream input, Stream output, 
                             LawnStringsFormat destFormat,
                             LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
JMap2JList(input, output);
break;

case LawnStringsFormat.RtonList:
JMap2RList(input, output);
break;

case LawnStringsFormat.RtonMap:
LawnStringsHelper.EncodeRton(input, output);
break;

default:
JMap2Txt(input, output, encodeFlags);
break;
}

}

}

}