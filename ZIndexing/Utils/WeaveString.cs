using System;
using System.Collections.Generic;

namespace ZIndexing.Utils
{
    public class WeaveString
    {
        public static IEnumerable<string> WeaveStrings(string str1, string str2) 
        {
            if (str1.Length == 0) yield return str2;
            else if (str2.Length == 0) yield return str1;
            else
            {
                foreach(var x in WeaveStrings(str1.Substring(1), str2)) 
                    yield return str1[0] + x;

                foreach(var x in WeaveStrings(str1, str2.Substring(1))) 
                    yield return str2[0] + x;
            }
        }
    }
}