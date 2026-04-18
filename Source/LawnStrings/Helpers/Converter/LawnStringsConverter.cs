using System.IO;

namespace TextHandler.LawnStrings
{
/// <summary> Convert LawnStrings into different formats </summary>

public static partial class LawnStringsConverter
{
// Convert LawnStrings Stream

public static void Convert(Stream input, Stream output,
                           LawnStringsFormat inFormat,
						   LawnStringsFormat outFormat,
                           LawnStringsEncoding plainEncodeIn = default,
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

}

}