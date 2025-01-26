using System;
using Behaviour;
using R3;
using VContainer;
using Object = UnityEngine.Object;

namespace Controller
{
    public class TargetController : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public TargetController()
        {
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Add(TargetBehaviour target)
        {
            target.OnDead
                .Subscribe(_ => Object.Destroy(target.gameObject))
                .AddTo(_disposables);
        }
    }
}
