using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZIndexing.Utils
{
    public static class StringHelper
    {
        /// <summary>
        /// Extension function to allow us to print out character lists.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static string MakeString(this IEnumerable<char> from) 
        {
            var sw = new StringBuilder(from.Count());
            sw.Append(from.ToArray());
            return sw.ToString();
        }
    }
}