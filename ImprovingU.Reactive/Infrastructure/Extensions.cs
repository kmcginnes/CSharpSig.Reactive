using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Reactive.Linq
{
    public static class Extensions
    {
        public static TResult IfNotNull<T, TResult>(
            this T that, 
            Func<T, TResult> selector, 
            TResult defaultValue = default (TResult))
        {
            return that != null ? selector(that) : defaultValue;
        }
    }
}
