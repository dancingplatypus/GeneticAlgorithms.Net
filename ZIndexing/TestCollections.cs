using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestCollections
    {
        [Test]
        public void TestJoinedList() 
        {
            var a = new List<int> {1, 2, 3, 4};
            var b = new List<int> {5, 6, 7, 8};

            var j = new JoinedList<int>(a, b);

            Assert.AreEqual(8, j.Count);
            Assert.AreEqual(6, j[5]);
        }

        [Test]
        public void TestChoppedList() 
        {
            var a = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
            var c = new ChoppedList<int>(a, 4);
            Assert.AreEqual(5, c.Count);
            Assert.AreEqual(7, c[2]);

            c.Start = 2;
            Assert.AreEqual(7, c.Count);
            Assert.AreEqual(5, c[2]);

            c.End = 6;
            Assert.AreEqual(4, c.Count);
            Assert.AreEqual(6, c[3]);

            try 
            {
                int x = c[5];
                Assert.Fail("Should be out of range");
            }
            catch(IndexOutOfRangeException) {}

        }


        [Test]
        public void TestCircularList() 
        {
            var a = new List<int> {1, 2, 3, 4};
            var c = new CircularList<int>(a);
            Assert.AreEqual(4, c.Count);
            Assert.AreEqual(2, c[5]);
            Assert.AreEqual(4, c[-1]);
        }

        [Test]
        public void TestStringApplication() 
        {
            var a = "hello world";
            var c = new ChoppedList<char>(a.ToCharArray(), 3);
            Console.WriteLine(c.MakeString());
        }

    }
}