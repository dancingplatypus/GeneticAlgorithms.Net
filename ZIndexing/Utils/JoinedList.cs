using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace ZIndexing.Utils
{
    /// <summary>
    /// Concatenate two lists "in-place" (no copying of elements).  Fast.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JoinedList<T> : ListBase<T>
    {
        private int _count;
        private IList<T>[] _lists;
        public IList<T>[] Lists
        {
            get { return this._lists; }
            set 
            { 
                this._lists = value;
                this._count = -1;
            }
        }

        public JoinedList() 
        {
            this._count = 0;
        }

        public JoinedList(params IList<T>[] lists)
        {
            this.Lists = lists;
        }

        public override void Clear() { throw new NotImplementedException(); }
        public override void Insert(int index, T item) { throw new NotImplementedException(); }
        public override void RemoveAt(int index) { throw new NotImplementedException(); }
        
        public override T this[int index]
        {
            get 
            {
                foreach(var list in this.Lists) 
                {
                    if (index < list.Count)
                        return list[index];
                    index -= list.Count;
                }
                throw new IndexOutOfRangeException();
            }
            set 
            { 
                foreach(var list in this.Lists) 
                {
                    if (index < list.Count)
                        list[index] = value;
                    index -= list.Count;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public override int Count
        {
            get 
            { 
                if (this._count != -1)
                    return this._count;
                this._count = 0;
                foreach(var list in this.Lists) 
                    this._count += list.Count;
                return this._count;
            }
        }
    }
}