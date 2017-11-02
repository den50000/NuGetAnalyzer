using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NuGetAnalyzer.Model
{
    public class DGMLFile
    {
        private readonly string _filePath;
        private XmlDocument _doc;

        public DGMLFile(string filePath)
        {
            _filePath = filePath;
            _doc = new XmlDocument();
            _doc.Load(_filePath);
        }

        public IEnumerable<IGrouping<string, Tuple<string, Version>>> GetNonUnique()
        {
            // read the XmlDocument.
            List<string> packagesIds = ReadPackages();

            // exclude unique 
            // var nonUnique = packagesIds.GroupBy(x => x).Where(g => g.Count() > 1).SelectMany(y => y).ToList();

            // Split Package Name from version 
            List<Tuple<string, Version>> packages = new List<Tuple<string, Version>>();
            foreach (var package in packagesIds)
            {
                string[] packageSplitted = package.Split(' ');
                // get version 
                string version = packageSplitted.Last();
                Version parseResult;
                if (Version.TryParse(version, out parseResult))
                {
                    packages.Add(new Tuple<string, Version>(packageSplitted.First(), new Version(version)));
                }
                else
                {
                    ReportError("Can't parse package: " + package);
                }
            }

            return packages.GroupBy(x => x.Item1);
        }

        // Event to report processing progress
        public event Action<string> ReportError;

        public List<Tuple<string, SemanticVersion>> GetPackages()
        {
            // read the XmlDocument.
            List<string> packagesIds = ReadPackages();

            // Split Package Name from version 
            List<Tuple<string, SemanticVersion>> packages = new List<Tuple<string, SemanticVersion>>();
            foreach (var package in packagesIds)
            {
                string[] packageSplitted = package.Split(' ');
                // get version 
                string version = packageSplitted.Last();
                SemanticVersion parseResult;
                if (SemanticVersion.TryParse(version, out parseResult))
                {
                    packages.Add(new Tuple<string, SemanticVersion>(packageSplitted.First(), new SemanticVersion(version)));
                }
                else
                {
                    ReportError(package + " can't extract package from DGML file ");
                }
            }
            return packages;
        }


        private List<string> ReadPackages()
        {

            //  XmlNode node = doc.DocumentElement.SelectSingleNode("/Nodes");

            List<string> packagesIds = new List<string>();
            _doc.Load(_filePath);
            foreach (XmlNode somenode in _doc.DocumentElement.ChildNodes.Item(0).ChildNodes) // go through all nodes
            {
                if (somenode.Attributes["Category"].InnerText.Contains("Package"))
                {
                    packagesIds.Add(somenode.Attributes["Id"]?.InnerText);
                }
            }

            return packagesIds;
        }

        private IEnumerable<XmlNode> GetAllLinks()
        {
            return _doc.DocumentElement.ChildNodes.Item(1).ChildNodes.Cast<XmlNode>();
        }

        IEnumerable<XmlNode> FindParents(Tuple<string, SemanticVersion> sourcePackage)
        {
            return GetAllLinks().Where(x => x.Attributes["Target"].InnerText.Contains(sourcePackage));
        }


        IEnumerable<XmlNode> FindParents(XmlNode source)
        {
            return GetAllLinks().Where(x => x.Attributes["Target"].InnerText.Contains(source.Attributes["Source"].InnerText));
        }



        private string FormatOutput(XmlNode nodeToFormat)
        {
            if (nodeToFormat.Attributes["Category"].InnerText.Contains("Installed Package"))
            {
                return String.Format("Project ------------> " + nodeToFormat.Attributes["Source"]?.InnerText);
            }
            else
            {
                return String.Format(" --> " + nodeToFormat.Attributes["Source"]?.InnerText);
            }
        }

        public List<string> BuildDependencyChains(Tuple<string, SemanticVersion> sourcePackage)
        {
            List<string> chain = new List<string>();
            // Start recursively iterating through tree structure starting from a provided root
            var queue = new Queue<XmlNode>();
            //queue.PushRange(FindParents(sourcePackage));
            foreach (XmlNode xmlNode in FindParents(sourcePackage))
            {
                queue.Enqueue(xmlNode);

                chain.Add(FormatOutput(xmlNode)); // record parents        

                while (queue.Count > 0)
                {
                    XmlNode currentNode = queue.Dequeue(); // take from stack

                    foreach (XmlNode xmlNode2 in FindParents(currentNode))
                    {
                        queue.Enqueue(xmlNode2);
                        chain.Add(FormatOutput(xmlNode2)); // record parents      
                    }
                }
            }
            return chain;
        }

        public bool IsWithoutDependencies(Tuple<string, SemanticVersion> package)
        {
            return !GetAllLinks().Any(x => x.Attributes["Source"].InnerText.Contains(package));
        }

        public void Save(XDocument xDocument)
        {
            xDocument.Save(_filePath);
        }


    }
}
