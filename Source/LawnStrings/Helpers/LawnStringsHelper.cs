using System;
using System.IO;
using System.Text;

namespace TextHandler.LawnStrings
{
/// <summary> Performs some Helpful Tasks used on Handling LawnStrings. </summary>

public static class LawnStringsHelper
{
// Get LawnStrings Encoding

public static EncodingType GetEncoding(LawnStringsEncoding flags)
{

return flags switch
{
LawnStringsEncoding.UTF16 => EncodingType.UTF16,
_ => EncodingType.UTF8
};

}

// Clean line

public static int GetCleanLength(ReadOnlySpan<char> src)
{
int len = 0;

for(int i = 0; i < src.Length; i++)
{

if(src[i] == '\\' && i + 1 < src.Length && src[i + 1] == 'n')
{
len++;
i++;
}

else if(src[i] == '\r')
continue;
        
else
len++;
        
}

if(len == 0 || src[^1] != '\n')
len++;

return len;
}

// Get LawnStrings Ext

public static string GetExtension(LawnStringsFormat sourceFormat)
{

return sourceFormat switch
{
LawnStringsFormat.JsonList or LawnStringsFormat.JsonMap => ".json",
LawnStringsFormat.RtonList or LawnStringsFormat.RtonMap => ".rton",
_ => ".txt",
};

}

// Get new Path for LawnStrings

public static string BuildPath(string sourcePath, string suffix, LawnStringsFormat destFormat)
{
string baseDir = Path.GetDirectoryName(sourcePath);
string fileName = Path.GetFileNameWithoutExtension(sourcePath);

string fileExt = GetExtension(destFormat);
string outputPath = Path.Combine(baseDir, $"{fileName}_{suffix}{fileExt}");

PathHelper.CheckDuplicatedPath(ref outputPath);

return outputPath;
}

}

}