using System;
using Behaviour;
using Input;
using R3;
using VContainer;

public class PlayerAircraftController : IDisposable
{
    private readonly AircraftBehaviour _aircraft;
    private readonly AircraftInput _aircraftInput;
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    public PlayerAircraftController(AircraftBehaviour aircraft, AircraftInput aircraftInput)
    {
        _aircraft = aircraft;
        _aircraftInput = aircraftInput;
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    public void Initialize()
    {
        _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
            .Subscribe(c => _aircraft.Movement.Pitch(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
            .Subscribe(c => _aircraft.Movement.Roll(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
            .Subscribe(c => _aircraft.Movement.Yaw(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
            .Subscribe(c => _aircraft.Movement.Throttle(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnBegan
            .Subscribe(_ => _aircraft.Fire(true))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnEnded
            .Subscribe(_ => _aircraft.Fire(false))
            .AddTo(_disposables);
    }
}
