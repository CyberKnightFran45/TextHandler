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

public static NativeMemoryOwner<char> CleanLine(ReadOnlySpan<char> src)
{
NativeMemoryOwner<char> cleanStr = new(src.Length + 1);

int j = 0;

for(int i = 0; i < src.Length; i++)
{

if(src[i] == '\\' && i + 1 < src.Length && src[i + 1] == 'n')
{
cleanStr[j++] = '\n';
i++;
}

else if(src[i] == '\r')
continue;

else
cleanStr[j++] = src[i];

}

if(j == 0 || cleanStr[j - 1] != '\n')
cleanStr[j++] = '\n';

cleanStr.Realloc(j);

return cleanStr;
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

public static int AlphanumCompare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
{
int ix = 0, iy = 0;

while(ix < x.Length && iy < y.Length)
{
if (char.IsDigit(x[ix]) && char.IsDigit(y[iy]) )
{
long numX = 0, numY = 0;

while(ix < x.Length && char.IsDigit(x[ix]) )
numX = numX * 10 + (x[ix++] - '0');

while(iy < y.Length && char.IsDigit(y[iy]) )
numY = numY * 10 + (y[iy++] - '0');

int numCompare = numX.CompareTo(numY);

if(numCompare != 0)
return numCompare;

}

else
{
int cmp = x[ix].CompareTo(y[iy]);

if(cmp != 0) return cmp;

ix++; iy++;
}

}

return x.Length.CompareTo(y.Length);
}

}

}