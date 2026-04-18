using System.Collections.Generic;
using System.IO;

namespace TextHandler.LawnStrings
{
/// <summary> Updates the content of a LawnStrings file by adding new strings </summary>

public static partial class LawnStringsUpdater
{
// Update LawnStrings

public static void Update(Stream oldStream, Stream newStream, Stream patch, LawnStringsFormat format,
                          HashSet<string> excludeList = null,
                          LawnStringsEncoding plainEncode = default)
{
excludeList ??= new();

switch(format)
{
case LawnStringsFormat.JsonList:
UpdateJList(oldStream, newStream, patch, excludeList);
break;

case LawnStringsFormat.JsonMap:
UpdateJMap(oldStream, newStream, patch, excludeList);
break;

case LawnStringsFormat.RtonList:
UpdateRton(oldStream, newStream, patch, false, excludeList);
break;

case LawnStringsFormat.RtonMap:
UpdateRton(oldStream, newStream, patch, true, excludeList);
break;

default:
UpdateTxt(oldStream, newStream, patch, plainEncode, excludeList);
break;
}

}

}

}