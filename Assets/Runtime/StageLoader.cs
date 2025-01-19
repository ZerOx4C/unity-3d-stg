using System;
using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public class StageLoader
{
    private readonly AircraftBehaviour _aircraftBehaviourPrefab;
    private AircraftModel _playerAircraftModelPrefab;

    [Inject]
    public StageLoader(AircraftBehaviour aircraftBehaviourPrefab)
    {
        _aircraftBehaviourPrefab = aircraftBehaviourPrefab;
    }

    public void SetPlayerAircraftModelPrefab(AircraftModel aircraftModelPrefab)
    {
        _playerAircraftModelPrefab = aircraftModelPrefab;
    }

    public async UniTask<Result> LoadAsync(GameObject stagePrefab, CancellationToken cancellation)
    {
        var result = new Result();

        var stage = await Utility.InstantiateAsync(stagePrefab, cancellationToken: cancellation);

        var locators = stage.transform.GetComponentsInChildren<SpawnLocator>();
        var positions = new Vector3[locators.Length];
        var rotations = new Quaternion[locators.Length];

        for (var i = 0; i < locators.Length; i++)
        {
            locators[i].transform.GetPositionAndRotation(out positions[i], out rotations[i]);
        }

        var aircrafts = await Object.InstantiateAsync(_aircraftBehaviourPrefab, locators.Length, null,
            new ReadOnlySpan<Vector3>(positions), new ReadOnlySpan<Quaternion>(rotations), cancellation);

        var loadTasks = new UniTask[aircrafts.Length];

        for (var i = 0; i < aircrafts.Length; i++)
        {
            var aircraft = aircrafts[i];
            var locator = locators[i];

            if (locator.isPlayer)
            {
                loadTasks[i] = aircraft.LoadModelAsync(_playerAircraftModelPrefab, cancellation);
                result.PlayerAircraft = aircraft;
            }
            else
            {
                loadTasks[i] = aircraft.LoadModelAsync(locator.modelPrefab, cancellation);
                result.EnemyAircrafts.Add(aircraft);
            }
        }

        await UniTask.WhenAll(loadTasks);
        return result;
    }

    public class Result
    {
        public List<AircraftBehaviour> EnemyAircrafts = new();
        public AircraftBehaviour PlayerAircraft;
    }
}
