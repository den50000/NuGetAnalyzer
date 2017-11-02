using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetAnalyzer.Model
{
    public class TeamFoundationServer
    {
        private readonly VersionControlServer _versionControlServer;

        /// <summary>
        /// Constructor. Will try to connect to the tfs with provided tfs server name
        /// </summary>
        public TeamFoundationServer(string TFSServerName)
        {
            // Try to connect to the TFS with current user authentication
            var projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(TFSServerName));
            projectCollection.EnsureAuthenticated();
            _versionControlServer = projectCollection.GetService<VersionControlServer>();
        }

        /// <summary>
        /// Will read tfs tree hierarchy
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public TFSHierarchyItem ReadFullHierarchy(TFSHierarchyItem root)
        {
            // Start recursively iterating through tree structure starting from a provided root
            var stack = new Stack<TFSHierarchyItem>();
            stack.Push(root);
            // Will infinitely process stack items until all of them is done  
            while (stack.Count > 0)
            {
                var currentNode = stack.Pop(); // take from stack

                Item[] children = ReadChildrenHierarchy(currentNode);
                stack.PushRange(currentNode.Children); // add all discovered children to the stack for the future processing
                ReadSolutionItem(currentNode, children); // important to do it after adding children in to the stack otherwise solution will be added infinitely 
            }

            return root;
        }

        // Event used to report progress to controller
        public event Action<string> ReportProgress;

        /// <summary>
        /// Will find solution item among provided children items and build and add corresponding tfs hierarchy item and current parent node 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="children"></param>
        private void ReadSolutionItem(TFSHierarchyItem parent, Item[] children)
        {
            foreach (Item item in children)
            {
                if (item.ServerItem.Contains(".sln"))
                {
                    // Build TFSHierarchyItem and mark it solution

                    TFSHierarchyItem solutionNode = item.ToTFSHierarchyItem();
                    solutionNode.isSolutionItem = true;
                    parent.Children.Clear();  // clear any existing children at this point only solution children expected
                    parent.Children.Add(solutionNode);
                    ReportProgress("Solution file found " + item.ServerItem);
                }
            }
        }

        /// <summary>
        /// Method will read one level of the tfs hierarchy 
        /// </summary>
        /// <param name="path">Path to the hierarchy item all children of which would be read</param>
        /// <returns>Hierarchy item with children</returns>
        public TFSHierarchyItem ReadChildrenHierarchy(string path)
        {
            // Build root item and read all children after
            Item root = _versionControlServer.GetItem(path);
            var rootTFSHierarchyItem = root.ToTFSHierarchyItem();

            ReadChildrenHierarchy(rootTFSHierarchyItem);

            return rootTFSHierarchyItem;
        }

        /// <summary>
        /// Read and populated children hierarchy down to solution file level
        /// </summary>
        /// <param name="root">>Root item from which population started</param>
        /// <returns> List of children which might be used to get access to filtered out items</returns>
        private Item[] ReadChildrenHierarchy(TFSHierarchyItem root)
        {
            // Will read one level of folders hierarchy unless it contain solution file.              
            Item[] children = _versionControlServer.GetItems(root.Name, RecursionType.OneLevel).Items; // GetItems will return all children including parent like first item
            if (!children.Any(x => x.ServerItem.Contains(".sln"))) // filter it is out down to solution level to speed up
            {
                // Substitute existing children with newly read
                root.Children.Clear(); 
                foreach (var child in children)
                {
                    if (child.ServerItem == root.Name) // exclude root directory
                        continue;

                    if (child.ItemType == ItemType.Folder) // will exclude files. Folders only
                    {
                        // Build corresponding children ToTFSHierarchyItems and add it to the parent 
                        var newChildItem = child.ToTFSHierarchyItem();
                        root.Children.Add(newChildItem);
                        ReportProgress("Read " + newChildItem.Name);
                    }
                }
            }
            return children;
        }

        /// <summary>
        /// Read additional (second) level af tfs hierarchy 
        /// </summary>
        /// <param name="root">Root from which population started</param>
        /// <returns>TFSHierarchyItem with children and their children populated</returns>
        public void ReadChildrenChildrenHierarchy(TFSHierarchyItem root)
        {
            foreach (var child in root.Children)
            {
                ReadChildrenHierarchy(child);
            }
        }
        public event Action<string> ReportError;

        public TFSHierarchyItem GetPackageConfigs(string tfsPath)
        {
            // Connect to the TFS
            Item packageConfigItem = null;
            try
            {
                packageConfigItem = _versionControlServer.GetItem(Path.Combine(tfsPath, "packages.config"));
            }
            catch (Exception ex)
            {
                ReportError(ex.Message);
            }

            TFSHierarchyItem packageConfigTFSHierarchyItem = null;
            if (packageConfigItem != null)
            {
                packageConfigTFSHierarchyItem = new TFSHierarchyItem() { Name = packageConfigItem.ServerItem, Source = packageConfigItem };
            }
            return packageConfigTFSHierarchyItem;
        }

        /// <summary>
        /// Reload from tfs hierarchy for selected items and extract solution files
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        public List<TFSHierarchyItem> ExtractSolutions(List<TFSHierarchyItem> selectedItems)
        {
            List<TFSHierarchyItem> solutionList = new List<TFSHierarchyItem>();
            foreach (TFSHierarchyItem tFSHierarchyItem in selectedItems)
            {
                // Reread tfs hierarchy
                TFSHierarchyItem loadedBrunch = ReadFullHierarchy(tFSHierarchyItem);
                solutionList.AddRange(loadedBrunch.Children.Flatten(x => x.Children)
                                                           .Where(x => x.isSolutionItem == true)
                                                           .ToList());
            }

            return solutionList;
        }
      
    }

}

