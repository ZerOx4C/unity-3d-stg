using System;
using Behaviour;
using R3;
using VContainer;

namespace Controller
{
    public class TargetController : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly FragmentController _fragmentController;

        [Inject]
        public TargetController(FragmentController fragmentController)
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
                .Subscribe(_ => _fragmentController.Break(target))
                .AddTo(_disposables);
        }
    }
}
