using System.IO;

namespace TextHandler.LawnStrings
{
// RtonMap converter

public static partial class LawnStringsConverter
{
// RtonMap to txt

private static void RMap2Txt(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
using var jsonStream = LawnStringsHelper.DecodeRton(input);

JMap2Txt(jsonStream, output, encodeFlags);
}

// RtonMap to JsonList

private static void RMap2JList(Stream input, Stream output)
{
using var jsonStream = LawnStringsHelper.DecodeRton(input);

JMap2JList(jsonStream, output);
}

// RtonMap to RtonList

private static void RMap2RList(Stream input, Stream output)
{
using ChunkedMemoryStream jListStream = new();
RMap2JList(input, jListStream);

LawnStringsHelper.EncodeRton(jListStream, output);
}

// RtonMap Conversion

private static void FromRMap(Stream input, Stream output,
                             LawnStringsFormat destFormat,
                             LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
RMap2JList(input, output);
break;

case LawnStringsFormat.JsonMap:
LawnStringsHelper.DecodeRton(input, output);
break;

case LawnStringsFormat.RtonList:
RMap2RList(input, output);
break;

default:
RMap2Txt(input, output, encodeFlags);
break;
}

}

}

}