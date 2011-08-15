using System.Collections.Generic;

namespace ZIndexing.Dna
{
    public class SequenceComparator<T> : IEqualityComparer<IEnumerable<T>> 
    {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y) 
        {
            var enum1 = x.GetEnumerator();
            var enum2 = y.GetEnumerator();

            while (enum1.MoveNext()) 
            {
                if (!enum2.MoveNext()) return false;
                if (!enum1.Current.Equals(enum2.Current))
                    return false;
            }

            if (enum2.MoveNext())
                return false;

            return true;
        }

        /// <summary>
        /// By returning a constant, we force equals to be evaluated
        /// </summary>
        public int GetHashCode(IEnumerable<T> obj) 
        {
            return 0;
        }
    }
}