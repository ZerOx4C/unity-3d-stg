using System;
using System.Threading;
using Behaviour;
using Controller;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace Presenter
{
    public class TargetPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly FragmentController _fragmentController;

        [Inject]
        public TargetPresenter(FragmentController fragmentController)
        {
            _fragmentController = fragmentController;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Add(TargetBehaviour target)
        {
            target.OnDead
                .Subscribe(_ => _fragmentController.BreakAsync(target, CancellationToken.None).Forget())
                .AddTo(_disposables);
        }
    }
}
