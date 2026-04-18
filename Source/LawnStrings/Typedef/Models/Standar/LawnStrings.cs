using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TextHandler.LawnStrings
{
/// <summary> Represents a List of Strings used in PvZ 2 <c>(v8.9.1 and on)</c>. </summary>

public class LawnStrings : SexyObjTable<LawnStringsJsonData>
{
/// <summary> Creates a new Instance of the <c>LawnStrings</c>. </summary>

public LawnStrings()
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

public static readonly LawnStrContext Context = new(JsonSerializer.Options);
}

// Context for serialization

[JsonSerializable(typeof(LawnStringsData) ) ]
[JsonSerializable(typeof(SexyObj<LawnStringsData>) ) ]

[JsonSerializable(typeof(LawnStringsJsonData) ) ]
[JsonSerializable(typeof(List<LawnStringsJsonData>) ) ]
[JsonSerializable(typeof(SexyObjTable<LawnStringsJsonData>) ) ]

[JsonSerializable(typeof(LawnStrings) ) ]

public partial class LawnStrContext : JsonSerializerContext
{
}

}