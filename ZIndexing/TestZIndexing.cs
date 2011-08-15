using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ZIndexing.Utils;
using ZIndexing.ZIndexer;

namespace ZIndexing
{
    [TestFixture]
    public class TestZIndexing
    {
        [Test]
        public void TestStraightZIndexing()
        {
            string[] strings = { "abcabcdabcabcabcd", "aaaaaaaaaaaaaaaaa"};

            foreach(var str in strings)
            {
                Console.WriteLine(str);
                for (int i = 0; i < str.Length; i++)
                {
                    Console.WriteLine(String.Format("i = {0}, Z = {1}, Substr={2}", i,
                        LinearZIndexer.ComputeZ(str.ToCharArray(), i, 0, -1, -1), str.Substring(i)));
                }
            }
        }

        [Test]
        public void TestLinearZIndexing() 
        {
            string[] strings = 
            { 
                "Once upon a time there was a princess",
                "monkeyamonkeybmonkeycmonkeydmonkeye",
                "abcabcdabcabcabcdabcabcd", 
                "abcdefgabcdefgabcdefg", 
                "abcabcdabcabcd", 
                "abcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcd", 
                "aaaaaaaaaaaaaaaaa"
            };

            foreach(var str in strings)
            {
                int left = 0;
                int right = 0;
                int caseNum = 0;
                var Z = new List<int>(str.Length);

                Console.WriteLine(str);
                for (int i = 0; i < str.Length; i++)
                {
                    Console.WriteLine(String.Format("i = {0}, Z = {1}, LinearZ = {2} (C{3} L{4} R{5}), Substr={6}", i,
                        LinearZIndexer.ComputeZ(str.ToCharArray(), i, 0, -1, -1), 
                        LinearZIndexer.ComputeLinearZ(str.ToCharArray(), i, ref left, ref right, out caseNum, Z),
                        caseNum,
                        left, right,
                        str.Substring(i)));
                }
            }
        }

        [Test]
        public void TestRuthlessLinearZIndexing() 
        {
            string[] strings = 
            { 
                "abcdefgabcdefgabcdefg", 
                "abcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcd", 
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "aaabbbcccdddeeefffaaabbbcccdddeeefffaaabbb",
                "ababababababababababababababababababababa",
                "ababababababaababababababababababababababa",
                "ababababababababababababababababbababababa",
            };

            foreach(var str in strings)
            {
                int left = 0;
                int right = 0;
                int caseNum;
                var Z = new List<int>(str.Length);

                Console.WriteLine(str);

                for (int i = 0; i < str.Length; i++)
                {
                    Assert.AreEqual(
                        LinearZIndexer.ComputeZ(str.ToCharArray(), i, 0, -1, -1), 
                        LinearZIndexer.ComputeLinearZ(str.ToCharArray(), i, ref left, ref right, out caseNum, Z));
                }
            }
        }

        [Test]
        public void TestRuthlessLinearZIndexingPings() 
        {
            string[] strings = 
            { 
                "Once upon a time there was a princess",
                "Once upon Oa time there Once Oa princess",
                "aaabaaabbaaabbbaaabbbbaaabbbbbaaa",
                "monkeyamonkeybmonkeycmonkeydmonkeye",
                "monkeymonkeybmonkeycmonkeydmonkeye",
                "aaabbaaabaaababaaabababa",
                "ThisSentenceRepeatsExactlyOnceThisSentenceRepeatsExactlyOnce",
                "abcdefgabcdefgabcdefg", 
                "abcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcdabcabcdabcabcabcd", 
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "ababababababababababababababababababababa",
                "ababababababaababababababababababababababa",
                "ababababababababababababababababbababababa",
            };

            foreach(var str in strings)
            {
                int left = 0;
                int right = 0;
                int caseNum;
                var Z = new List<int>(str.Length);

                Console.WriteLine(str);

                LinearZIndexer.Counter = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    LinearZIndexer.ComputeZ(str.ToCharArray(), i, 0, -1, -1); 
                }
                Console.WriteLine(String.Format("Naive = {0}", LinearZIndexer.Counter));

                LinearZIndexer.Counter = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    LinearZIndexer.ComputeLinearZ(str.ToCharArray(), i, ref left, ref right, out caseNum, Z);
                }
                Console.WriteLine(String.Format("Linear = {0}", LinearZIndexer.Counter));
            }
        }


        [Test]
        public void TestFindStringZIndexing() 
        {
            string pattern = "monkey";

            string[] strings = 
            { 
                "keyturkeymonkeyamouseinamonkeyhousepokemon"
            };

            Console.WriteLine(String.Format(@"Searching for ""{0}""", pattern));

            foreach(var str in strings)
            {
                Console.WriteLine(String.Format(@"Source string: {0}", str));
                var offset = 0;
                var results = LinearZIndexer.FindZ(pattern.ToCharArray(), str.ToCharArray(), false);
                foreach(var result in results)
                    Console.WriteLine(String.Format("Linear found at offset {0}", result));

                results = LinearZIndexer.FindZCircular(pattern.ToCharArray(), str.ToCharArray(), 
                    false);
                foreach(var result in results)
                    Console.WriteLine(String.Format("Circular found at offset {0}", result));
            
            }
        }

        [Test]
        public void TestPeriodicStrings() 
        {
            string[] strings = 
            {
                "aaaaaaaaaaaaaaaaaaaaa",
                "abababababababababababababababab",
                "ababababababababababababababababa"

                // 0  1  2   3  4  5  6  7  8  9  10  11
                // a, b, a,  b, a, b, a, b, a, b,  a,  b
                // 12,0, 10, 0, 8, 0, 6, 0, 4, 0,  2,  0
                //
                // 12 / 1,  12 / 2, 12 / 4


                //  a,  b,  c,  a,  b,  c,  d
                // 

                 
            };

            foreach(var source in strings) 
            {
                int left = 0;
                int right = 0;
                int caseNum;
                var Z = new List<int>(source.Length);

                Console.WriteLine(source);

                for (int offset = 0; offset < source.Length / 2; offset++)
                {
                    LinearZIndexer.ComputeLinearZ(source.ToCharArray(), offset, ref left, ref right, out caseNum, Z);
                    if (offset != 0 && (Z[offset] / offset > 1 ))
                        Console.WriteLine
                            (String.Format("Prefix {0} is periodic {1} times ",
                            source.Substring(0, offset), Z[offset] / offset));
                }
            }

        }


    }
}
