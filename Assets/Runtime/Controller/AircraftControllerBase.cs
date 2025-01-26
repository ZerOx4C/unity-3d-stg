using System;
using Behaviour;
using R3;
using VContainer;

namespace Controller
{
    public abstract class AircraftControllerBase : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly FireController _fireController;

        [Inject]
        protected AircraftControllerBase(
            AircraftBehaviour aircraft,
            FireController fireController)
        {
            Aircraft = aircraft;
            _fireController = fireController;
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
        }

        public abstract void Tick();
    }
}
