using System;
using System.Reactive.Linq;

namespace ImprovingU.Reactive
{
    public static class ObservableExtensions
    {
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> stream) where T : class 
        {
            return stream.Where(value => value != null);
        } 
    }
}