using System;
using Behaviour;
using Input;
using R3;
using VContainer;

namespace Controller
{
    public class PlayerAircraftController : IDisposable
    {
        private readonly AircraftInput _aircraftInput;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public PlayerAircraftController(AircraftInput aircraftInput)
        {
            _aircraftInput = aircraftInput;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Initialize(AircraftBehaviour aircraft)
        {
            _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
                .Subscribe(c => aircraft.Movement.SetPitch(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
                .Subscribe(c => aircraft.Movement.SetRoll(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
                .Subscribe(c => aircraft.Movement.SetYaw(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
                .Subscribe(c => aircraft.Movement.SetThrottle(c.ReadValue<float>()))
                .AddTo(_disposables);

            _aircraftInput.Fire.OnBegan
                .Subscribe(_ => aircraft.Fire(true))
                .AddTo(_disposables);

            _aircraftInput.Fire.OnEnded
                .Subscribe(_ => aircraft.Fire(false))
                .AddTo(_disposables);
        }
    }
}
