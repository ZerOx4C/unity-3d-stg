using System;
using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Controller;
using Cysharp.Threading.Tasks;
using Input;
using Model;
using R3;
using Stage;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntryPoint : IAsyncStartable, ITickable, IDisposable
{
    private readonly AircraftInput _aircraftInput;
    private readonly BulletBehaviour _bulletPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private readonly List<EnemyAircraftController> _enemyAircraftControllers = new();
    private readonly AircraftModel _playerAircraftModelPrefab;
    private readonly StageLoader _stageLoader;
    private readonly StageLayout _stageLayoutPrefab;
    private bool _initialized;
    private AircraftBehaviour _playerAircraft;
    private PlayerAircraftController _playerAircraftController;

    [Inject]
    public BattleEntryPoint(
        AircraftModel playerAircraftModelPrefab,
        BulletBehaviour bulletPrefab,
        BattleCameraController cameraController,
        AircraftInput aircraftInput,
        StageLayout stageLayoutPrefab,
        StageLoader stageLoader)
    {
        _playerAircraftModelPrefab = playerAircraftModelPrefab;
        _bulletPrefab = bulletPrefab;
        _cameraController = cameraController;
        _aircraftInput = aircraftInput;
        _stageLayoutPrefab = stageLayoutPrefab;
        _stageLoader = stageLoader;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        _stageLoader.SetPlayerAircraftModelPrefab(_playerAircraftModelPrefab);
        var loadResult = await _stageLoader.LoadAsync(_stageLayoutPrefab, cancellation);

        var playerAircraft = loadResult.PlayerAircraft;
        _playerAircraftController = new PlayerAircraftController(playerAircraft, _aircraftInput, _bulletPrefab);
        _playerAircraftController.Initialize();
        playerAircraft.Ready();

        foreach (var aircraft in loadResult.EnemyAircrafts)
        {
            var controller = new EnemyAircraftController(aircraft, _bulletPrefab);
            controller.Initialize();
            aircraft.Ready();

            _enemyAircraftControllers.Add(controller);
            _disposables.Add(controller);
        }

        await _cameraController.ReadyAsync(cancellation);
        _cameraController.SetFollowTarget(playerAircraft.transform);

        DebugHud.OnReset
            .Subscribe(_ =>
            {
                playerAircraft.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                playerAircraft.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            })
            .AddTo(_disposables);

        _playerAircraft = playerAircraft;
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

        foreach (var controller in _enemyAircraftControllers)
        {
            controller.Tick();
        }

        DebugHud.Log("speed", $"speed:{Vector3.Dot(_playerAircraft.transform.forward, _playerAircraft.Movement.LinearVelocity):f2}");
        DebugHud.Log("altitude", $"altitude:{_playerAircraft.transform.position.y:f2}");
    }
}
