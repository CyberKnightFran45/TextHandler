using System.Collections.Generic;
using System;

namespace TextHandler.LawnStrings
{
/// <summary> Manages LawnStrings files, such as Conversion, Comparing and Sorting. </summary>

public static class LawnStringsMgr
{
#region ==============  CONVERTER  ==============

// Convert LawnStrings File

public static void ConvertFile(string inputPath,
                               LawnStringsFormat inFormat,
							   LawnStringsFormat outFormat,
                               LawnStringsEncoding plainEncodeIn = default,
							   LawnStringsEncoding plainEncodeOut = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Conversion Started");

try
{
var txtFmtIn = inFormat == default ? $"{inFormat} {plainEncodeIn}" : $"{inFormat}";
var txtFmtOut = outFormat == default ? $"{outFormat} {plainEncodeOut}" : $"{outFormat}";

string outputPath = LawnStringsHelper.BuildPath(inputPath, "converted", outFormat);

TraceLogger.WriteDebug($"{inputPath}: {txtFmtIn} --> {txtFmtOut}");

TraceLogger.WriteActionStart("Opening files...");

using var inFile = FileManager.OpenRead(inputPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

LawnStringsConverter.Convert(inFile, outFile, inFormat, outFormat, plainEncodeIn, plainEncodeOut);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Convert file");
}

TraceLogger.WriteLine("LawnStrings Conversion Finished");
}

#endregion


#region ==============  SORTER  ==============

// Sort File

public static void SortFile(string inputPath, LawnStringsFormat format,
                            LawnStringsEncoding plainEncode = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Sort Started");

try
{
string outputPath = LawnStringsHelper.BuildPath(inputPath, "sorted", format);

TraceLogger.WriteDebug($"{inputPath}");

TraceLogger.WriteActionStart("Opening files...");

using var inFile = FileManager.OpenRead(inputPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

LawnStringsSorter.Sort(inFile, outFile, format, plainEncode);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Sort file");
}

TraceLogger.Write("LawnStrings Sort Finished");
}

#endregion


#region ==============  COMPARER  ==============

// Compare Files

public static void CompareFiles(string oldPath, string newPath, LawnStringsFormat format,
                                LawnStringsCompareMode compareMode,
								HashSet<string> excludeList = null,
                                LawnStringsEncoding plainEncode = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Comparisson Started");

try
{
string outputPath = LawnStringsHelper.BuildPath(oldPath, "diff", format);

TraceLogger.WriteDebug($"{oldPath} vs {newPath} (Mode: {compareMode})");

TraceLogger.WriteActionStart("Opening files...");

using var oldFile = FileManager.OpenRead(oldPath);
using var newFile = FileManager.OpenRead(newPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

LawnStringsComparer.Compare(oldFile, newFile, outFile, format, compareMode, excludeList, plainEncode);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Compare files");
}

TraceLogger.WriteLine("LawnStrings Comparisson Finished");
}

#endregion


#region ==============  UPDATER  ==============

// Update LawnStrings

public static void UpdateFile(string oldPath, string newPath, LawnStringsFormat format,
						      HashSet<string> excludeList = null,
                              LawnStringsEncoding plainEncode = default)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Update Started");

try
{
string outputPath = LawnStringsHelper.BuildPath(oldPath, "updated", format);

TraceLogger.WriteDebug($"{oldPath} vs {newPath}");

TraceLogger.WriteActionStart("Opening files...");

using var oldFile = FileManager.OpenRead(oldPath);
using var newFile = FileManager.OpenRead(newPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

LawnStringsUpdater.Update(oldFile, newFile, outFile, format, excludeList, plainEncode);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Update file");
}

TraceLogger.WriteLine("LawnStrings Update Finished");
}

#endregion
}

}