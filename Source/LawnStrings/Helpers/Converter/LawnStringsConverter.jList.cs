using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
// JsonList converter

public static partial class LawnStringsConverter
{
// Convert list to plaintext

private static void JList2Txt(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonList = LawnStringsHelper.LoadJList(input);

TraceLogger.WriteActionStart("Converting JsonList to PlainText...");

var lst = jsonList.Objects[0].ObjData.LocStringValues;
LawnStringsPlain.WriteList(output, lst, encodeFlags);

TraceLogger.WriteActionEnd();
}

// Convert list to map

private static LawnStringsMap ToMap(LawnStrs jsonList)
{
LawnStringsMap strMap = new();
HashSet<string> seen = new();

var lst = jsonList.Objects[0].ObjData.LocStringValues;
var dict = strMap.Objects[0].ObjData.LocStringValues;

int strCount = lst.Count - 1;

for(int i = 0; i < strCount; i += 2)
{
string key = lst[i];

if(seen.Contains(key) )
continue;

seen.Add(key);

string val = lst[i + 1];
dict.Add(key, val);
}

return strMap;
}

// Convert JsonList to JsonMap

private static void JList2JMap(Stream input, Stream output)
{
var jsonList = LawnStringsHelper.LoadJList(input);

TraceLogger.WriteActionStart("Converting JsonList to JsonMap...");
var jsonMap = ToMap(jsonList);

TraceLogger.WriteActionEnd();

LawnStringsHelper.SaveJMap(output, jsonMap);
}

// Convert JsonList to RtonMap

private static void JList2RMap(Stream input, Stream output)
{
using ChunkedMemoryStream jsonStream = new();
JList2JMap(input, jsonStream);

LawnStringsHelper.EncodeRton(jsonStream, output);
}

// JsonList Conversion

private static void FromJList(Stream input, Stream output, LawnStringsFormat destFormat,
                              LawnStringsEncoding encodeFlags = default)
{

switch(destFormat)
{
case LawnStringsFormat.JsonMap:
JList2JMap(input, output);
break;

case LawnStringsFormat.RtonList:
LawnStringsHelper.EncodeRton(input, output);
break;

case LawnStringsFormat.RtonMap:
JList2RMap(input, output);
break;

default:
JList2Txt(input, output, encodeFlags);
break;
}

}

}

}