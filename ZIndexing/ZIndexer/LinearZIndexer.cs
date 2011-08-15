using System;
using System.Collections.Generic;
using System.Linq;
using ZIndexing.Utils;

namespace ZIndexing.ZIndexer
{
    public static class LinearZIndexer
    {
        public static int Counter = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="offset">how far from the beginning of the list should we start</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxZ"></param>
        /// <returns></returns>
        public static int ComputeZ<T>(IList<T> data, int offset, int start, int end, int maxZ) where T:IComparable
        {
            var i = 0;
            
            foreach(var entry in data.Skip(offset))
                if (!data.ElementAt(i + start).Equals(entry))
                    break;
                else
                {
                    i++;
                    Counter++;
                    if (i == maxZ)
                        break;
                }

            return i;
        }

        // to do this, we need 
        public static int ComputeLinearZ<T>(IList<T> data, 
                                            int offset,
                                            ref int prevLeft, 
                                            ref int prevRight, 
                                            out int caseNum,
                                            IList<int> Z) where T:IComparable
        {
            if (offset == 0) 
            {
                // set initial conditions
                prevLeft = 0;
                prevRight = 0;
                Z.Add(data.Count);
                caseNum = 0;
                return data.Count;
            }

            int newZ;
            Counter++;

            // CASE 1
            if (prevRight <= offset)
            {
                newZ = ComputeZ(data, offset, 0, -1, -1);
                prevLeft = offset;
                prevRight = offset + newZ;
                caseNum = 1;
            }
            else
            {
                int j = offset - prevLeft;
                if (Z.ElementAt(j) < prevRight - offset)
                {
                    // CASE 2
                    newZ = Z.ElementAt(j);
                    caseNum = 2;
                }
                else
                {
                    // CASE 3
                    var zSoFar = prevRight - offset;
                    newZ = zSoFar + ComputeZ(data, prevRight, zSoFar, -1, -1);
                    prevLeft = offset;
                    prevRight = offset + newZ;
                    caseNum = 3;
                }
            }

            Z.Add(newZ);
            return newZ;
        }


        public static IEnumerable<int> FindZ<T>(IList<T> pattern, IList<T> source, bool FirstMatchOnly) where T : IComparable
        {
            var union = new JoinedList<T>(pattern, source);
            var results = new List<int>();

            int left = 0;
            int right = 0;
            int caseNum;
            var Z = new List<int>(union.Count);

            for (int i = 0; i < union.Count; i++)
            {
                ComputeLinearZ(union, i, ref left, ref right, out caseNum, Z);
                if (i >= pattern.Count && Z[i] >= pattern.Count) 
                {
                    results.Add(i - pattern.Count);
                    if (FirstMatchOnly)
                        break;
                }
            }

            return results;
        }

        public static IEnumerable<int> FindZCircular<T>(IList<T> pattern, IList<T> source, bool FirstMatchOnly) where T : IComparable 
        {
            var list = 
                new CircularList<T>(
                    new ChoppedList<T>(
                        new JoinedList<T>(source, source), 0, source.Count + pattern.Count));

            return FindZ(pattern, list, FirstMatchOnly);
        }

    }
}