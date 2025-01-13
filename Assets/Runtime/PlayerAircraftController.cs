using System;
using R3;
using VContainer;

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

    public void Initialize(AircraftBehaviour aircraftBehaviour)
    {
        _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
            .Subscribe(c => aircraftBehaviour.Movement.Pitch(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
            .Subscribe(c => aircraftBehaviour.Movement.Roll(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
            .Subscribe(c => aircraftBehaviour.Movement.Yaw(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
            .Subscribe(c => aircraftBehaviour.Movement.Throttle(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnBegan
            .Subscribe(_ => aircraftBehaviour.Fire(true))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnEnded
            .Subscribe(_ => aircraftBehaviour.Fire(false))
            .AddTo(_disposables);
    }
}
