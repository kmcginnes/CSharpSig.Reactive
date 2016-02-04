using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImprovingU.Reactive
{
    public static class ObservableExtensions
    {
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> stream) where T : class 
        {
            return stream.Where(value => value != null);
        }

        public static IObservable<Unit> ToUnit<T>(this IObservable<T> stream)
        {
            return stream.Select(_ => Unit.Default);
        } 
    }
}