using Behaviour;
using Input;
using R3;
using VContainer;

namespace Controller
{
    public class PlayerAircraftController : AircraftControllerBase
    {
        private readonly AircraftInput _aircraftInput;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public PlayerAircraftController(
            AircraftBehaviour aircraft,
            AircraftInput aircraftInput,
            BulletBehaviour bulletPrefab)
            : base(aircraft, bulletPrefab)
        {
            _aircraftInput = aircraftInput;
        }

        public override void Dispose()
        {
            base.Dispose();

            _disposables.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();

            _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
                .Subscribe(c => Aircraft.Movement.SetPitch(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
                .Subscribe(c => Aircraft.Movement.SetRoll(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
                .Subscribe(c => Aircraft.Movement.SetYaw(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
                .Subscribe(c => Aircraft.Movement.SetThrottle(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Fire.OnBegan
                .Subscribe(_ => Aircraft.Fire(true))
                .AddTo(_disposables);

            _aircraftInput.Fire.OnEnded
                .Subscribe(_ => Aircraft.Fire(false))
                .AddTo(_disposables);
        }

        public override void Tick()
        {
        }
    }
}
