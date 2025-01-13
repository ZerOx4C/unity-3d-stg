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

public class BattleEntryPoint : IAsyncStartable, ITickable, IDisposable
{
    private readonly AircraftBehaviour _aircraftBehaviourPrefab;
    private readonly AircraftInput _aircraftInput;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BulletBehaviour _bulletPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private EnemyAircraftController _enemyAircraftController;
    private bool _initialized;
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
        var playerAircraft = await Utility.InstantiateAsync(_aircraftBehaviourPrefab, cancellationToken: cancellation);
        var enemyAircraft = await Utility.InstantiateAsync(_aircraftBehaviourPrefab,
            new Vector3(0, 0, 100), Quaternion.identity, cancellationToken: cancellation);

        await _cameraController.ReadyAsync(cancellation);
        _cameraController.SetFollowTarget(playerAircraft.transform);

        await playerAircraft.Loader.LoadAsync(_aircraftModelPrefab, cancellation);
        await enemyAircraft.Loader.LoadAsync(_aircraftModelPrefab, cancellation);

        playerAircraft.OnFire
            .Subscribe(gun => Fire(playerAircraft, gun))
            .AddTo(_disposables);

        enemyAircraft.OnFire
            .Subscribe(gun => Fire(enemyAircraft, gun))
            .AddTo(_disposables);

        _playerAircraftController = new PlayerAircraftController(playerAircraft, _aircraftInput);
        _playerAircraftController.Initialize();

        _enemyAircraftController = new EnemyAircraftController(enemyAircraft);
        _enemyAircraftController.Initialize();

        playerAircraft.Ready();
        enemyAircraft.Ready();

        DebugHud.OnReset
            .Subscribe(_ =>
            {
                playerAircraft.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                playerAircraft.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            })
            .AddTo(_disposables);

        _initialized = true;
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _playerAircraftController.Dispose();
    }

    public void Tick()
    {
        if (!_initialized)
        {
            return;
        }

        _aircraftInput.Tick();
        _enemyAircraftController.Tick();
    }

    private void Fire(AircraftBehaviour aircraft, Transform gun)
    {
        var bullet = Object.Instantiate(_bulletPrefab);
        bullet.Movement.Initialize(aircraft.Movement, gun, 100);
        bullet.Initialize();
    }
}
