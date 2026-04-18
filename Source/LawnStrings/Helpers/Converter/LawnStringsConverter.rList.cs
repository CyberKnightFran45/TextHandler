using System.IO;

namespace TextHandler.LawnStrings
{
// RtonList converter

public static partial class LawnStringsConverter
{
// RtonList to txt

private static void RList2Txt(Stream input, Stream output, LawnStringsEncoding encodeFlags)
{
using var jsonStream = LawnStringsHelper.DecodeRton(input);

JList2Txt(jsonStream, output, encodeFlags);
}

// RtonList to JsonMap

private static void RList2JMap(Stream input, Stream output)
{
using var jsonStream = LawnStringsHelper.DecodeRton(input);

JList2JMap(jsonStream, output);
}

// RtonList to RtonMap

private static void RList2RMap(Stream input, Stream output)
{
using ChunkedMemoryStream jMapStream = new();
RList2JMap(input, jMapStream);

LawnStringsHelper.EncodeRton(jMapStream, output);
}

// RtonList Conversion

private static void FromRList(Stream input, Stream output,
                              LawnStringsFormat destFormat,
                              LawnStringsEncoding encodeFlags)
{

switch(destFormat)
{
case LawnStringsFormat.JsonList:
LawnStringsHelper.DecodeRton(input, output);
break;

case LawnStringsFormat.JsonMap:
RList2JMap(input, output);
break;

case LawnStringsFormat.RtonMap:
RList2RMap(input, output);
break;

default:
RList2Txt(input, output, encodeFlags);
break;
}

}

}

}