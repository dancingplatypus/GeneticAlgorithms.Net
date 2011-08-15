using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wintellect.PowerCollections;
using ZIndexing.Utils;

namespace ZIndexing.SuffixTrees
{
    /// <summary>
    /// Normal suffix tree
    /// </summary>
    public class SuffixTree<T> where T:IComparable
    {
        public int Ping { get; set; }

        private IList<T> _source;
        private NTree<Pair<List<int>, ChoppedList<T>>> _tree = new NTree<Pair<List<int>, ChoppedList<T>>>();

        public override string ToString() 
        {
            return _tree.ToString("  ");
        }

        /// <summary>
        /// Hold the locations in the source for searches later on.
        /// </summary>

        /// <summary>
        /// To find a pattern, we simply walk the suffix tree.  If we are able to walk,
        /// then we take the look at the indexes in the leaf to know where they are in the
        /// source string.  If the pattern is longer than the length of the source - index,
        /// then disregard
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IEnumerable<int> Find(IList<T> pattern) 
        {
            var fail = new int[0];
            var tree = _tree;
            var offset = 0;
            for(int i = 0; i < pattern.Count; i++) 
            {
                    if (tree.Value.Second.Count <= offset)
                    {
                        if (tree.Children.Count == 0)
                            return fail;

                        // we are not at a leaf, look for the new tree (if it doesn't exist... make it)
                        for (var branchIndex = 0; branchIndex < tree.Children.Count; branchIndex++)
                        {
                            if (pattern[i].Equals(tree.Children[branchIndex].Value.Second[0]))
                            {
                                tree = tree.Children[branchIndex];
                                offset = 1;
                                break;
                            }
                        }

                        if (offset != 1)
                            return fail;

                        continue;
                    }

                    if (tree.Value.Second[offset].Equals(pattern[i]))
                        offset++;
                    else
                        return fail;
            }

            // if you made it here, then you have a match.
            return tree.Value.First.Where(x => pattern.Count + x <= _source.Count);
        }

        /// <summary>
        /// The strategy is to add the new pattern to the existing tree.  As we add, if we find ourselves
        /// creating a new node, we just stop and record the current extension up to the mismatch if it is shorter
        /// than our current mismatch.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<T> FindShortestUniqueSubstring(IList<T> source) 
        {
            IList<T> winner = source;

            this._tree.Value = MakeList(source, 0, 0);

            Ping = 0;
            int phase;

            for(phase = 0; phase < source.Count; phase++) 
            {
                var chopSource = ChoppedList.MakeList(source);
                chopSource.Start = phase;
                var tree = this._tree;
                var offset = 0;

                for(var extension = phase; extension < source.Count; extension++) 
                {
                    chopSource.End = extension + 1;
                    Ping++;
                    if (tree.Value.Second.Count <= offset) 
                    {
                        // Are we at a leaf? If so, just extend it -- shortcut, alread done with phase
                        if (tree.Children.Count == 0)
                        {
                            if (chopSource.Count < winner.Count)
                                winner = chopSource;
                            break;
                        }

                        // we are not at a leaf, look for the new tree (if it doesn't exist... make it)
                        for (var branchIndex = 0; branchIndex < tree.Children.Count; branchIndex++) 
                        {
                            if (source[extension].Equals(tree.Children[branchIndex].Value.Second[0])) 
                            {
                                tree = tree.Children[branchIndex];
                                offset = 1;
                                break;
                            }
                        }

                        if (offset == 1)
                            continue;

                        if (chopSource.Count < winner.Count)
                            winner = chopSource;
                        break;
                    }

                    // If everything is equal, there's nothing to do, need to keep looking
                    if (tree.Value.Second[offset].Equals(source[extension]))
                    {
                        offset++;
                        chopSource.End = extension + offset + 1;
                        if (winner.Count < chopSource.Count)
                            break;
                        continue;
                    }

                    if (chopSource.Count < winner.Count)
                        winner = chopSource;

                    break;
                }
            }

            if (winner == source)
                return new List<T>();

            return winner;
        }



        private Pair<List<int>, ChoppedList<T>> MakeList(IList<T> source, int start, int end) 
        {
            return new Pair<List<int>, ChoppedList<T>>(new List<int>(), new ChoppedList<T>(source, start, end));
        }

