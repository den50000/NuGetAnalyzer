using NuGetAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NuGetAnalyzer.View
{
    /// <summary>
    /// Class contain extension methods related to the treeview and it is components
    /// </summary>
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Extension method that traverses the whole treeview 
        /// </summary>
        /// <param name="c">Tree node collection to start traverse from</param>
        /// <returns>Flat list of tree nodes</returns>
        /// <remarks>https://stackoverflow.com/questions/26542568/get-list-of-all-checked-nodes-and-its-subnodes-in-treeview</remarks>
        internal static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Method will build tree view hierarchy based on provided tfs hierarchy tree
        /// </summary>
        /// <param name="tree"></param>
        internal static TreeNode BuildFrom(this TreeNode treeNode, TFSHierarchyItem source)
        {
            // Except using recursion will use stack to traverse through provided TFSHierarchy items tree                        
            // Extract Item from stack -> process it -> add all items children to the stack -> repeat

            treeNode.Text = source.Name;
            treeNode.Tag = source;
            treeNode.Name = source.Name;
            var stack = new Stack<TreeNode>();
            stack.Push(treeNode);

            // Will infinitely process stack items until all of them is done         

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop(); // extract item from stack for processing
                foreach (var tfsHierarchyItem in ((TFSHierarchyItem)currentNode.Tag).Children)
                {
                    var childNode = new TreeNode(Path.GetFileName(tfsHierarchyItem.Name)) { Tag = tfsHierarchyItem, Name = tfsHierarchyItem.Name }; // will not use full path to display
                    currentNode.Nodes.Add(childNode);
                    stack.Push(childNode); // add child to the stack for further processing
                }
            }
            return treeNode;
        }

        internal static void AddRange(this TreeNodeCollection treeNodeCollection, TreeNodeCollection treeNodeCollectionToAdd)
        {
            // Create an array of TreeNodes
            TreeNode[] treeNodeArray = new TreeNode[treeNodeCollectionToAdd.Count];

            // Copy the tree nodes to the treeNodeArray 
            treeNodeCollectionToAdd.CopyTo(treeNodeArray, 0);
            
            // Add array to the treeNodeCollection
            treeNodeCollection.AddRange(treeNodeArray);
        }

        /// <summary>
        /// Updated node with a new children by substitute existing
        /// </summary>
        /// <param name="treeNodeToUpdate"></param>
        /// <param name="source"></param>
        internal static void UpdateChildren(this TreeNode treeNodeToUpdate, TreeNodeCollection treeNodeCollectionToAdd)
        {
            treeNodeToUpdate.TreeView.BeginUpdate();

            treeNodeToUpdate.Nodes.Clear();
            treeNodeToUpdate.Nodes.AddRange(treeNodeCollectionToAdd);

            treeNodeToUpdate.TreeView.EndUpdate();
        }

    }
}
