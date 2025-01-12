using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

public class BattleEntryPoint : IAsyncStartable, IDisposable
{
    private readonly AircraftController _aircraftControllerPrefab;
    private readonly AircraftInput _aircraftInput;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private AircraftController _aircraftController;

    [Inject]
    public BattleEntryPoint(
        AircraftController aircraftControllerPrefab,
        GameObject aircraftModelPrefab,
        BattleCameraController cameraController,
        AircraftInput aircraftInput)
    {
        _aircraftControllerPrefab = aircraftControllerPrefab;
        _aircraftModelPrefab = aircraftModelPrefab;
        _cameraController = cameraController;
        _aircraftInput = aircraftInput;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var instances = await Object.InstantiateAsync(_aircraftControllerPrefab, 1,
            null, Vector3.zero, Quaternion.identity, cancellation);

        _aircraftController = instances[0];

        await _cameraController.Ready(cancellation);
        await _aircraftController.SetModel(_aircraftModelPrefab, cancellation);

        _cameraController.SetFollowTarget(_aircraftController.transform);

        _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
            .Subscribe(c => _aircraftController.Pitch(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
            .Subscribe(c => _aircraftController.Roll(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
            .Subscribe(c => _aircraftController.Yaw(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
            .Subscribe(c => _aircraftController.Throttle(c.ReadValue<float>()))
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
