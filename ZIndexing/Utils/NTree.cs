using System;
using System.Collections.Generic;
using System.Text;
using Wintellect.PowerCollections;

namespace ZIndexing.Utils
{
    /// <summary>
    /// Cheap implementation of an n tree where any node can have 0 or more children.
    /// Nothing exciting to see here.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NTree<T> // : ListBase<NTree<T>>
    {
        public T Value { get; set; }
        public IList<NTree<T>> Children { get; private set; }

        public NTree<T> Parent { get; private set; }

        public NTree()
        {
            Children = new List<NTree<T>>();
        }

        public string ToString(string prefix) 
        {
            var strResult = new StringBuilder();
            strResult.Append(prefix);
            strResult.Append(Value.ToString());
            foreach(var child in Children) 
                strResult.Append(child.ToString(prefix + prefix));

            return strResult.ToString();
        }

        public void Add(NTree<T> item) { Insert(Count, item); }

        public /* override */ void Clear() { Children.Clear(); }
        
        public /* override */ void Insert(int index, NTree<T> item) 
        {
            Children.Insert(index, item);
            item.Parent = this;
        }

        public void Insert(int index, T item)
        {
            var newNode = new NTree<T> { Value = item, Parent = this};
            Children.Insert(index, newNode);
        }

        public /* override */ void RemoveAt(int index) { Children.RemoveAt(index); }
        
        public /* override */ NTree<T> this[int index]
        {
            get { return Children[index]; }
            set { Children[index] = value; }
        }

        public /* override */ int Count
        {
            get { return Children.Count; }
        }

        public NTree<T> Copy() 
        {
            var nodesSoFar = new Dictionary<NTree<T>, NTree<T>>();
            return Copy(nodesSoFar);
        }

        private NTree<T> Copy(Dictionary<NTree<T>, NTree<T>> far) 
        {
            NTree<T> node;
            if (far.TryGetValue(this, out node)) 
                return node;

            node = new NTree<T> {Value = this.Value};
            far.Add(this, node);
            foreach(var g in Children)
                node.Add(g.Copy(far));

            return node;
        }

        public Set<NTree<T>> FlatNodes 
        {
            get 
            {
                var nodes = new Set<NTree<T>>();
                GetNodes(nodes);
                return nodes;
            }
        }

        private void GetNodes(Set<NTree<T>> nodesSoFar) 
        {
            if (nodesSoFar.Contains(this))
                return;
            nodesSoFar.Add(this);
            foreach(var child in Children)
                child.GetNodes(nodesSoFar);
        }
    }
}