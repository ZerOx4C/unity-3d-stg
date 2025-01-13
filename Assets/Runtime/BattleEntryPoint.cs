using System;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using Input;
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
    private PlayerAircraftController _playerAircraftController;

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
        _aircraftBehaviour = await Utility.InstantiateAsync(_aircraftBehaviourPrefab, cancellationToken: cancellation);

        await _cameraController.ReadyAsync(cancellation);
        await _aircraftBehaviour.Loader.LoadAsync(_aircraftModelPrefab, cancellation);

        _cameraController.SetFollowTarget(_aircraftBehaviour.transform);

        _aircraftBehaviour.OnFire
            .Subscribe(Fire)
            .AddTo(_disposables);

        _playerAircraftController = new PlayerAircraftController(_aircraftBehaviour, _aircraftInput);
        _playerAircraftController.Initialize();
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
