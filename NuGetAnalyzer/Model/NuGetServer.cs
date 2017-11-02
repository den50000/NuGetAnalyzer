using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGetAnalyzer.Model
{
    public class NuGetServer
    {
        IPackageRepository _packageRepository;

        public NuGetServer(string repositoryURI)
        {
            // Connect to the package repository            
            _packageRepository = PackageRepositoryFactory.Default.CreateRepository(repositoryURI);
        }

        public event Action<string> ReportProgress;

        public IPackage GetPackage(string packageName, string packageVersion)
        {
            IPackage package = _packageRepository.FindPackagesById(packageName).Where(x => x.Version == SemanticVersion.Parse(packageVersion)).FirstOrDefault();
            if (package != null)
            {
                ReportProgress("Search package: " + package.GetFullName());
            }
            else
            {
                ReportError(String.Format("Can't find {0} {1}", packageName, packageVersion));
            }
            return package;
        }

        // will report full Nuget package name and true if it is latest
        public event Action<string, bool> ReportResult;
        public event Action<string> ReportError;

        public List<Tuple<string, SemanticVersion, SemanticVersion>> CompareIfLatest(List<Tuple<string, SemanticVersion>> packages)
        {
            List<Tuple<string, SemanticVersion, SemanticVersion>> updateNeededPackages = new List<Tuple<string, SemanticVersion, SemanticVersion>>();
            foreach (Tuple<string, SemanticVersion> package in packages)
            {
                // Get latest version of the package with a same id from given repository 
                IPackage nugetPackage = _packageRepository.FindPackagesById(package.Item1).Where(x => x.IsLatestVersion == true).FirstOrDefault();
                if (nugetPackage != null)
                {
                    // Will report what packages need to be updated and which already good and latest
                    if (package.Item2 == nugetPackage.Version)
                    {
                        ReportResult(nugetPackage.GetFullName(), package.Item2 == nugetPackage.Version);
                    }   
                    else
                    {
                        updateNeededPackages.Add(new Tuple<string, SemanticVersion, SemanticVersion>(package.Item1, package.Item2, nugetPackage.Version));
                        ReportResult(package.Item1 + " " + package.Item2 + " -> " + nugetPackage.GetFullName(), package.Item2 == nugetPackage.Version);
                    }
                }
                else
                {
                    ReportError(String.Format("Can't find {0} {1}", package.Item1, package.Item2.ToString()));
                }
            }
            return updateNeededPackages;
        }

        public SemanticVersion GetLatestVersion(string packageName)
        {
            IPackage nugetPackage = _packageRepository.FindPackagesById(packageName).Where(x => x.IsLatestVersion == true).FirstOrDefault();
            return nugetPackage.Version;
        }
    }
}
