using System;
using ReactiveUI;

namespace ImprovingU.Reactive.Infrastructure
{
    public interface IModalController
    {
        ReactiveModal Modal { get; }
        void ShowModal(ReactiveModal modal);
    }

    public class ModalController : ReactiveObject, IModalController
    {
        public ModalController()
        {
            this.WhenAnyObservable(x => x.Modal.CloseSignal)
                .Subscribe(_ =>
                {
                    (Modal as IDisposable)?.Dispose();
                    Modal = null;
                });
        }

        public void ShowModal(ReactiveModal modal)
        {
            Modal = modal;
        }

        ReactiveModal _modal;
        public ReactiveModal Modal
        {
            get { return _modal; }
            private set { this.RaiseAndSetIfChanged(ref _modal, value); }
        }
    }
}
