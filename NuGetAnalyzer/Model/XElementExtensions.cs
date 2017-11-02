using NuGet;
using System;
using System.Xml.Linq;

namespace NuGetAnalyzer
{
    public static class XMLExtensions
    {
        public static string GetTarget(this XElement element)
        {
            return element.Attribute("Target").Value;
        }

        public static string GetSource(this XElement element)
        {
            return element.Attribute("Source").Value;
        }

        public static bool Contains(this string s, Tuple<string, SemanticVersion> sourcePackage)
        {
            return s.Contains(sourcePackage.Item1 + " " + sourcePackage.Item2);
        }

    }
}