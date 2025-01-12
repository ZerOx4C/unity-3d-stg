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

        await UniTask.WaitUntil(() => _cameraController.Ready, cancellationToken: cancellation);
        _cameraController.SetFollowTarget(_aircraftController.transform);

        await _aircraftController.SetModel(_aircraftModelPrefab, cancellation);

        _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
            .Subscribe(ctx =>
            {
                _aircraftController.Pitch = ctx.ReadValue<float>();
                DebugHud.Log("Pitch", $"Pitch:{_aircraftController.Pitch}");
            })
            .AddTo(_disposables);

        _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
            .Subscribe(ctx =>
            {
                _aircraftController.Roll = ctx.ReadValue<float>();
                DebugHud.Log("Roll", $"Roll:{_aircraftController.Roll}");
            })
            .AddTo(_disposables);

        _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
            .Subscribe(ctx =>
            {
                _aircraftController.Yaw = ctx.ReadValue<float>();
                DebugHud.Log("Yaw", $"Yaw:{_aircraftController.Yaw}");
            })
            .AddTo(_disposables);

        _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
            .Subscribe(ctx =>
            {
                _aircraftController.Throttle = ctx.ReadValue<float>();
                DebugHud.Log("Throttle", $"Throttle:{_aircraftController.Throttle}");
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
