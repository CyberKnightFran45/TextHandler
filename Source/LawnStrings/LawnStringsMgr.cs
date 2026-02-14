using LawnStrs = TextHandler.LawnStrings.LawnStrings;

using System.IO;
using SexyParsers.ReflectionObjectNotation;
using System.Collections.Generic;
using System;

namespace TextHandler.LawnStrings
{
/// <summary> Manages LawnStrings files, such as Conversion, Comparing and Sorting. </summary>

public static class LawnStringsMgr
{
#region CONVERTER

// Convert Raw Txt from UTF8 (BOM) to UTF16 (Little Endian)

private static void Utf8Bom2U16(Stream input, Stream output)
{
var jsonMap = LawnStringsMap.FromPlainText(input, LawnStringsEncoding.UTF8_BOM);

jsonMap.ToPlainText(output, LawnStringsEncoding.UTF16);
}

// Convert Txt to JsonList

private static void Txt2JList(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonList = LawnStrs.FromPlainText(input, encodeFlags);

JsonSerializer.SerializeObject(jsonList, output, LawnStrs.Context);
}

// Convert Txt to JsonMap

private static void Txt2JMap(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
var jsonMap = LawnStringsMap.FromPlainText(input, encodeFlags);

JsonSerializer.SerializeObject(jsonMap, output, LawnStringsMap.Context);
}

// Convert Txt to RTON

private static void Txt2Rton(Stream input, Stream output, LawnStringsEncoding encodeFlags, bool useMap)
{
using ChunkedMemoryStream jsonStream = new();

if(useMap)
Txt2JMap(input, jsonStream, encodeFlags);

else
Txt2JList(input, jsonStream, encodeFlags);

jsonStream.Seek(0, SeekOrigin.Begin);
RtonParser.EncodeStream(jsonStream, output);
}

// PlainText Conversion

private static void FromPlain(Stream input, Stream output, LawnStringsFormat destFormat,
LawnStringsEncoding encodeFlags = default)
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
Utf8Bom2U16(input, output);
break;
}

}

// Convert JsonList to JsonMap

private static void JList2Map(Stream input, Stream output)
{
var jsonList = JsonSerializer.DeserializeObject<LawnStrs>(input, LawnStrs.Context);
var jsonMap = jsonList.ToMap();

JsonSerializer.SerializeObject(jsonMap, output, LawnStringsMap.Context);
}

// JsonList Conversion

private static void FromJList(Stream input, Stream output, LawnStringsFormat destFormat,
LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonMap:
JList2Map(input, output);
break;

case LawnStringsFormat.RtonList:
RtonParser.EncodeStream(input, output);
break;

case LawnStringsFormat.RtonMap:

using(ChunkedMemoryStream jsonStream = new() )
{
JList2Map(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

RtonParser.EncodeStream(jsonStream, output);
};

break;

default:
var jsonList = JsonSerializer.DeserializeObject<LawnStrs>(input, LawnStrs.Context);

jsonList.ToPlainText(output, encodeFlags);
break;
}

}

// Convert JsonMap to JsonList

private static void JMap2List(Stream input, Stream output)
{
var jsonMap = JsonSerializer.DeserializeObject<LawnStringsMap>(input, LawnStringsMap.Context);
var jsonList = jsonMap.ToList();

JsonSerializer.SerializeObject(jsonList, output, LawnStrs.Context);
}

// JsonMap Conversion

private static void FromJMap(Stream input, Stream output, LawnStringsFormat destFormat,
LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
JMap2List(input, output);
break;

case LawnStringsFormat.RtonList:

using(ChunkedMemoryStream jsonStream = new() )
{
JMap2List(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

RtonParser.EncodeStream(jsonStream, output);
};

break;

case LawnStringsFormat.RtonMap:
RtonParser.EncodeStream(input, output);
break;

default:
var jsonMap = JsonSerializer.DeserializeObject<LawnStringsMap>(input, LawnStringsMap.Context);

jsonMap.ToPlainText(output, encodeFlags);
break;
}

}

// RtonList Conversion

