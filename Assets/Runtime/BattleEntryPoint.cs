using System;
using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Controller;
using Cysharp.Threading.Tasks;
using Input;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntryPoint : IAsyncStartable, ITickable, IDisposable
{
    private readonly AircraftInput _aircraftInput;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BulletBehaviour _bulletPrefab;
    private readonly BattleCameraController _cameraController;
    private readonly CompositeDisposable _disposables = new();
    private readonly List<EnemyAircraftController> _enemyAircraftControllers = new();
    private readonly StageLoader _stageLoader;
    private readonly GameObject _stagePrefab;
    private bool _initialized;
    private AircraftBehaviour _playerAircraft;
    private PlayerAircraftController _playerAircraftController;

    [Inject]
    public BattleEntryPoint(
        GameObject aircraftModelPrefab,
        BulletBehaviour bulletPrefab,
        BattleCameraController cameraController,
        AircraftInput aircraftInput,
        GameObject stagePrefab,
        StageLoader stageLoader)
    {
        _aircraftModelPrefab = aircraftModelPrefab;
        _bulletPrefab = bulletPrefab;
        _cameraController = cameraController;
        _aircraftInput = aircraftInput;
        _stagePrefab = stagePrefab;
        _stageLoader = stageLoader;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        _stageLoader.SetPlayerAircraftModelPrefab(_aircraftModelPrefab);
        var loadResult = await _stageLoader.LoadAsync(_stagePrefab, cancellation);

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
