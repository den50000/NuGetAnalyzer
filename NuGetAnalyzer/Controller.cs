using NuGet;
using NuGetAnalyzer.Model;
using NuGetAnalyzer.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace NuGetAnalyzer
{
    class Controller
    {
        private readonly MainForm _view;
        private TeamFoundationServer _teamFundationServer;
        private NuGetServer _nuGetServer;
        private DGMLFile _dgmlFile;
        private DGMLBuilder _dgmlFileBuilder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"></param>
        public Controller(MainForm view)
        {
            // Initialize view and subscribe on events from it
            _view = view;
            _view.LoadComplete += LoadCompleteEventHandler;
            _view.TreeNodeExpanded += TreeNodeExpandedEventHandler;
            _view.GenerateClicked += GenerateClickedEventHandler;
            _view.FindDuplicatesClicked += FindDuplicatesEventHandler;
            _view.CompareIfLatestClicked += CompareIfLatestEventHandler;
        }

        /// <summary>
        /// Event handler executed when view finish loading
        /// </summary>
        private void LoadCompleteEventHandler()
        {
            // Initialize and subscribe on events from tfs server, nuget server, dgmlFile, dgmlBuilder
            _teamFundationServer = new TeamFoundationServer(ConfigurationManager.AppSettings["TFSServerAddress"]);
            _teamFundationServer.ReportProgress += ReportProgressEventHandler;
            _teamFundationServer.ReportError += ReportErrorEventHandler;

            // Read and show first level of the TFS hierarchy starting from a provided root path (include path)
            // This will allow to treeview control to show expand ("+") symbols
            TFSHierarchyItem root = _teamFundationServer.ReadChildrenHierarchy(ConfigurationManager.AppSettings["TFSInitialFolder"]);
            _view.ShowTFSHierarchy(root);

            _nuGetServer = new NuGetServer(ConfigurationManager.AppSettings["NuGetRepositoryURI"]);
            _nuGetServer.ReportProgress += ReportProgressEventHandler;
            _nuGetServer.ReportError += ReportErrorEventHandler;
            _nuGetServer.ReportResult += NuGetServerReportResultEventHandler;

            _dgmlFile = new DGMLFile(ConfigurationManager.AppSettings["DGMLFile"]);
            _dgmlFile.ReportError += ReportErrorEventHandler;

            _dgmlFileBuilder = new DGMLBuilder(_teamFundationServer, _nuGetServer, _dgmlFile);
            _dgmlFileBuilder.ReportProgress += ReportProgressEventHandler;

            ReportConfigSettings();
        }

        void ReportConfigSettings()
        {
            _view.WriteLineResult(Color.Lime, "TFS server address: " + ConfigurationManager.AppSettings["TFSServerAddress"]);
            _view.WriteLineResult(Color.Lime, "TFS Initial Folder: " + ConfigurationManager.AppSettings["TFSInitialFolder"]);
            _view.WriteLineResult(Color.Lime, "NuGet repositary URI: " + ConfigurationManager.AppSettings["NuGetRepositoryURI"]);
            _view.WriteLineResult(Color.Lime, "DGML File: " + ConfigurationManager.AppSettings["DGMLFile"]);
        }


        private void CompareIfLatestEventHandler()
        {
            // Parse dgml file and extract packages with version
            List<Tuple<string, SemanticVersion>> packages = _dgmlFile.GetPackages();

            foreach (Tuple<string, SemanticVersion, SemanticVersion> package in _nuGetServer.CompareIfLatest(packages))
            {
                //    if (_dgmlFile.IsWithoutDependencies(package))
                {
                    _view.WriteLineResult(Color.Khaki, package.Item1 + " " + package.Item2 + " --> " + package.Item3);
                    _dgmlFile.BuildDependencyChains(new Tuple<string, SemanticVersion>(package.Item1, package.Item2)).ForEach(x => _view.WriteLineResult(Color.Khaki, x));
                }

            }
        }

        private void ReportErrorEventHandler(string packageName)
        {
            _view.WriteLineError(packageName);
        }

        private void NuGetServerReportResultEventHandler(string packageName, bool isLatest)
        {
            if (isLatest)
            {
                _view.WriteLineResult(Color.Lime, packageName);
            }
            else
            {
                _view.WriteLineResult(Color.Khaki, packageName);
            }
        }

        private void FindDuplicatesEventHandler()
        {
            // Analyze dgml file and get groups of non unique packages
            var nonUnique = _dgmlFile.GetNonUnique();

            // Publish analysis result
            foreach (var group in nonUnique)
            {
                if (group.Count() > 1)
                {
                    _view.WriteLine(String.Format("{0}", group.Key), Color.Lime);
                    foreach (var item in group)
                    {
                        _view.WriteLine(String.Format("      {0}", item.Item2), Color.Orange);
                    }
                }

            }
        }

        /// <summary>
        /// Handle request to generate dgml file based on selected hierarchyItems
        /// </summary>
        /// <param name="selectedItems"></param>
        private void GenerateClickedEventHandler(List<TFSHierarchyItem> selectedItems)
        {
            _dgmlFileBuilder.Build(selectedItems);
        }

        private void ReportProgressEventHandler(string text)
        {
            _view.WriteLine(text);
        }

        /// <summary>
        /// When treeNode expanded next level of hierarchy for given node will be read from tfs
        /// </summary>
        /// <param name="expandedTreeNode"></param>
        private void TreeNodeExpandedEventHandler(TreeNode expandedTreeNode)
        {
            var item = (TFSHierarchyItem)expandedTreeNode.Tag;
            _teamFundationServer.ReadChildrenChildrenHierarchy(item);
            _view.UpdateChildren(expandedTreeNode, item);
        }
    }
}

