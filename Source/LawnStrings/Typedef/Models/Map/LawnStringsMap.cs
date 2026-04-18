using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TextHandler.LawnStrings
{
/// <summary> Represents a Dictionary of Strings used in PvZ 2 <c>(v6.8.1 - 8.8.1)</c>. </summary>

public class LawnStringsMap : SexyObjTable<LawnStringsJsonMap>
{
/// <summary> Creates a new Instance of the <c>LawnStrings</c>. </summary>

public LawnStringsMap()
{
Objects ??= new();

Objects.Add( new() );

Objects[0].ObjData = new()
{
LocStringValues = new()
};

}

// Check for null Fields

public override void CheckObjs()
{
Objects ??= new();

if(Objects.Count == 0)
Objects.Add( new() );

Objects[0] ??= new();
Objects[0].ObjData ??= new();

Objects[0].ObjData.LocStringValues ??= new();
}

public static readonly LawnStrMapContext Context = new(JsonSerializer.Options);
}

// Context for serialization

[JsonSerializable(typeof(LawnStringsMapData) ) ]
[JsonSerializable(typeof(SexyObj<LawnStringsMapData>) ) ]

[JsonSerializable(typeof(LawnStringsJsonMap) ) ]
[JsonSerializable(typeof(List<LawnStringsJsonMap>) ) ]
[JsonSerializable(typeof(SexyObjTable<LawnStringsJsonMap>) ) ]

[JsonSerializable(typeof(LawnStringsMap) ) ]

public partial class LawnStrMapContext : JsonSerializerContext
{
}

}