private static void FromRList(Stream input, Stream output, LawnStringsFormat destFormat,
LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
RtonParser.DecodeStream(input, output);
break;

case LawnStringsFormat.JsonMap:

using(ChunkedMemoryStream jsonStream = new() )
{
RtonParser.DecodeStream(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

JList2Map(jsonStream, output);
};

break;

case LawnStringsFormat.RtonMap:

using(ChunkedMemoryStream jMapStream = new() )
{

using(ChunkedMemoryStream jListStream = new() )
{
RtonParser.DecodeStream(input, jListStream);
jListStream.Seek(0, SeekOrigin.Begin);

JList2Map(jListStream, jMapStream);
};

jMapStream.Seek(0, SeekOrigin.Begin);
RtonParser.EncodeStream(jMapStream, output);
};

break;

default:

using(ChunkedMemoryStream jsonStream = new() )
{
RtonParser.DecodeStream(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

var jsonList = JsonSerializer.DeserializeObject<LawnStrs>(jsonStream, LawnStrs.Context);
jsonList.ToPlainText(output, encodeFlags);
};

break;
}

}

// RtonMap Conversion

private static void FromRMap(Stream input, Stream output, LawnStringsFormat destFormat,
LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:

using(ChunkedMemoryStream jsonStream = new() )
{
RtonParser.DecodeStream(input, jsonStream);

jsonStream.Seek(0, SeekOrigin.Begin);
JMap2List(jsonStream, output);
};

break;

case LawnStringsFormat.JsonMap:
RtonParser.DecodeStream(input, output);
break;

case LawnStringsFormat.RtonList:

using(ChunkedMemoryStream jListStream = new() )
{

using(ChunkedMemoryStream jMapStream = new() )
{
RtonParser.DecodeStream(input, jMapStream);
jMapStream.Seek(0, SeekOrigin.Begin);

JMap2List(jMapStream, jListStream);
};

jListStream.Seek(0, SeekOrigin.Begin);
RtonParser.EncodeStream(jListStream, output);
};

break;

default:

using(ChunkedMemoryStream jsonStream = new() )
{
RtonParser.DecodeStream(input, jsonStream);
jsonStream.Seek(0, SeekOrigin.Begin);

var jsonMap = JsonSerializer.DeserializeObject<LawnStringsMap>(jsonStream, LawnStringsMap.Context);
jsonMap.ToPlainText(output, encodeFlags);
};

break;
}

}

// Convert LawnStrings Stream

public static void Convert(Stream input, Stream output, LawnStringsFormat inFormat,
LawnStringsFormat outFormat, LawnStringsEncoding plainEncodeIn = default,
LawnStringsEncoding plainEncodeOut = default)
{
bool sameFormat = inFormat == outFormat;

bool isPlainText = inFormat == LawnStringsFormat.PlainText;
bool sameEncoding = plainEncodeIn == plainEncodeOut;

if(sameFormat && (!isPlainText || sameEncoding) )
{
TraceLogger.WriteWarn("Input and output formats are the same. Conversion is redundant.");
return;
}

switch(inFormat)
{
case LawnStringsFormat.JsonList:
FromJList(input, output, outFormat, plainEncodeIn);
break;

case LawnStringsFormat.JsonMap:
FromJMap(input, output, outFormat, plainEncodeIn);
break;

case LawnStringsFormat.RtonList:
FromRList(input, output, outFormat, plainEncodeIn);
break;

case LawnStringsFormat.RtonMap:
FromRMap(input, output, outFormat, plainEncodeIn);
break;

default:
FromPlain(input, output, outFormat, plainEncodeIn);
break;
}

}

// Convert LawnStrings File

