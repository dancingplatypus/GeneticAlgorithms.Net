using System;
using NUnit.Framework;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestDag
    {
        [Test]
        public void TestBuildDag() 
        {
            var d = Dag.Parse("a[bcd,efg[hij,k],lmn]");
            var t = d;

            Assert.AreEqual("a[bcd,efg[hij,k],lmn]", t.ToString());
            Assert.AreEqual(3, t.Children.Count);
            t = t.Children[1];
            Assert.AreEqual("efg[hij,k]", t.ToString());
            Assert.AreEqual("hij", t.Children[0].Value);
            Assert.AreEqual(2, t.Children.Count);
            t = t.Children[1];
            Assert.AreEqual("k", t.ToString());
            Assert.AreEqual("k", t.Value);
            Assert.AreEqual(0, t.Children.Count);
        }
    }
}