using System.IO;
using System.Text.Json.Serialization;

namespace TextHandler.LawnStrings
{
/// <summary> Represents a Server Entry for the LawnStrings File. </summary>

public class LawnStringsRes
{
/** <summary> Gets or Sets the File Entry itself. </summary>
<returns> The LawnStrings File. </returns> */

public LawnStringsResEntry File{ get; set; }

/// <summary> Creates a new Instance of the <c>LawnStringsRes</c>. </summary>

public LawnStringsRes()
{
File = new();
}

public static LawnStringsRes Init(Stream source)
{
LawnStringsRes res = new();

var entry = LawnStringsResEntry.Create(source);
res.File = entry;

return res;
}

public static readonly JsonSerializerContext Context = LawnStrResContext.Default;
}

// Context for serialization

[JsonSerializable(typeof(LawnStringsResEntry) ) ]

[JsonSerializable(typeof(LawnStringsRes) ) ]

public partial class LawnStrResContext : JsonSerializerContext
{
}

}