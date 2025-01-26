using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using Model;
using Stage;
using Utility;
using VContainer;

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

        var playerAircraftTask = Instantiator.Create(_aircraftBehaviourPrefab)
            .SetTransforms(stageLayout.PlayerLocator.transform)
            .InstantiateAsync(cancellation).First;

        var enemyAircraftsTask = Instantiator.Create(_aircraftBehaviourPrefab)
            .SetTransforms(stageLayout.EnemyAircraftLocators.Select(l => l.transform).ToList())
            .InstantiateAsync(cancellation).All;

        var targetsTask = Instantiator.Create(_targetBehaviourPrefab)
            .SetTransforms(stageLayout.TargetLocators.Select(l => l.transform).ToList())
            .InstantiateAsync(cancellation).All;

        var loadTasks = new Stack<UniTask>();

        var playerAircraft = await playerAircraftTask;
        loadTasks.Push(playerAircraft.LoadModelAsync(_playerAircraftModelPrefab, cancellation));

        var enemyAircrafts = await enemyAircraftsTask;
        for (var i = 0; i < enemyAircrafts.Length; i++)
        {
            var aircraft = enemyAircrafts[i];
            var locator = stageLayout.EnemyAircraftLocators[i];
            loadTasks.Push(aircraft.LoadModelAsync(locator.modelPrefab, cancellation));
        }

        var targets = await targetsTask;
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

    public class Result
    {
        public IEnumerable<AircraftBehaviour> EnemyAircrafts;
        public AircraftBehaviour PlayerAircraft;
        public IEnumerable<TargetBehaviour> Targets;
    }
}
