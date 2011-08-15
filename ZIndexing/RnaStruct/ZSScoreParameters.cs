using System;
using Wintellect.PowerCollections;

namespace ZIndexing.RnaStruct
{
    public class ZsScoreParameters : ScoreParameters
    {
        public int a;
        public int b;
        public int c;
        public WScore[,] V;
        public WScore[] W;
        public WScore[,] WM;
        public string Source { get; set; }

        public void Compute(string source)
        {
            Source = source;
            var size = source.Length;
            this.W = new WScore[size];
            this.V = new WScore[size,size];
            this.WM = new WScore[size,size];
            ComputeW(size - 1);
        }

        /// <summary>
        /// This simply allows you to add numbers together.  One it hits positive infinity, it won't go any fuorther.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int ProtectedAdd(params int[] x)
        {
            var accumulate = 0;
            foreach (var i in x)
            {
                if (i == int.MaxValue)
                    return int.MaxValue;
                accumulate += i;
            }
            return accumulate;
        }

        public void Render()
        {
            //for (var x = 0; x < Source.Length; x++)
            //    Console.Write("" + x + "\t");
            //Console.WriteLine();

            //for (var x = 0; x < Source.Length; x++)
            //    Console.Write("V\t");
            //Console.WriteLine();

            //for (var x = 0; x < Source.Length; x++)
            //{
            //    var m = this.W[x] ?? new BasicScore();
            //    Console.Write((m.score == int.MaxValue ? "." : "" + m.score) + "\t");
            //}
            //Console.WriteLine();

            Console.WriteLine("Base pairs:");
            var score = this.W[Source.Length - 1];
            EmitBasePairs(score);
        }

        private int ComputeV(int i, int j)
        {
            // is it out of bounds?
            if (i < 0 || j < 0 || i > Source.Length - 1 || j > Source.Length - 1)
                return int.MaxValue;

            // is it memoized?
            var v = this.V[i, j];
            if (v != null)
                return v.score;

            v = this.V[i, j] = new WScore {};

            // if there is no base pair, this is all moot
            if (!RNA.IsCompatible(Source[i], Source[j]))
            {
                v.score = int.MaxValue;
                return v.score;
            }

            // we ARE a base pair
            v.BasePair = new Pair<int, int>(i, j);
            v.UseBasePair = true;

            // the minimization has four possible paths.  Check hairpin first
            v.score = getEH(i, j);

            // check for stack
            var minFromEs = ProtectedAdd(getES(i, j, i + 1, j - 1), ComputeV(i + 1, j - 1));

            if (minFromEs < v.score)
            {
                v.score = minFromEs;
                v.UseV = true;
                v.TraceV = new Pair<int, int>(i + 1, j - 1);
            }

            // check for inner loops
            for (var iInner = i; iInner < j - 2; iInner++)
                for (var jInner = i + 1; jInner < j; jInner++)
                {
                    if ((iInner - i + j - jInner) <= 2)
                        continue;

                    var minFromEl = ProtectedAdd(getEL(i, j, iInner, jInner), ComputeV(iInner, jInner));
                    if (minFromEl < v.score)
                    {
                        v.score = minFromEl;
                        v.UseV = true;
                        v.TraceV = new Pair<int, int>(iInner, jInner);
                    }
                }

            // check for multiloops
            for (var k = i + 2; k < j; k++)
            {
                var minFromWm = ProtectedAdd(ComputeWm(i + 1, k - 1), ComputeWm(k, j - 1), this.a);
                if (minFromWm < v.score)
                {
                    v.score = minFromWm;
                    v.UseV = false;
                    v.UseWM1 = true;
                    v.UseWM2 = true;
                    v.TraceWM1 = new Pair<int, int>(i + 1, k - 1);
                    v.TraceWM2 = new Pair<int, int>(k, j - 1);
                }
            }

            return v.score;
        }

