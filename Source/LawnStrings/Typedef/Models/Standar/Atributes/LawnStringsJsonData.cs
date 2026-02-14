namespace TextHandler.LawnStrings
{
/// <summary> Represents some Data for the LawnStrings File as JSON. </summary>

public class LawnStringsJsonData : SexyObj<LawnStringsData>
{
/// <summary> Creates a new Instance of the <c>LawnStringsJsonData</c>. </summary>

public LawnStringsJsonData()
{
ObjClass = "LawnStringsData";
ObjData = new();

Aliases.Add(ObjClass);
}

}

}