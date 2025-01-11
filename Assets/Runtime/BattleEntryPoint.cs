using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleEntryPoint : IAsyncStartable
{
    private readonly GameObject _aircraftControllerPrefab;
    private readonly GameObject _aircraftModelPrefab;
    private readonly BattleCameraController _cameraController;

    [Inject]
    public BattleEntryPoint(
        GameObject aircraftControllerPrefab,
        GameObject aircraftModelPrefab,
        BattleCameraController cameraController)
    {
        _aircraftControllerPrefab = aircraftControllerPrefab;
        _aircraftModelPrefab = aircraftModelPrefab;
        _cameraController = cameraController;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var instances = await Object.InstantiateAsync(_aircraftControllerPrefab, 1,
            null, Vector3.zero, Quaternion.identity, cancellation);

        var aircraftController = instances[0].GetComponent<AircraftController>();

        await aircraftController.SetModel(_aircraftModelPrefab, cancellation);

        await UniTask.WaitUntil(() => _cameraController.Ready, cancellationToken: cancellation);

        _cameraController.SetFollowTarget(aircraftController.transform);
    }
}