        private int ComputeW(int i)
        {
            // is it out of bounds?
            if (i < 0)
                return int.MaxValue;

            // is it memoized?
            var w = this.W[i];
            if (w != null)
                return w.score;

            // Nope? Then find the minimum energy
            w = this.W[i] = new WScore {score = ComputeW(i - 1) + 1};

            if (i - 1 >= 0)
            {
                w.UseW = true;
                w.TraceW = i - 1;
            }

            for (var k = -1; k < i; k++)
            {
                var possibleMin = ProtectedAdd(ComputeW(k) == int.MaxValue ? 0 : ComputeW(k), ComputeV(k + 1, i));
                if (possibleMin < w.score)
                {
                    if (ComputeW(k) != int.MaxValue)
                    {
                        w.UseW = true;
                        w.TraceW = k;
                    }

                    w.UseV = true;
                    w.TraceV = new Pair<int, int>(k + 1, i);

                    w.score = possibleMin;
                }
            }

            return w.score;
        }

        private int ComputeWm(int i, int j)
        {
            // Is it out of bounds?
            if (i < 0 || j < 0 || i > Source.Length - 1 || j > Source.Length - 1)
                return int.MaxValue;

            // is it memoized?
            var wm = this.WM[i, j];
            if (wm != null)
                return wm.score;

            // Nope? Then find the minimum energy
            wm = this.WM[i, j] = new WScore {};

            // Check for all of the features here first
            wm.score = ComputeV(i, j) + this.b;
            wm.UseV = true;
            wm.TraceV = new Pair<int, int>(i, j);

            // second part of the min equations
            var minFromWm1 = ProtectedAdd(ComputeWm(i, j - 1), this.c);
            if (minFromWm1 < wm.score)
            {
                wm.score = minFromWm1;
                wm.UseV = false;
                wm.UseWM1 = true;
                wm.TraceWM1 = new Pair<int, int>(i, j - 1);
            }

            // third part of the min equations
            var minFromWm2 = ProtectedAdd(ComputeWm(i + 1, j), this.c);
            if (minFromWm2 < wm.score)
            {
                wm.score = minFromWm2;
                wm.UseV = false;
                wm.UseWM1 = false;
                wm.UseWM2 = true;
                wm.TraceWM2 = new Pair<int, int>(i + 1, j);
            }

            // fourth part of the min equations
            for (var k = i + 1; k <= j; k++)
            {
                var minFromWm = ProtectedAdd(ComputeWm(i, k - 1), ComputeWm(k, j), this.a);
                if (minFromWm < wm.score)
                {
                    wm.score = minFromWm;
                    wm.UseV = false;
                    wm.UseWM1 = true;
                    wm.UseWM2 = true;
                    wm.TraceWM1 = new Pair<int, int>(i, k - 1);
                    wm.TraceWM2 = new Pair<int, int>(k, j);
                }
            }

            return wm.score;
        }

        /// <summary>
        /// This traces back through the matrices, outputting the base pairs that make up the solution
        /// </summary>
        /// <param name="score"></param>
        private void EmitBasePairs(WScore score)
        {
            if (score.UseBasePair)
                Console.WriteLine("(" + score.BasePair.First + "," + score.BasePair.Second + ")");

            if (score.UseW)
                EmitBasePairs(this.W[score.TraceW]);

            if (score.UseV)
                EmitBasePairs(this.V[score.TraceV.First, score.TraceV.Second]);

            if (score.UseWM1)
                EmitBasePairs(this.V[score.TraceWM1.First, score.TraceWM1.Second]);

            if (score.UseWM2)
                EmitBasePairs(this.V[score.TraceWM2.First, score.TraceWM2.Second]);
        }

        private int getEH(int i, int j) { return Math.Abs(i - j) - 1; }

        private int getEL(int i, int j, int i2, int j2) { return Math.Abs(i2 - i) + Math.Abs(j2 - j) - 2; }

        private int getES(int i, int j, int i2, int j2) { return Math.Abs(i2 - i) + Math.Abs(j2 - j) - 2; }

        public class WScore : BasicScore
        {
            public Pair<int, int> BasePair { get; set; }
            public Pair<int, int> TraceV { get; set; }
            public int TraceW { get; set; }
            public Pair<int, int> TraceWM1 { get; set; }
            public Pair<int, int> TraceWM2 { get; set; }
            public bool UseBasePair { get; set; }
            public bool UseV { get; set; }
            public bool UseW { get; set; }
            public bool UseWM1 { get; set; }
            public bool UseWM2 { get; set; }
        }
    }
}