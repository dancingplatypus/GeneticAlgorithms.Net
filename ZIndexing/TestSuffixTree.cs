using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZIndexing.SuffixTrees;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestSuffixTree
    {
        /// <summary>
        /// Just create some suffix trees.  Print out the number of loop iterations it took to construct.
        /// </summary>
        [Test]
        public void TestSimple()
        {
            string[] strings = {
                                   "papua", "suffix", "Once upon a time there was a princess who ",
                                   "bananabananabananabananabananabananabananaban",
                                   "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                                   "abcdefghijklmnopwrstuvwxyzABCDEFGHIJKLMNOP",
                                   "abcdefghijklmnopwrstuvwxyzdABCDEFGHIJKLMNO",
                                   "abcdefghijklmnopdrstuvwxyzdABCDEFGHIJKLMNO"
                               };

            foreach (var str in strings)
            {
                var tree = new SuffixTree<char>();
                tree.BuildTree(str.ToCharArray());
                Console.WriteLine(String.Format("Length = {0}, Ping = {1}", str.Length, tree.Ping));
                //Console.WriteLine(tree.ToString());
            }
        }

        /// <summary>
        /// Make sure the search feature of the suffix tree is working.
        /// </summary>
        [Test]
        public void TestSearch()
        {
            string strTest = "bananabananaorangebananaorangebananabananabananaban";
            string[] strFind = {"banana", "orange", "ban"};

            var tree = new SuffixTree<char>();
            tree.BuildTree(strTest.ToCharArray());

            foreach (var str in strFind) 
            {
                Console.WriteLine(String.Format("{0}", str));
                var results = tree.Find(str.ToCharArray());
                foreach(var r in results) 
                {
                    Console.WriteLine(r);
                }
            }
        }

        /// <summary>
        /// Special sort of Find.  We pretend to add some new string to the suffix tree to 
        /// see what the shortest add would end up being.
        /// </summary>
        [Test]
        public void TestShortestUniqueSubstring() 
        {
            string strTest = "bananabananaorangebananaorangebananabananabananaban";
            string[] strFind = {"bagel", "banabana", "fred"};

            var tree = new SuffixTree<char>();
            tree.BuildTree(strTest.ToCharArray());

            foreach (var str in strFind) 
            {
                var substr = tree.FindShortestUniqueSubstring(str.ToCharArray());

                if (substr.Count == 0)
                    Console.WriteLine(String.Format("{0} was found in the sequence", str));
                else
                    Console.WriteLine(String.Format("{0} - shortest unique substring = {1}", str, substr));
            }
        }


        [Test]
        public void TestSearchForWovenStrings()
        {
            string strTest = "obanraanngaeorange";
            string[] strFind = {"banana", "orange"};

            var tree = new SuffixTree<char>();
            tree.BuildTree(strTest.ToCharArray());

            string wovenString = "";

            var results = tree.FindWoven(strFind[0].ToCharArray(), strFind[1].ToCharArray(), out wovenString);
            Console.WriteLine(wovenString);
            foreach(var r in results) 
            {
                Console.WriteLine(r);
            }
        }





        public void AlignSequences<T>(IList<T> pattern1, IList<T> pattern2)
        {
            var matrix = new Matrix<T>(pattern1, pattern2);

            for (var i = 0; i < pattern1.Count; i++)
                for (var j = 0; j < pattern2.Count; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        matrix.Contents[i, j] = 0;
                        continue;
                    }

                    var a = matrix.Contents[i - 1, j - 1] + ScoreMatching(pattern1[i].ToString(), pattern2[j].ToString());
                    var b = matrix.Contents[i - 1, j];
                    var c = matrix.Contents[i, j - 1];

                    matrix.Contents[i, j] = Math.Max(Math.Max(a, b), c);
                }

            matrix.Print();
        }

        public static int ScoreMatching(string a, string b)
        {
            return (a == b) ? 1 : 0;
        }

        public class Matrix<T>
        {
            public IList<T>  Sequence1 { get; private set; }
            public IList<T>  Sequence2 { get; private set; }
            public int[,] Contents { get; private set; }

            public Matrix(IList<T> sequence1, IList<T> sequence2)
            {
                this.Sequence1 = sequence1;
                this.Sequence2 = sequence2;

                this.Contents = new int[sequence1.Count, sequence2.Count];
            }

            public void Print()
            {
                Console.Write("\t");
                this.Sequence2.ToList().ForEach(c => Console.Write("{0}\t", c));
                Console.WriteLine();

                for (var i = 0; i < this.Contents.GetLength(0); i++)
                {
                    Console.Write("{0}\t", this.Sequence1[i]);
                    for (var j = 0; j < this.Contents.GetLength(1); j++)
                        Console.Write("{0}\t", this.Contents[i, j]);
                    Console.WriteLine();
                }
            }
        }
















    }








}