using System.IO;
using BlossomLib.Modules.Security;

namespace TextHandler.LawnStrings
{
/// <summary> Represents a File in the LawnStrings Server (used in PvZ 2 China). </summary>

public class LawnStringsResEntry
{
/** <summary> Gets or Sets the File Name. </summary>
<returns> The Res Name. </returns> */

public string Name{ get; set; } = "pvz2_l.txt";

/** <summary> Gets or Sets a MD5 Hash from the File. </summary>
<returns> The Res Hash. </returns> */

public string Hash{ get; set; } 

/// <summary> Creates a new Instance of the <c>LawnStringsResEntry</c>. </summary>

public LawnStringsResEntry()
{
Hash = "<md5>";
}

/// <summary> Creates a new Instance of the <c>LawnStringsResEntry</c>. </summary>

public static LawnStringsResEntry Create(Stream source)
{
LawnStringsResEntry entry = new();

using var hOwner = GenericDigest.GetString(source, "MD5");
string md5 = new(hOwner.AsSpan() );

entry.Hash = md5;

return entry;
}

}

}