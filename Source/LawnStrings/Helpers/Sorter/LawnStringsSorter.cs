using System.IO;

namespace TextHandler.LawnStrings
{
/// <summary> Sorts LawnStrings from A to Z </summary>

public static partial class LawnStringsSorter
{
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
SortPlain(input, output, plainEncode);
break;
}

}

}

}