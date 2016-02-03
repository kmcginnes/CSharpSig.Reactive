using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using ImprovingU.Reactive.UI;

namespace ReactiveUI
{
    public static class ReactiveUiExtensions
    {
        public static IDisposable BindPassword<TViewModel, TView, TVMProp, TVProp>(this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TVMProp>> vmProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            SecurePasswordBox passwordBox) where TViewModel : class where TView : IViewFor
        {
            var oneWayBind = view.OneWayBind(viewModel, vmProperty, viewProperty);
            var bindTo = passwordBox.Events().TextChanged
                .Select(_ => passwordBox.Text)
                .BindTo(viewModel, vmProperty);
            return new CompositeDisposable(oneWayBind, bindTo);
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