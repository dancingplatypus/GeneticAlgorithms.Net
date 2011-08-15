using System;

namespace ZIndexing.RnaStruct
{
    public class NaiveScoreParameters : ScoreParameters
    {
        public Score[,] M;

        public NaiveScoreParameters()
        {
        }

        public string Source { get; set; }
        public void Compute(string source) 
        { 
            this.Source = source; 
            this.M = new Score[source.Length,source.Length];
            getScoreNaive(0, source.Length - 1);
        }

        public int getScoreNaive(int x, int y)
        {
            // Console.WriteLine("Had to compute (" + x + "," + y + ") - " + debug);

            // if x and y are out of range, return 0 immediately
            if (x < 0 || y < 1)
                return 0;

            // if we've already memoized, return score immediately
            var s = this.M[x, y] ?? new Score();
            if (s.filledOut)
                return s.score;

            var maxScore = getScoreNaive(x, y - 1);

            for (var k = x; k < y; k++)
            {
                var possibleMaxScore = getScoreNaive(x, k - 1)
                    + getScoreNaive(k + 1, y - 1) + (RNA.IsCompatible(this.Source[k], this.Source[y]) ? 1 : 0);
                if (possibleMaxScore > maxScore)
                {
                    maxScore = possibleMaxScore;
                    if (RNA.IsCompatible(this.Source[k], this.Source[y]))
                    {
                        s.addPair = true;
                        s.pairX = k;
                        s.pairY = y;
                    }

                    s.leftX = k + 1;
                    s.leftY = y - 1;

                    s.rightX = x;
                    s.rightY = k - 1;
                }
            }

            s.score = maxScore;
            s.filledOut = true;
            this.M[x, y] = s;

            return maxScore;
        }

        public void Render()
        {
            Console.Write("  \t");
            for (var x = 0; x < this.Source.Length; x++)
                Console.Write("" + x + "\t");
            Console.WriteLine();

            Console.Write("  \t");
            for (var x = 0; x < this.Source.Length; x++)
                Console.Write("V\t");
            Console.WriteLine();

            for (var x = 0; x < this.Source.Length; x++)
            {
                Console.Write("" + x + ">\t");

                for (var y = 0; y < this.Source.Length; y++)
                {
                    var m = this.M[y, x] ?? new Score();
                    Console.Write(m.filledOut ? "" + m.score + "\t" : ".\t");
                }
                Console.WriteLine();
            }
        }

        public class Score : BasicScore
        {
            public bool addPair;
            public bool filledOut;

            public int leftX;
            public int leftY;
            public int pairX;
            public int pairY;

            public int rightX;
            public int rightY;
            public int score;
        }
    }
}