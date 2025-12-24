using System.Collections.Generic;

namespace TextHandler.LawnStrings
{
/// <summary> Represents some Data for the LawnStrings File. </summary>

public class LawnStringsMapData
{
/** <summary> Gets or Sets the Strings Located by Language. </summary>
<returns> The Strings. </returns> */

public Dictionary<string, string> LocStringValues{ get; set; }

/// <summary> Creates a new Instance of the <c>LawnStringsData</c>. </summary>

public LawnStringsMapData()
{
}

}

}