public static void ConvertFile(string inputPath, LawnStringsFormat inFormat, LawnStringsFormat outFormat,
LawnStringsEncoding plainEncodeIn = default, LawnStringsEncoding plainEncodeOut = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Conversion Started");

try
{
var txtFmtIn = inFormat == default ? $"{inFormat} {plainEncodeIn}" : $"{inFormat}";
var txtFmtOut = outFormat == default ? $"{outFormat} {plainEncodeOut}" : $"{outFormat}";

TraceLogger.WriteDebug($"{inputPath}: {txtFmtIn} --> {txtFmtOut}");

TraceLogger.WriteActionStart("Opening files...");

using var inFile = FileManager.OpenRead(inputPath);

string outputPath = LawnStringsHelper.BuildPath(inputPath, "converted", outFormat);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Converting data...");
Convert(inFile, outFile, inFormat, outFormat, plainEncodeIn, plainEncodeOut);

TraceLogger.WriteActionEnd();
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Convert file");
}

TraceLogger.WriteLine("LawnStrings Conversion Finished");
}

#endregion


#region SORTER

// Sort JsonList

private static void SortJList(Stream input, Stream output)
{
var jsonList = JsonSerializer.DeserializeObject<LawnStrs>(input, LawnStrs.Context);
jsonList.Sort();

JsonSerializer.SerializeObject(jsonList, output, LawnStrs.Context);
}

// Sort JsonMap

private static void SortJMap(Stream input, Stream output)
{
var jsonMap = JsonSerializer.DeserializeObject<LawnStringsMap>(input, LawnStringsMap.Context);
jsonMap.Sort();

JsonSerializer.SerializeObject(jsonMap, output, LawnStringsMap.Context);
}

// Sort RTON

private static void SortRton(Stream input, Stream output, bool useMap)
{
using ChunkedMemoryStream unsortedStream = new();

RtonParser.DecodeStream(input, unsortedStream);
unsortedStream.Seek(0, SeekOrigin.Begin);

using ChunkedMemoryStream sortedStream = new();

if(useMap)
SortJMap(unsortedStream, sortedStream);

else
SortJList(unsortedStream, sortedStream);

sortedStream.Seek(0, SeekOrigin.Begin);
RtonParser.EncodeStream(sortedStream, output);
}

// Sort LawnStrings

public static void Sort(Stream input, Stream output, LawnStringsFormat format,
LawnStringsEncoding plainEncode = default)
{

switch(format)
{
case LawnStringsFormat.JsonList:
SortJList(input, output);
break;

case LawnStringsFormat.JsonMap:
SortJMap(input, output);
break;

case LawnStringsFormat.RtonList:
SortRton(input, output, false);
break;

case LawnStringsFormat.RtonMap:
SortRton(input, output, true);
break;

default:
LawnStringsPlain.Sort(input, output, plainEncode);
break;
}

}

// Sort File

public static void SortFile(string inputPath, LawnStringsFormat format,
LawnStringsEncoding plainEncode = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Sort Started");

try
{
TraceLogger.WriteDebug($"{inputPath}");

TraceLogger.WriteActionStart("Opening files...");
using var inFile = FileManager.OpenRead(inputPath);

string outputPath = LawnStringsHelper.BuildPath(inputPath, "sorted", format);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Sorting strings...");
Sort(inFile, outFile, format, plainEncode);

TraceLogger.WriteActionEnd();
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Sort file");
}

TraceLogger.Write("LawnStrings Sort Finished");
}

#endregion


#region COMPARER

// Compare PlainText

private static void CompareTxt(Stream a, Stream b, Stream diff, LawnStringsCompareMode compareMode,
LawnStringsEncoding encodeFlags, HashSet<string> excludeList = null)
{

var result = compareMode switch
{
LawnStringsCompareMode.Changed => LawnStringsPlain.FindChanged(a, b, encodeFlags, excludeList),
LawnStringsCompareMode.FullDiff => LawnStringsPlain.FindFullDiff(a, b, encodeFlags, excludeList),
_ => LawnStringsPlain.FindAdded(a, b, encodeFlags, excludeList),
};

LawnStringsPlain.WriteKvp(diff, result, encodeFlags);
}

// Compare JList