        /// <summary>
        /// Builds an implicit suffix tree out of a source.
        /// 
        /// Notice that we keep pointers into the source.  We do not ever copy strings anywhere.  This allows
        /// our storage space to be O(n) (It's dependent on how many nodes are in the implicit tree)
        /// </summary>
        /// <param name="source"></param>
        public void BuildTree(IList<T> source) 
        {
            _source = source;
            this._tree.Value = MakeList(source, 0, 0);

            Ping = 0;
            for(var phase = 0; phase < source.Count; phase++) 
            {
                var tree = this._tree;
                var offset = 0;

                for(var extension = phase; extension < source.Count; extension++) 
                {
                    Ping++;
                    if (tree.Value.Second.Count <= offset) 
                    {
                        // Are we at a leaf? If so, just extend it -- shortcut, alread done with phase
                        if (tree.Children.Count == 0)
                        {
                            tree.Value.Second.End = source.Count;
                            break;
                        }

                        // we are not at a leaf, look for the new tree (if it doesn't exist... make it)
                        for (var branchIndex = 0; branchIndex < tree.Children.Count; branchIndex++) 
                        {
                            if (source[extension].Equals(tree.Children[branchIndex].Value.Second[0])) 
                            {
                                tree = tree.Children[branchIndex];
                                tree.Value.First.Add(phase);
                                offset = 1;
                                break;
                            }
                        }

                        if (offset == 1)
                            continue;

                        // Since we are adding a new child, we can shortcut this extension
                        var newChild = new ChoppedList<T>(source, extension);
                        var newTree = new NTree<Pair<List<int>, ChoppedList<T>>>
                                          {Value = new Pair<List<int>, ChoppedList<T>>(new List<int>(), newChild)};
                        newTree.Value.First.Add(phase);
                        tree.Children.Add(newTree);
                        break;
                    }

                    // If everything is equal, there's nothing to do, need to keep looking
                    if (tree.Value.Second[offset].Equals(source[extension]))
                    {
                        offset++;
                        continue;
                    }

                    // we failed in the middle of the tree.  Split it
                    // left will hold the original string before we inject ourselves
                    var left = new ChoppedList<T>(source, tree.Value.Second.Start + offset, tree.Value.Second.End);
                    var treeLeft = new NTree<Pair<List<int>, ChoppedList<T>>>
                                       {Value = new Pair<List<int>, ChoppedList<T>>(new List<int>(), left)};
                    // a fix because of an implementation detail.
                    if (treeLeft.Value.Second.Start == 0)
                        treeLeft.Value.First.Add(0);
                    treeLeft.Value.First.AddRange(tree.Value.First);

                    // right will hold our new suffix
                    var right = new ChoppedList<T>(source, extension, source.Count);
                    var treeRight = new NTree<Pair<List<int>, ChoppedList<T>>>
                                        {Value = new Pair<List<int>, ChoppedList<T>>(new List<int>(), right)};
                    treeRight.Value.First.Add(phase);

                    tree.Children.Add(treeLeft);
                    tree.Children.Add(treeRight);

                    tree.Value.Second.End = tree.Value.Second.Start + offset;

                    break;
                }
            }
        }


        private class WorkItem
        {
            public string WovenString { get; set; }

            public int StringOffset1 { get; set; }
            public int StringOffset2 { get; set; }
            public int Offset { get; set; }
            public NTree<Pair<List<int>, ChoppedList<T>>> Tree { get; set; }
        }

        public IEnumerable<int> FindWoven(IList<T> string1, IList<T> string2, out string wovenString) 
        { 
            var fail = new int[0];

            var workItems = new Stack<WorkItem>();

            workItems.Push(new WorkItem {StringOffset1 = 0, StringOffset2 = 0, Tree = _tree, WovenString = "", Offset = 0});

            // at each step, we can take a character from string1 or from string2.
            // if both are possible, push one possibility and take the other.
            // if our search fails at any point, then pop from the stack and try the
            // next possibility.

            while(workItems.Count > 0) 
            {
                var workItem = workItems.Pop();

                if (workItem.WovenString.Length == string1.Count + string2.Count) 
                {
                    wovenString = workItem.WovenString;
                    return workItem.Tree.Value.First;
                }

                var tree = workItem.Tree;

                if (tree.Value.Second.Count > workItem.Offset) 
                {
                    if (workItem.StringOffset1 < string1.Count)
                        if (tree.Value.Second[workItem.Offset].Equals(string1[workItem.StringOffset1]))
                            workItems.Push(new WorkItem
                                {
                                    WovenString = workItem.WovenString + tree.Value.Second[workItem.Offset],
                                    StringOffset1 = workItem.StringOffset1 + 1, 
                                    StringOffset2 = workItem.StringOffset2, 
                                    Offset = workItem.Offset + 1,
                                    Tree = tree
                                }
                            );

                    if (workItem.StringOffset2 < string2.Count)
                        if (tree.Value.Second[workItem.Offset].Equals(string2[workItem.StringOffset2]))
                            workItems.Push(new WorkItem 
                                {
                                    WovenString = workItem.WovenString + tree.Value.Second[workItem.Offset],
                                    StringOffset1 = workItem.StringOffset1, 
                                    StringOffset2 = workItem.StringOffset2 + 1, 
                                    Offset = workItem.Offset + 1,
                                    Tree = tree
                                }
                            );
                }
                else
                {
                    var found1 = false;
                    var found2 = false;

                    // we are not at a leaf, look for the new tree (if it doesn't exist... make it)
                    for (var branchIndex = 0; branchIndex < tree.Children.Count; branchIndex++)
                    {
                        if (!found1)
                            if (workItem.StringOffset1 < string1.Count)
                                if (string1[workItem.StringOffset1].Equals(tree.Children[branchIndex].Value.Second[0]))
                                {
                                    workItems.Push(new WorkItem
                                                       {
                                                           WovenString = workItem.WovenString + string1[workItem.StringOffset1],
                                                           StringOffset1 = workItem.StringOffset1 + 1,
                                                           StringOffset2 = workItem.StringOffset2,
                                                           Offset = 1,
                                                           Tree = tree.Children[branchIndex]
                                                       });
                                    found1 = true;
                                }

                        if (!found2)
                            if (workItem.StringOffset2 < string2.Count)
                                if (string2[workItem.StringOffset2].Equals(tree.Children[branchIndex].Value.Second[0]))
                                {
                                    workItems.Push(new WorkItem
                                                       {
                                                           WovenString = workItem.WovenString + string2[workItem.StringOffset2],
                                                           StringOffset1 = workItem.StringOffset1,
                                                           StringOffset2 = workItem.StringOffset2+1,
                                                           Offset = 1,
                                                           Tree = tree.Children[branchIndex]
                                                       });
                                    found2 = true;
                                }

                        if (found1 && found2)
                            break;
                    }
                }
            }

            // if you made it here, then you have a match.
            wovenString = null;
            return fail;
       }
    }
}