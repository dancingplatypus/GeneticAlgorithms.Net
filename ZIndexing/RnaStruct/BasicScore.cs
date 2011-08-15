using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace ZIndexing.RnaStruct
{
    public class Node
    {
        public Pair<int, int> BasePair;
        public List<Node> Children = new List<Node>();
    }

    public class BasicScore
    {
        public bool filledOut;
        public int score;

        // base pair contributed by this score
        public IEnumerable<Node> Node = new Node[]{};
    }
}