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
    private readonly AircraftBehaviour _aircraftBehaviourPrefab;
    private readonly AircraftInput _aircraftInput;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BulletBehaviour _bulletPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private AircraftBehaviour _aircraftBehaviour;

    [Inject]
    public BattleEntryPoint(
        AircraftBehaviour aircraftBehaviourPrefab,
        GameObject aircraftModelPrefab,
        BulletBehaviour bulletPrefab,
        BattleCameraController cameraController,
        AircraftInput aircraftInput)
    {
        _aircraftBehaviourPrefab = aircraftBehaviourPrefab;
        _aircraftModelPrefab = aircraftModelPrefab;
        _bulletPrefab = bulletPrefab;
        _cameraController = cameraController;
        _aircraftInput = aircraftInput;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var instances = await Object.InstantiateAsync(_aircraftBehaviourPrefab, 1,
            null, Vector3.zero, Quaternion.identity, cancellation);

        _aircraftBehaviour = instances[0];

        await _cameraController.Ready(cancellation);
        await _aircraftBehaviour.Loader.LoadAsync(_aircraftModelPrefab, cancellation);

        _cameraController.SetFollowTarget(_aircraftBehaviour.transform);

        _aircraftInput.Pitch.OnProgress.Merge(_aircraftInput.Pitch.OnEnded)
            .Subscribe(c => _aircraftBehaviour.Movement.Pitch(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Roll.OnProgress.Merge(_aircraftInput.Roll.OnEnded)
            .Subscribe(c => _aircraftBehaviour.Movement.Roll(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Yaw.OnProgress.Merge(_aircraftInput.Yaw.OnEnded)
            .Subscribe(c => _aircraftBehaviour.Movement.Yaw(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Throttle.OnProgress.Merge(_aircraftInput.Throttle.OnEnded)
            .Subscribe(c => _aircraftBehaviour.Movement.Throttle(c.ReadValue<float>()))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnBegan
            .Subscribe(_ => _aircraftBehaviour.Fire(true))
            .AddTo(_disposables);

        _aircraftInput.Fire.OnEnded
            .Subscribe(_ => _aircraftBehaviour.Fire(false))
            .AddTo(_disposables);

        _aircraftBehaviour.OnFire
            .Subscribe(Fire)
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void Fire(Transform gun)
    {
        var bullet = Object.Instantiate(_bulletPrefab);
        bullet.Movement.Initialize(_aircraftBehaviour.Movement, gun, 100);
        bullet.Initialize();
    }
}
