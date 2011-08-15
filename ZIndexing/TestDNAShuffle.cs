using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wintellect.PowerCollections;
using ZIndexing.Dna;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestDNAShuffle
    {
        [Test]
        public void TestSpectrum() 
        {
            string str = "TATGGTGC";
            var spectrum = DnaSequence.ComputeLMers(3, str.ToCharArray());
            foreach (var x in spectrum) 
                System.Console.WriteLine(StringHelper.MakeString(x));

            Assert.AreEqual(6, spectrum.Count());

        }

        [Test]
        public void TestSpectrumWithRepeats() 
        {
            string str = "TATTATGGTGC";
            var spectrum = DnaSequence.ComputeLMers(3, str.ToCharArray());
            foreach (var x in spectrum) 
                System.Console.WriteLine(StringHelper.MakeString(x));

            Assert.AreEqual(8, spectrum.Count());

            var vertices = DnaSequence.ExpandLMers(3, spectrum);
            foreach (var x in vertices) 
                System.Console.WriteLine(StringHelper.MakeString(x));
        }


        [Test]
        public void TestSpectrumToVertices() 
        {
            string str = "ATGGCGTGCA";
            var spectrum = DnaSequence.ComputeLMers(3, str.ToCharArray());
            foreach (var x in spectrum) 
                System.Console.WriteLine(StringHelper.MakeString(x));

            var vertices = DnaSequence.ExpandLMers(3, spectrum);
            foreach (var x in vertices) 
                System.Console.WriteLine(StringHelper.MakeString(x));
        }


        public class A
        {
            public A SomeA { get; set; }
        }

        public class MyTree<T> 
        {
            public MyTree() 
            {
                Children = new List<MyTree<T>>();
            }

            public IList<T> Value { get; set; }
            public IList<MyTree<T>> Children { get; private set; }
            public void Clear() { throw new NotImplementedException(); }
            public void Insert(int index, MyTree<T> item) { Children.Insert(index, item); }
            public void RemoveAt(int index) { throw new NotImplementedException(); }
            public MyTree<T> this[int index]
            {
                get { return Children[index]; }
                set { Children[index] = value; }
            }

            public void Add(MyTree<T> item) { Insert(Count, item); }

            public int Count
            {
                get { return Children.Count; }
            }
        }

        [Test]
        public void testCirc1() 
        {
            var a = new MyTree<string>();
            var b = new MyTree<string>();
            var c = new MyTree<string>();

            a.Add(b);
            b.Add(c);
            c.Add(a);
        }


        [Test]
        public void testCirc() 
        {
            var a = new NTree<int>();
            var b = new NTree<int>();
            var c = new NTree<int>();
            a.Add(b);
            b.Add(c);
            c.Add(a);

        }




        [Test]
        public void TestGetDeBruijnGraph() 
        {
             string str = "ATAATTAG";
            //string str = "TATGGTGC";
            var spectrum = DnaSequence.GetDeBruijnGraph(3, str.ToCharArray());

            string str2 = DnaSequence.ShuffleGraph(spectrum);
            System.Console.WriteLine(str2);
        }


    }
}