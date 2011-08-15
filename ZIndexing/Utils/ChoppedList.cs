using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace ZIndexing.Utils
{
    public static class ChoppedList
    {
        public static ChoppedList<T> MakeList<T>(IList<T> list) 
        {
            return new ChoppedList<T>(list);
        }
    }

    /// <summary>
    /// Take a subsection of some underlying list without copying anything.  Fast.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChoppedList<T> : ListBase<T>
    {
        private readonly IList<T> _list;

        public ChoppedList(IList<T> list)
        {
            this._list = list;
            Start = 0;
            End = list.Count;
        }

        public ChoppedList(IList<T> list, int start)
        {
            this._list = list;
            Start = start;
            End = list.Count;
        }

        public ChoppedList(IList<T> list, int start, int end)
        {
            this._list = list;
            Start = start;
            End = end;
        }

        public override int Count
        {
            get { return Math.Max(0, End - Start); }
        }

        public int End { get; set; }
        public int Start { get; set; }

        public override T this[int index]
        {
            get
            {
                if (index + Start >= End)
                    throw new IndexOutOfRangeException();
                return this._list[index + Start];
            }
            set
            {
                if (index + Start >= End)
                    throw new IndexOutOfRangeException();

                this._list[index + Start] = value;
            }
        }

        public override void Clear() { throw new NotImplementedException(); }
        public override void Insert(int index, T item) { throw new NotImplementedException(); }
        public override void RemoveAt(int index) { throw new NotImplementedException(); }
    }
}