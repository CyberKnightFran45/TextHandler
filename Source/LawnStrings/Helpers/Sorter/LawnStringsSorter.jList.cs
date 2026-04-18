using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TextHandler.LawnStrings
{
// JsonList sorter

public static partial class LawnStringsSorter
{
// Bubble sort (Quick variation)

private static void QuickSort(List<string> list, int left, int right, CompareInfo comparer)
{

if(left >= right)
return;

int pivotIndex = Partition(list, left, right, comparer);

QuickSort(list, left, pivotIndex - 1, comparer);
QuickSort(list, pivotIndex + 1, right, comparer);
}

// Segment list into two Parts

private static int Partition(List<string> list, int left, int right, CompareInfo comparer)
{
string pivotKey = list[right * 2];
int i = left - 1;

var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols 
                                        | CompareOptions.IgnoreNonSpace
                                        | CompareOptions.StringSort;

for(int j = left; j < right; j++)
{
string currentKey = list[j * 2];
int cmp = comparer.Compare(currentKey, pivotKey, options);

if(cmp <= 0)
{
i++;

SwapPairs(list, i, j);
}

}

SwapPairs(list, i + 1, right);

return i + 1;
}

// Swap pairs

private static void SwapPairs(List<string> list, int indexA, int indexB)
{
int iA = indexA * 2;
int iB = indexB * 2;

string tempKey = list[iA];
string tempVal = list[iA + 1];

list[iA] = list[iB];
list[iA + 1] = list[iB + 1];

list[iB] = tempKey;
list[iB + 1] = tempVal;
}

// Sort LawnStrings

private static void Sort(LawnStrs jsonList)
{
var list = jsonList.Objects[0].ObjData.LocStringValues;
var comparer = CultureInfo.InvariantCulture.CompareInfo;

QuickSort(list, 0, list.Count / 2 - 1, comparer);
}

// Sort JsonList

private static void SortJList(Stream input, Stream output)
{
var jsonList = LawnStringsHelper.LoadJList(input);

TraceLogger.WriteActionStart("Sorting strings...");
Sort(jsonList);

TraceLogger.WriteActionEnd();

LawnStringsHelper.SaveJList(output, jsonList);
}

}

}