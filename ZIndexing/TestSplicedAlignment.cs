using System.Collections.Generic;
using NUnit.Framework;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestSplicedAlignment
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="k">number of intervals</param>
        /// <returns></returns>
        public IEnumerable<int> ExonChaining(NTree<string> graph, int k) 
        {
            return null;
        }

        [Test]
        public void TestAlignment() 
        {
            // input:  two Dags.
            // output: paths P1 and P2 in two graphs with the labelling sequences
            //
            // compute three dimensional table, S(i,j,B)
            // score of the optimal spliced alignment is 
            // max[all blocks B]S(end(B), length(T), B)
            //





        }
    }
}