using Microsoft.TeamFoundation.VersionControl.Client;
using NuGet;
using NuGetAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NuGetAnalyzer.View
{
    /// <summary>
    /// Main window of the application responsible for sending user input to the controller and showing results
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public event Action<List<TFSHierarchyItem>> GenerateClicked;
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Get flat list of all checked treeNodes and send it payload to the controller
            List<TFSHierarchyItem> selectedItems = tfsHierarchyTree.Nodes.Descendants()
                                                                         .Where(n => n.Checked)
                                                                         .Select(n => (TFSHierarchyItem)n.Tag)
                                                                         .ToList();
            GenerateClicked(selectedItems);
        }

        public event Action<TreeNode> ApplyFilters;
        private void btnApplyFiltres_Click(object sender, EventArgs e)
        {
            ApplyFilters(tfsHierarchyTree.SelectedNode);
        }

        public event Action FindDuplicatesClicked;
        private void btnDuplicates_Click(object sender, EventArgs e)
        {
            FindDuplicatesClicked();
        }


        public event Action CompareIfLatestClicked;
        private void btnFindIfLatest_Click(object sender, EventArgs e)
        {
            CompareIfLatestClicked();
        }

        public event Action LoadComplete;
        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadComplete();
        }

        /// <summary>
        /// Event to notify that treeNode expanded by user
        /// </summary>
        public event Action<TreeNode> TreeNodeExpanded;
        private void tfsHierarchyTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNodeExpanded(e.Node);
        }

        /// <summary>
        /// Method will build tree view hierarchy based on provided tfs hierarchy tree
        /// </summary>
        /// <param name="tree"></param>
        public void ShowTFSHierarchy(TFSHierarchyItem tree)
        {
            // Clear tree before populating with a new data
            tfsHierarchyTree.Nodes.Clear();
            var node = new TreeNode().BuildFrom(tree);
            tfsHierarchyTree.Nodes.Add(node); // add result to the tree
        }

        /// <summary>
        /// Method will update node by substitute all nodes children 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="branch"></param>
        /// <remarks> Method used for Tree View lazy loading behavior when only upon expansion tree view actually populated
        /// Attempt to substitut treeNode themselves will not work because we need it expanded. Call Expand will trigger substitution again 
        /// </remarks>        
        public void UpdateChildren(TreeNode nodeToUpdate, TFSHierarchyItem branch)
        {
            // Build TreeNode hierarchy from provided tfs hierarchy
            var newBranch = new TreeNode().BuildFrom(branch);
            nodeToUpdate.UpdateChildren(newBranch.Nodes);
        }

        public void WriteLineResult(Color messageColor, string message, params object[] args)
        {
            string formattedMessage = GetFormattedMessage(message, args);
            rtbResult.WriteLineAsync(formattedMessage, messageColor);
        }

        public void WriteLineError(string message, params object[] args)
        {
            string formattedMessage = GetFormattedMessage(message, args);
            rtbOutput.WriteLineAsync(string.Format(message, args), Color.Red);
        }

        public void WriteLine(string message, params object[] args)
        {
            string formattedMessage = GetFormattedMessage(message, args);
            rtbOutput.WriteLineAsync(formattedMessage, Color.Lime);
        }

        private string GetFormattedMessage(string message, object[] args)
        {
            string formattedMessage;
            if (String.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            if (args == null)
            {
                formattedMessage = message;
            }
            else
            {
                formattedMessage = string.Format(message, args);
            }
            return formattedMessage;
        }

        private void tfsHierarchyTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    // this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }
        }

        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    // if (node.IsExpanded) // will check children only if node expanded
                    {
                        this.CheckAllChildNodes(node, nodeChecked);
                    }
                }
            }
        }


    }
}
