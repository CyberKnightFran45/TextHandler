using System.IO;
using BlossomLib.Modules.Security;

namespace TextHandler.LawnStrings
{
/// <summary> Represents a File in the LawnStrings Server (used in PvZ 2 China). </summary>

public class LawnStringsResEntry
{
/// <summary> Res Name </summary>

public string Name{ get; set; } = "pvz2_l.txt";

/// <summary> MD5 hash from LawnStrings </summary>

public string Hash{ get; set; } 

/// <summary> Creates a new <c>LawnStringsResEntry</c>. </summary>

public LawnStringsResEntry()
{
Hash = "<md5>";
}

/// <summary> Creates a new <c>LawnStringsResEntry</c> from given source </summary>

public static LawnStringsResEntry Create(Stream source)
{
LawnStringsResEntry entry = new();

using var hOwner = GenericDigest.GetString(source, "MD5");
entry.Hash = new(hOwner.AsSpan() );

return entry;
}

}

}