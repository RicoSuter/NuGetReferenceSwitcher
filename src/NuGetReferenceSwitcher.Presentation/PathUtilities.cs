using System;
using System.IO;

namespace NuGetReferenceSwitcher.Presentation
{
    public static class PathUtilities
    {
        public static string MakeRelative(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }

        public static string MakeAbsolute(string filePath, string referencePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                var s = Path.Combine(Path.GetDirectoryName(referencePath), filePath);
                return  Path.GetFullPath(s);
            }
            return filePath;
        }
    }
}
