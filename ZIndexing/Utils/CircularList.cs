using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace ZIndexing.Utils
{
    /// <summary>
    /// Make a list behave circularly.  No rotate function yet, so 0 index is fixed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularList<T> : ListBase<T>
    {
        private IList<T> _list;

        public CircularList(IList<T> list)
        {
            _list = list;
        }

        public override void Clear() { throw new NotImplementedException(); }
        public override void Insert(int index, T item) { throw new NotImplementedException(); }
        public override void RemoveAt(int index) { throw new NotImplementedException(); }
        public override T this[int index]
        {
            get 
            {
                index = (index < 0) ? (Count + (index%Count)) : index%Count;
                return _list[index]; 
            }
            set 
            { 
                index = (index < 0) ? (Count + (index%Count)) : index%Count;
                _list[index] = value; 
            }
        }

        public override int Count
        {
            get { return _list.Count; }
        }
    }
}