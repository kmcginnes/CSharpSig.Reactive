using System.Collections.Generic;
using System.Linq;

namespace ImprovingU.Reactive
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string s, IEnumerable<char> characters)
        {
            return s.IndexOfAny(characters.ToArray()) > -1;
        }
    }
}