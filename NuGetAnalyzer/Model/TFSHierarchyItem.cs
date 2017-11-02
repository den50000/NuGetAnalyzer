using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetAnalyzer.Model
{
   public class TFSHierarchyItem
    {   
        public TFSHierarchyItem()
        {
            Children = new List<TFSHierarchyItem>();            
            isSolutionItem = false;
        }
        public string Name { get; set; }
        public List<TFSHierarchyItem> Children { get; set; }
        public Item Source { get; set; }        
        public bool isSolutionItem { get; set; }
    }
}
