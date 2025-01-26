using System;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace Controller
{
    public class AircraftController : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly FireController _fireController;
        private readonly FragmentController _fragmentController;

        [Inject]
        public AircraftController(
            FireController fireController,
            FragmentController fragmentController)
        {
            _fireController = fireController;
            _fragmentController = fragmentController;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Add(AircraftBehaviour aircraft)
        {
            aircraft.OnFire
                .Subscribe(g => _fireController.Fire(aircraft, g))
                .AddTo(_disposables);

            aircraft.OnDead
                .Subscribe(_ => _fragmentController.BreakAsync(aircraft, CancellationToken.None).Forget())
                .AddTo(_disposables);
        }
    }
}
