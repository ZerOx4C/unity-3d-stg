using System;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

public class BattleEntryPoint : IAsyncStartable, IDisposable
{
    private readonly AircraftBehaviour _aircraftBehaviourPrefab;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BulletBehaviour _bulletPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerAircraftController _playerAircraftController;
    private AircraftBehaviour _aircraftBehaviour;

    [Inject]
    public BattleEntryPoint(
        AircraftBehaviour aircraftBehaviourPrefab,
        GameObject aircraftModelPrefab,
        BulletBehaviour bulletPrefab,
        BattleCameraController cameraController,
        PlayerAircraftController playerAircraftController)
    {
        _aircraftBehaviourPrefab = aircraftBehaviourPrefab;
        _aircraftModelPrefab = aircraftModelPrefab;
        _bulletPrefab = bulletPrefab;
        _cameraController = cameraController;
        _playerAircraftController = playerAircraftController;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        _aircraftBehaviour = await Utility.InstantiateAsync(_aircraftBehaviourPrefab, cancellationToken: cancellation);

        await _cameraController.ReadyAsync(cancellation);
        await _aircraftBehaviour.Loader.LoadAsync(_aircraftModelPrefab, cancellation);

        _cameraController.SetFollowTarget(_aircraftBehaviour.transform);
        _playerAircraftController.Initialize(_aircraftBehaviour);

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
