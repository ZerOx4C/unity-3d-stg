using System;
using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Controller;
using Cysharp.Threading.Tasks;
using Input;
using Model;
using Presenter;
using R3;
using Stage;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntryPoint : IAsyncStartable, ITickable, IDisposable
{
    private readonly AircraftInput _aircraftInput;
    private readonly AircraftPresenter _aircraftPresenter;
    private readonly BattleCameraController _cameraController;
    private readonly CollisionController _collisionController;
    private readonly CompositeDisposable _disposables = new();
    private readonly List<EnemyAircraftController> _enemyAircraftControllers = new();
    private readonly FireController _fireController;
    private readonly FragmentController _fragmentController;
    private readonly AircraftModel _playerAircraftModelPrefab;
    private readonly StageLayout _stageLayoutPrefab;
    private readonly StageLoader _stageLoader;
    private readonly TargetPresenter _targetPresenter;
    private bool _initialized;
    private AircraftBehaviour _playerAircraft;
    private PlayerAircraftController _playerAircraftController;

    [Inject]
    public BattleEntryPoint(
        AircraftPresenter aircraftPresenter,
        AircraftModel playerAircraftModelPrefab,
        BattleCameraController cameraController,
        CollisionController collisionController,
        FireController fireController,
        FragmentController fragmentController,
        AircraftInput aircraftInput,
        StageLayout stageLayoutPrefab,
        StageLoader stageLoader,
        TargetPresenter targetPresenter)
    {
        _aircraftPresenter = aircraftPresenter;
        _playerAircraftModelPrefab = playerAircraftModelPrefab;
        _cameraController = cameraController;
        _collisionController = collisionController;
        _fireController = fireController;
        _fragmentController = fragmentController;
        _aircraftInput = aircraftInput;
        _stageLayoutPrefab = stageLayoutPrefab;
        _stageLoader = stageLoader;
        _targetPresenter = targetPresenter;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        await _fireController.ReadyAsync(cancellation);
        _fragmentController.Ready();

        _stageLoader.SetPlayerAircraftModelPrefab(_playerAircraftModelPrefab);
        var loadResult = await _stageLoader.LoadAsync(_stageLayoutPrefab, cancellation);

        var playerAircraft = loadResult.PlayerAircraft;
        _playerAircraftController = new PlayerAircraftController(_aircraftInput);
        _playerAircraftController.Initialize(playerAircraft);
        _aircraftPresenter.Add(playerAircraft);
        playerAircraft.Ready();

        foreach (var aircraft in loadResult.EnemyAircrafts)
        {
            var controller = new EnemyAircraftController(aircraft);
            _aircraftPresenter.Add(aircraft);
            aircraft.Ready();

            _enemyAircraftControllers.Add(controller);
        }

        foreach (var target in loadResult.Targets)
        {
            _targetPresenter.Add(target);
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

        _collisionController.Tick();

        DebugHud.Log("speed", $"speed:{Vector3.Dot(_playerAircraft.transform.forward, _playerAircraft.Movement.LinearVelocity):f2}");
        DebugHud.Log("altitude", $"altitude:{_playerAircraft.transform.position.y:f2}");
    }
}
