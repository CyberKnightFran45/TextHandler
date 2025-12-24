using NetworkMgr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TextHandler.LawnStrings
{
/** <summary> Grants access to the LawnStrings Server from PvZ 2 China
              <b>(Used in v3.3.5 and on)</b> </summary> */

public static class LawnStringsServer
{
// Files Containing Strings

private static readonly Dictionary<LawnStringsServerType, string> SOURCE = new()
{

{ LawnStringsServerType.Release, "https://pvz2cdn.ditwan.cn/ad/res_release/pvz2_l.txt" },
{ LawnStringsServerType.Shipping, "https://pvz2cdn.ditwan.cn/ad/res_shipping/pvz2_l.txt" }

};

// File Containing Hash

private static readonly Dictionary<LawnStringsServerType, string> HASH = new()
{

{ LawnStringsServerType.Release, "https://pvz2cdn.ditwan.cn/ad/res_release/file_list.txt" },
{ LawnStringsServerType.Shipping, "https://pvz2cdn.ditwan.cn/ad/res_shipping/file_list.txt" }

};

// Download Internal (Memory)

private static async Task DownloadAsync(string url, ChunkedMemoryStream target)
{
string fileName = Path.GetFileName(new Uri(url).LocalPath);

TraceLogger.WriteLine($"• Download started: {fileName} (temp)");
TraceLogger.WriteLine();

using Stream compiledText = await UrlFetcher.GetResponseStreamAsync(url);
CompiledText.DecodeStream(compiledText, target);

target.Seek(0, SeekOrigin.Begin);
}

// Download Internal (Streaming)

private static async Task DownloadAsync(string url, FileStream target)
{
string fileName = Path.GetFileName(new Uri(url).LocalPath);

TraceLogger.WriteLine($"• Download started: {fileName}");
TraceLogger.WriteLine();

using Stream compiledText = await UrlFetcher.GetResponseStreamAsync(url);
CompiledText.DecodeStream(compiledText, target);

target.Seek(0, SeekOrigin.Begin);
}

// Download Internal (File)

private static async Task DownloadAsync(string url, string outputDir)
{
string fileName = Path.GetFileName(new Uri(url).LocalPath);

string outputPath = Path.Combine(outputDir, fileName);
using var plainRes = FileManager.OpenWrite(outputPath);

await DownloadAsync(url, plainRes);
}

// Download Res

private static async Task DownloadResAsync(string outputDir, LawnStringsServerType serverType)
{
await DownloadAsync(SOURCE[serverType], outputDir);
}

// Download ResInfo

private static async Task DownloadInfoAsync(string outputDir, LawnStringsServerType serverType)
{
await DownloadAsync(HASH[serverType], outputDir);
}

// Download All Files

public static async Task DownloadFileAsync(string baseDir, LawnStringsResType res,
LawnStringsServerType serverType)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings Download Started");

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

// Get new Strings added in Server, by comparing Local file

public static async Task GetUpdate(Stream target, Stream diff, LawnStringsServerType serverType,
HashSet<string> excludeList = null)
{
using ChunkedMemoryStream res = new();
await DownloadAsync(SOURCE[serverType], res);

var newStrs = LawnStringsPlain.FindAdded(target, res, default, excludeList);
LawnStringsPlain.WriteKvp(diff, newStrs, default);
}

// Get new Strings

public static async Task GetUpdate(string inputPath, LawnStringsServerType serverType,
HashSet<string> excludeList = null)
{
TraceLogger.Init();
TraceLogger.WriteLine("LawnStrings: Update Check Started");

try
{
TraceLogger.WriteDebug($"Local: {inputPath} vs Server: {serverType}");

TraceLogger.WriteActionStart("Opening files...");

using var inFile = FileManager.OpenRead(inputPath);

string outputPath = LawnStringsHelper.BuildPath(inputPath, "update", default);
using var outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

await GetUpdate(inFile, outFile, serverType, excludeList);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Check update");
}

TraceLogger.WriteLine("Update Check Finished");
}

}

}