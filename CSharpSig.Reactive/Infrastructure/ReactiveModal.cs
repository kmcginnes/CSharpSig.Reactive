using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace ImprovingU.Reactive.Infrastructure
{
    public abstract class ReactiveModal : ReactiveObject, IActivatable
    {
        readonly Subject<Unit> _closeSubject;
        public IObservable<Unit> CloseSignal => _closeSubject.AsObservable();

        protected ReactiveModal()
        {
            _closeSubject = new Subject<Unit>();
        }

        public void NotifyClose()
        {
            _closeSubject.OnNext(Unit.Default);
        }
    }
}
