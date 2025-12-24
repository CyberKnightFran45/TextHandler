namespace TextHandler.LawnStrings
{
/// <summary> Represents some Data for the LawnStrings File as Json. </summary>

public class LawnStringsJsonMap : SexyObj<LawnStringsMapData>
{
/// <summary> Creates a new Instance of the <c>LawnStringsJsonData</c>. </summary>

public LawnStringsJsonMap()
{
ObjClass = "LawnStringsData";
ObjData = new();

Aliases.Add(ObjClass);
}

}

}