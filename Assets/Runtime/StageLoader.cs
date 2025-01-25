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
    private readonly TargetModel _targetModelPrefab;
    private AircraftModel _playerAircraftModelPrefab;

    [Inject]
    public StageLoader(
        AircraftBehaviour aircraftBehaviourPrefab,
        TargetModel targetModelPrefab)
    {
        _aircraftBehaviourPrefab = aircraftBehaviourPrefab;
        _targetModelPrefab = targetModelPrefab;
    }

    public void SetPlayerAircraftModelPrefab(AircraftModel aircraftModelPrefab)
    {
        _playerAircraftModelPrefab = aircraftModelPrefab;
    }

    public async UniTask<Result> LoadAsync(StageLayout stageLayoutPrefab, CancellationToken cancellation)
    {
        var stageLayout = await Instantiator.Create(stageLayoutPrefab)
            .InstantiateAsync(cancellation)
            .First();

        var playerAircraft = (await InstantiateWithLocatorAsync(_aircraftBehaviourPrefab, new[] { stageLayout.PlayerLocator }, cancellation))[0];
        var enemyAircrafts = await InstantiateWithLocatorAsync(_aircraftBehaviourPrefab, stageLayout.EnemyAircraftLocators, cancellation);
        await InstantiateWithLocatorAsync(_targetModelPrefab, stageLayout.TargetLocators, cancellation);

        var loadTasks = new Stack<UniTask>();
        loadTasks.Push(playerAircraft.LoadModelAsync(_playerAircraftModelPrefab, cancellation));

        for (var i = 0; i < enemyAircrafts.Length; i++)
        {
            var aircraft = enemyAircrafts[i];
            var locator = stageLayout.EnemyAircraftLocators[i];
            loadTasks.Push(aircraft.LoadModelAsync(locator.modelPrefab, cancellation));
        }

        await UniTask.WhenAll(loadTasks);
        return new Result
        {
            EnemyAircrafts = enemyAircrafts,
            PlayerAircraft = playerAircraft,
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
    }
}
