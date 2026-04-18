using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TextHandler.LawnStrings
{
/** <summary> Grants access to the LawnStrings Server from PvZ 2 China
              <c>(used in v3.3.5 and on)</c> </summary> */

public static class LawnStringsServer
{
// Text file urls

private static readonly Dictionary<LawnStringsServerType, string> TextFileUrls = new()
{

{ LawnStringsServerType.Release, "https://pvz2cdn.ditwan.cn/ad/res_release/pvz2_l.txt" },
{ LawnStringsServerType.Shipping, "https://pvz2cdn.ditwan.cn/ad/res_shipping/pvz2_l.txt" }

};

// Hash file urls

private static readonly Dictionary<LawnStringsServerType, string> HashFileUrls = new()
{

{ LawnStringsServerType.Release, "https://pvz2cdn.ditwan.cn/ad/res_release/file_list.txt" },
{ LawnStringsServerType.Shipping, "https://pvz2cdn.ditwan.cn/ad/res_shipping/file_list.txt" }

};

// Get name from url

private static string ResolveName(string url)
{
Uri resUri = new(url);

return Path.GetFileName(resUri.LocalPath);
}

// Download Internal (Memory)

private static async Task DownloadAsync(string url, ChunkedMemoryStream target)
{
string fileName = ResolveName(url);

TraceLogger.WriteLine($"• Download res: {fileName} (temp)");
TraceLogger.WriteLine();

using Stream compiledText = await UrlFetcher.GetResponseStreamAsync(url);
CompiledText.DecodeStream(compiledText, target);

target.Seek(0, SeekOrigin.Begin);
}

// Download Internal (Streaming)

private static async Task DownloadAsync(string url, FileStream target)
{
string fileName = ResolveName(url);

TraceLogger.WriteLine($"• Download res: {fileName}");
TraceLogger.WriteLine();

using Stream compiledText = await UrlFetcher.GetResponseStreamAsync(url);
CompiledText.DecodeStream(compiledText, target);

target.Seek(0, SeekOrigin.Begin);
}

// Download Internal (File)

private static async Task DownloadAsync(string url, string outputDir)
{
string fileName = ResolveName(url);

string outputPath = Path.Combine(outputDir, fileName);
using var plainRes = FileManager.OpenWrite(outputPath);

await DownloadAsync(url, plainRes);
}

// Download Res

private static async Task DownloadResAsync(string outputDir, LawnStringsServerType serverType)
{
await DownloadAsync(TextFileUrls[serverType], outputDir);
}

// Download ResInfo

private static async Task DownloadInfoAsync(string outputDir, LawnStringsServerType serverType)
{
await DownloadAsync(HashFileUrls[serverType], outputDir);
}

// Download All Files (Async)

public static async Task DownloadFileAsync(string baseDir, LawnStringsResType res,
                                           LawnStringsServerType serverType)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Server: Download Started");

try
{
string outputDir = Path.Combine(baseDir, $"{serverType}");
TraceLogger.WriteDebug($"{res} file - {serverType} server → {outputDir}");

switch(res)
{
case LawnStringsResType.Md5:
await DownloadInfoAsync(outputDir, serverType);
break;

case LawnStringsResType.All:
await DownloadInfoAsync(outputDir, serverType);

await DownloadResAsync(outputDir, serverType);
break;

default:
await DownloadResAsync(outputDir, serverType);
break;
}

}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Download file");
}

TraceLogger.WriteLine("LawnStrings Download Finished");
}

// Download All Files

public static void DownloadFile(string baseDir, LawnStringsResType res,
                                LawnStringsServerType serverType)
{
DownloadFileAsync(baseDir, res, serverType).GetAwaiter().GetResult();
}								

// Get new Strings added in Server, by comparing Local file

public static async Task GetUpdateAsync(Stream target, Stream diff, LawnStringsServerType serverType,
                                        HashSet<string> excludeList = null)
{
using ChunkedMemoryStream res = new();
await DownloadAsync(TextFileUrls[serverType], res);

LawnStringsComparer.Compare(target, res, diff, default, default, excludeList);
}

// Get new Strings (Async)

public static async Task GetUpdateAsync(string inputPath, LawnStringsServerType serverType,
                                        HashSet<string> excludeList = null)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Server: Update Check Started");

try
{
string outputPath = LawnStringsHelper.BuildPath(inputPath, "newContent", default);

TraceLogger.WriteDebug($"Local: {inputPath} vs Server: {serverType}");

TraceLogger.WriteActionStart("Opening files...");

using var inFile = FileManager.OpenRead(inputPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

await GetUpdateAsync(inFile, outFile, serverType, excludeList);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Check update");
}

TraceLogger.WriteLine("Update Check Finished");
}

// Get new Strings

public static void GetUpdate(string inputPath, LawnStringsServerType serverType,
                             HashSet<string> excludeList = null)
{
GetUpdateAsync(inputPath, serverType, excludeList).GetAwaiter().GetResult();
}

// Update local file by comparing server content

public static async Task UpdateAsync(Stream oldStream, Stream patch, LawnStringsServerType serverType,
                                     HashSet<string> excludeList = null)
{
using ChunkedMemoryStream newStream = new();
await DownloadAsync(TextFileUrls[serverType], newStream);

LawnStringsUpdater.Update(oldStream, newStream, patch, default, excludeList);
}

// Update local LawnStrings using server (Async)

public static async Task UpdateAsync(string oldPath, LawnStringsServerType serverType,
                                     HashSet<string> excludeList = null)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Server: Update Started");

try
{
string outputPath = LawnStringsHelper.BuildPath(oldPath, "updated", default);

TraceLogger.WriteDebug($"Local: {oldPath} vs Server: {serverType}");

TraceLogger.WriteActionStart("Opening files...");

using var oldFile = FileManager.OpenRead(oldPath);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

await UpdateAsync(oldFile, outFile, serverType, excludeList);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Update file");
}

TraceLogger.WriteLine("LawnStrings Update Finished");
}

// Update local LawnStrings using server

public static void Update(string oldPath, LawnStringsServerType serverType,
                          HashSet<string> excludeList = null)
{
UpdateAsync(oldPath, serverType, excludeList).GetAwaiter().GetResult();
}

}

}