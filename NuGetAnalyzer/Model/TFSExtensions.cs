using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGetAnalyzer.Model
{
    public static class TFSExtensions
    {
        // https://stackoverflow.com/questions/11830174/how-to-flatten-tree-via-linq
        /// <summary>
        /// Flatten tree hierarchy
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }

        public static TFSHierarchyItem ToTFSHierarchyItem(this Item source)
        {
            return new TFSHierarchyItem() { Name = source.ServerItem, Source = source };
        }

        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                stack.Push(item);
            }
        }

        public static void PushRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
