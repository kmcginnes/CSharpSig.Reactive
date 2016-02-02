using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ImprovingU.Reactive.Infrastructure
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

        /// <summary>
        /// Aggregates the items in the observable sequence into a readonly list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<IReadOnlyList<T>> ToReadOnlyList<T>(this IObservable<T> source)
        {
            return source.ToList().Select(list => new ReadOnlyCollection<T>(list));
        }

        /// <summary>
        /// Aggregates the items in the observable sequence into a readonly list.
        /// </summary>
        public static IObservable<IReadOnlyList<TResult>> ToReadOnlyList<T, TResult>(this IObservable<IEnumerable<T>> source, Func<T, TResult> map)
        {
            return source.Select(items => new ReadOnlyCollection<TResult>(items.Select(map).ToList()));
        }
    }
}
