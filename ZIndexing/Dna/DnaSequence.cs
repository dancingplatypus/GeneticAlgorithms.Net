using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;
using ZIndexing.Utils;

namespace ZIndexing.Dna
{
    public class DnaSequence
    {
        /// <summary>
        /// Return a spectrum consisting of l-mers of length k
        /// </summary>
        public static IEnumerable<IEnumerable<T>> ComputeLMers<T>(int k, IList<T> sequence) 
        {
            var set = new Set<IEnumerable<T>>(new SequenceComparator<T>());
            for (var i = 0; i < sequence.Count - k + 1; i++) 
                set.Add(sequence.Skip(i).Take(k));
            return set;
        }

        /// <summary>
        /// Give the vertices of the (l-1) - mers
        /// </summary>
        public static IEnumerable<IEnumerable<T>> ExpandLMers<T>(int k, IEnumerable<IEnumerable<T>> sequence) 
        {
            var set = new Set<IEnumerable<T>>(new SequenceComparator<T>());
            foreach(var lmer in sequence) 
            {
                set.Add(lmer.Skip(1));
                set.Add(lmer.Take(k - 1));
            }
            return set;
        }

        static readonly SequenceComparator<Char> SeqCompare = new SequenceComparator<char>();

        /// <summary>
        /// Compute the DeBruijnGraph for a sequence of lmers.  We have to maintain O(n) to 
        /// satisfy our assignment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lmers"></param>
        /// <returns></returns>
        public static NTree<IEnumerable<char>> GetDeBruijnGraph(int k, IList<char> dna)
        {
            var strDna = dna.MakeString();
            
            var dictionary = new Dictionary<IEnumerable<char>, NTree<IEnumerable<char>>>(SeqCompare);

            NTree<IEnumerable<char>> first = null;
            NTree<IEnumerable<char>> last = null;

            for(var loop = 0; loop < strDna.Length - k + 1; loop++) 
            {
                NTree<IEnumerable<char>> entry;

                var left = strDna.Substring(loop, k - 1);
                var right = strDna.Substring(loop+1, k - 1);

                if (!dictionary.TryGetValue(left, out entry)) 
                {
                    entry = new NTree<IEnumerable<char>> { Value = left };
                    dictionary[left] = entry;
                    last = entry;
                    if (first == null)
                        first = entry;
                }

                // does entry contain the right side?  if not, add it
                var child = entry.Children.FirstOrDefault(x => SeqCompare.Equals(x.Value, right) );
                if (child == null)
                {
                    NTree<IEnumerable<char>> subEntry;
                    if (!dictionary.TryGetValue(right, out subEntry))
                    {
                        subEntry = new NTree<IEnumerable<char>> {Value = right};
                        dictionary[right] = subEntry;
                    }

                    entry.Add(subEntry);
                    last = subEntry;
                }
            }

            return first;
        }

        /// <summary>
        /// Randomly walk all the nodes of a DeBruijn graph to create a shuffled DNA.  We are
        /// going to use Fleury's algorithm.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static string ShuffleGraph(NTree<IEnumerable<char>> graph) 
        {
            var reducedGraph = graph.Copy();
            var nodes = reducedGraph.FlatNodes;

            Random r = new Random();

            var strResult = "";

            // find a starting point -- I was allowing arbitrary starting points,
            // but I end up needing to always start from the first one anyway.
            // var rand = r.Next() % reducedGraph.FlatNodes.Count;
            // var start = reducedGraph.FlatNodes.ElementAt(rand);
            
            var start = reducedGraph;

            var node = start;

            strResult += node.Value.MakeString();

            do
            {
                var setChildren = new Set<NTree<IEnumerable<char>>> ();

                // find our choices
                foreach(var child in node.Children.ToArray()) 
                {
                    // remove the branch
                    node.Children.Remove(child);
                    if (!IsDisconnected(node) && child.Children.Count > 0)
                    {
                        setChildren.Add(child);
                    }
                    node.Add(child);
                }

                var oldNode = node;

                if (setChildren.Count == 0) 
                {
                    node = node.Children.First();
                    oldNode.RemoveAt(0);
                }
                else
                {
                    var rand = r.Next()%setChildren.Count;
                    node = setChildren.ElementAt(rand);
                    oldNode.Children.Remove(node);
                }

                strResult += node.Value.MakeString().Substring(1);
            } 
            while (node.Children.Count > 0);

            return strResult;
        }

        public static bool IsDisconnected(NTree<IEnumerable<char>> graph) 
        {
            var set = graph.FlatNodes.Clone();
            RemoveNodes(graph, set);
            return (set.Count == 0) ? false : true;
        }

        private static void RemoveNodes(NTree<IEnumerable<char>> graph, ICollection<NTree<IEnumerable<char>>> set) 
        {
            if (set.Contains(graph))
            {
                set.Remove(graph);
                foreach (var g in graph.Children)
                    RemoveNodes(g, set);
            }
        }
    }
}