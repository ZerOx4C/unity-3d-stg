using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using Model;
using Stage;
using UnityEngine;
using Utility;
using VContainer;
using Object = UnityEngine.Object;

public class StageLoader
{
    private readonly AircraftBehaviour _aircraftBehaviourPrefab;
    private readonly TargetBehaviour _targetBehaviourPrefab;
    private AircraftModel _playerAircraftModelPrefab;

    [Inject]
    public StageLoader(
        AircraftBehaviour aircraftBehaviourPrefab,
        TargetBehaviour targetBehaviourPrefab)
    {
        _aircraftBehaviourPrefab = aircraftBehaviourPrefab;
        _targetBehaviourPrefab = targetBehaviourPrefab;
    }

    public void SetPlayerAircraftModelPrefab(AircraftModel aircraftModelPrefab)
    {
        _playerAircraftModelPrefab = aircraftModelPrefab;
    }

    public async UniTask<Result> LoadAsync(StageLayout stageLayoutPrefab, CancellationToken cancellation)
    {
        var stageLayout = await Instantiator.Create(stageLayoutPrefab)
            .InstantiateAsync(cancellation).First;

        var playerAircraft = (await InstantiateWithLocatorAsync(_aircraftBehaviourPrefab, new[] { stageLayout.PlayerLocator }, cancellation))[0];
        var enemyAircrafts = await InstantiateWithLocatorAsync(_aircraftBehaviourPrefab, stageLayout.EnemyAircraftLocators, cancellation);
        var targets = await InstantiateWithLocatorAsync(_targetBehaviourPrefab, stageLayout.TargetLocators, cancellation);

        var loadTasks = new Stack<UniTask>();
        loadTasks.Push(playerAircraft.LoadModelAsync(_playerAircraftModelPrefab, cancellation));

        for (var i = 0; i < enemyAircrafts.Length; i++)
        {
            var aircraft = enemyAircrafts[i];
            var locator = stageLayout.EnemyAircraftLocators[i];
            loadTasks.Push(aircraft.LoadModelAsync(locator.modelPrefab, cancellation));
        }

        for (var i = 0; i < targets.Length; i++)
        {
            var target = targets[i];
            var locator = stageLayout.TargetLocators[i];
            loadTasks.Push(target.LoadModelAsync(locator.modelPrefab, cancellation));
        }

        await UniTask.WhenAll(loadTasks);
        return new Result
        {
            EnemyAircrafts = enemyAircrafts,
            PlayerAircraft = playerAircraft,
            Targets = targets,
        };
    }

    private static async UniTask<T[]> InstantiateWithLocatorAsync<T>(T prefab, IEnumerable<MonoBehaviour> locators,
        CancellationToken cancellation) where T : Object
    {
        var locatorArray = locators.ToArray();
        var positions = new Vector3[locatorArray.Length];
        var rotations = new Quaternion[locatorArray.Length];

        for (var i = 0; i < locatorArray.Length; i++)
        {
            locatorArray[i].transform.GetPositionAndRotation(out positions[i], out rotations[i]);
        }

        return await Object.InstantiateAsync(prefab, locatorArray.Length, null,
            new ReadOnlySpan<Vector3>(positions), new ReadOnlySpan<Quaternion>(rotations), cancellation);
    }

    public class Result
    {
        public IEnumerable<AircraftBehaviour> EnemyAircrafts;
        public AircraftBehaviour PlayerAircraft;
        public IEnumerable<TargetBehaviour> Targets;
    }
}
