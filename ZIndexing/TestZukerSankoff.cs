using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZIndexing.RnaStruct;

namespace ZIndexing
{
    [TestFixture]
    public class TestZukerSankoff
    {
        public delegate R Action<T1, T2, T3, T4, T5, R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        public void runScore(string source, ScoreParameters parms,
                             Func<string, int, int, ScoreParameters, int> scoreFunction) { scoreFunction(source, 0, source.Length - 1, parms); }

        public void renderScore(string source, ScoreParameters parms,
                                Func<string, int, int, ScoreParameters, int> scoreFunction)
        {
            scoreFunction(source, 0, source.Length - 1, parms);
            parms.Render();
        }

        [Test]
        public void TestNaive()
        {
            var str = "ACGAUU";
            var parm = new NaiveScoreParameters();
            parm.Compute(str);
            parm.Render();
        }

        [Test]
        public void TestZS()
        {
            var str = "ACGAUU";
            var parm = new ZsScoreParameters{};
            parm.Compute(str);
            parm.Render();
        }

        [Test]
        public void TestScenario1() 
        {
        }


    }




}