private static void CompareJList(Stream a, Stream b, Stream diff, LawnStringsCompareMode compareMode,
HashSet<string> excludeList = null)
{
var jListA = JsonSerializer.DeserializeObject<LawnStrs>(a, LawnStrs.Context);
var jListB = JsonSerializer.DeserializeObject<LawnStrs>(b, LawnStrs.Context);

var result = compareMode switch
{
LawnStringsCompareMode.Changed => LawnStrs.FindChanged(jListA, jListB, excludeList),
LawnStringsCompareMode.FullDiff => LawnStrs.FindFullDiff(jListA, jListB, excludeList),
_ => LawnStrs.FindAdded(jListA, jListB, excludeList),
};

JsonSerializer.SerializeObject(result, diff);
}

// Compare JsonMap

private static void CompareJMap(Stream a, Stream b, Stream diff, LawnStringsCompareMode compareMode,
HashSet<string> excludeList = null)
{
var jMapA = JsonSerializer.DeserializeObject<LawnStringsMap>(a, LawnStringsMap.Context);
var jMapB = JsonSerializer.DeserializeObject<LawnStringsMap>(b, LawnStringsMap.Context);

var result = compareMode switch
{
LawnStringsCompareMode.Changed => LawnStringsMap.FindChanged(jMapA, jMapB, excludeList),
LawnStringsCompareMode.FullDiff => LawnStringsMap.FindFullDiff(jMapA, jMapB, excludeList),
_ => LawnStringsMap.FindAdded(jMapA, jMapB, excludeList),
};

JsonSerializer.SerializeObject(result, diff);
}

// Compare RTON

private static void CompareRton(Stream a, Stream b, Stream diff, LawnStringsCompareMode compareMode,
bool useMap, HashSet<string> excludeList = null)
{
using ChunkedMemoryStream jsonA = new();
RtonParser.DecodeStream(a, jsonA);

jsonA.Seek(0, SeekOrigin.Begin);

using ChunkedMemoryStream jsonB = new();
RtonParser.DecodeStream(b, jsonB);

jsonB.Seek(0, SeekOrigin.Begin);

using ChunkedMemoryStream jDiff = new();

if(useMap)
CompareJMap(jsonA, jsonB, jDiff, compareMode, excludeList);

else
CompareJList(jsonA, jsonB, jDiff, compareMode, excludeList);

jDiff.Seek(0, SeekOrigin.Begin);
RtonParser.EncodeStream(jDiff, diff);
}

// Compare LawnStrings

public static void Compare(Stream a, Stream b, Stream diff, LawnStringsFormat format,
LawnStringsCompareMode compareMode, HashSet<string> excludeList = null,
LawnStringsEncoding encodeFlags = default)
{

switch(format)
{
case LawnStringsFormat.JsonList:
CompareJList(a, b, diff, compareMode, excludeList);
break;

case LawnStringsFormat.JsonMap:
CompareJMap(a, b, diff, compareMode, excludeList);
break;

case LawnStringsFormat.RtonList:
CompareRton(a, b, diff, compareMode, false, excludeList);
break;

case LawnStringsFormat.RtonMap:
CompareRton(a, b, diff, compareMode, true, excludeList);
break;

default:
CompareTxt(a, b, diff, compareMode, encodeFlags, excludeList);
break;
}

}

// Compare Files

public static void CompareFiles(string oldPath, string newPath, LawnStringsFormat format,
LawnStringsCompareMode compareMode, HashSet<string> excludeList = null,
LawnStringsEncoding plainEncode = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Comparisson Started");

try
{
TraceLogger.WriteDebug($"{oldPath} vs {newPath} (Mode: {compareMode})");

TraceLogger.WriteActionStart("Opening files...");

using var oldFile = FileManager.OpenRead(oldPath);
using var newFile = FileManager.OpenRead(newPath);

string outputPath = LawnStringsHelper.BuildPath(oldPath, "diff", format);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

TraceLogger.WriteActionStart("Comparing files...");
Compare(oldFile, newFile, outFile, format, compareMode, excludeList, plainEncode);

TraceLogger.WriteActionEnd();
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Compare files");
}

TraceLogger.WriteLine("LawnStrings Comparisson Finished");
}

#endregion
}

}