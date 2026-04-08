using BlossomLib.Modules.Parsers;
using SexyCompressors.PopCapZLib;
using SexyCryptor;
using System;
using System.IO;

namespace TextHandler
{
/// <summary> Handles PopCap Compiled Text </summary>

public static class CompiledText
{
// Encode Stream

public static void EncodeStream(Stream input, Stream output)
{
TraceLogger.WriteLine("Step #1 - Compress Smf:");
TraceLogger.WriteLine();

int smfChunks = MemoryManager.GetBufferSize(input);
using ChunkedMemoryStream compressedStream = new(smfChunks);

SmfCompressor.CompressStream(input, compressedStream, default);
compressedStream.Seek(0, SeekOrigin.Begin);

TraceLogger.WriteLine("Step #2 - Encrypt CRton:");
TraceLogger.WriteLine();

int cryptoChunks = MemoryManager.GetBufferSize(compressedStream);
using ChunkedMemoryStream encryptedStream = new(cryptoChunks);

CRton.EncryptStream(compressedStream, encryptedStream);
encryptedStream.Seek(0, SeekOrigin.Begin);

TraceLogger.WriteLine("Step #3 - Encode Base64:");
TraceLogger.WriteLine();

TraceLogger.WriteActionStart("Encoding data...");
Base64.EncodeStream(encryptedStream, output, true);

TraceLogger.WriteActionEnd();
}	

/** <summary> Encodes Plaintext as a PopCapCompiledText. </summary>

<param name = "inputPath"> The Path where the File to Encode is Located. </param>
<param name = "outputPath"> The Location where the Encoded File will be Saved. </param> */

public static void EncodeFile(string inputPath, string outputPath)
{
TraceLogger.Init();
TraceLogger.WriteLine("CompiledText Encoding Started");

try
{
TraceLogger.WriteDebug($"{inputPath} → {outputPath}");

TraceLogger.WriteActionStart("Opening files...");

using FileStream inFile = FileManager.OpenRead(inputPath);
using FileStream outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

TraceLogger.WriteLine("• CompiledText Encode Process");
TraceLogger.WriteLine();

EncodeStream(inFile, outFile);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Encode file");
}

TraceLogger.WriteLine("CompiledText Encoding Finished");

var outSize = FileManager.GetFileSize(outputPath);
TraceLogger.WriteInfo($"Output Size: {SizeT.FormatSize(outSize)}", false);
}

// Decode Stream

public static void DecodeStream(Stream input, Stream output)
{
TraceLogger.WriteLine("Step #1 - Decode Base64:");
TraceLogger.WriteLine();

int cryptoChunks = MemoryManager.GetBufferSize(input);
using ChunkedMemoryStream encryptedStream = new(cryptoChunks);

TraceLogger.WriteActionStart("Decoding data...");

Base64.DecodeStream(input, encryptedStream, true);
encryptedStream.Seek(0, SeekOrigin.Begin);

TraceLogger.WriteActionEnd();

TraceLogger.WriteLine("Step #2 - Decrypt CRton:");
TraceLogger.WriteLine();

int smfChunks = MemoryManager.GetBufferSize(encryptedStream);
using ChunkedMemoryStream compressedStream = new(smfChunks);

CRton.DecryptStream(encryptedStream, compressedStream);
compressedStream.Seek(0, SeekOrigin.Begin);

TraceLogger.WriteLine("Step #3 - Decompress Smf:");
TraceLogger.WriteLine();

SmfCompressor.DecompressStream(compressedStream, output);
}

/** <summary> Decodes a PopCapCompiledText as Plaintext. </summary>

<param name = "inputPath"> The Path where the File to Decode is Located. </param>
<param name = "outputPath"> The Location where the Decode File will be Saved. </param> */

public static void DecodeFile(string inputPath, string outputPath)
{
TraceLogger.Init();
TraceLogger.WriteLine("CompiledText Decoding Started");

try
{
TraceLogger.WriteDebug($"{inputPath} → {outputPath}");

TraceLogger.WriteActionStart("Opening files...");

using FileStream inFile = FileManager.OpenRead(inputPath);
using FileStream outFile = FileManager.OpenWrite(outputPath);

TraceLogger.WriteActionEnd();

TraceLogger.WriteLine("• CompiledText Decode Process");
TraceLogger.WriteLine();

DecodeStream(inFile, outFile);
}

catch(Exception error)
{
TraceLogger.WriteError(error, "Failed to Decode file");
}

TraceLogger.WriteLine("CompiledText Decoding Finished");

var outSize = FileManager.GetFileSize(outputPath);
TraceLogger.WriteInfo($"Output Size: {SizeT.FormatSize(outSize)}", false);
}

}

}
