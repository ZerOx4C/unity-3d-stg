using System;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace Controller
{
    public abstract class AircraftControllerBase : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly FireController _fireController;
        private readonly FragmentController _fragmentController;

        [Inject]
        protected AircraftControllerBase(
            AircraftBehaviour aircraft,
            FireController fireController,
            FragmentController fragmentController)
        {
            Aircraft = aircraft;
            _fireController = fireController;
            _fragmentController = fragmentController;
        }

        protected AircraftBehaviour Aircraft { get; }

        public virtual void Dispose()
        {
            _disposables.Dispose();
        }

        public virtual void Initialize()
        {
            Aircraft.OnFire
                .Subscribe(g => _fireController.Fire(Aircraft, g))
                .AddTo(_disposables);

            Aircraft.OnDead
                .Subscribe(_ => _fragmentController.BreakAsync(Aircraft, CancellationToken.None).Forget())
                .AddTo(_disposables);
        }

        public abstract void Tick();
    }
}
