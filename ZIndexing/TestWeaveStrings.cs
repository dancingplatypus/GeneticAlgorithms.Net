using NUnit.Framework;
using ZIndexing.Utils;

namespace ZIndexing
{
    [TestFixture]
    public class TestWeaveStrings
    {
        [Test]
        public void testWeaveString() 
        {
            string str1 = "abcdef";
            string str2 = "xyz";

            int loop = 0;
            foreach (var str in WeaveString.WeaveStrings(str1, str2)) 
            {
                loop++;
                System.Console.WriteLine(str);
            }
            
            System.Console.WriteLine(loop);

        }

    }
}