using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
    [SerializeField] private BattleCameraBehaviour _cameraBehaviourPrefab;
    [SerializeField] private GameObject _aircraftControllerPrefab;
    [SerializeField] private GameObject _aircraftModelPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<BattleCameraController>()
            .WithParameter(_cameraBehaviourPrefab)
            .AsSelf();

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter("aircraftControllerPrefab", _aircraftControllerPrefab)
            .WithParameter("aircraftModelPrefab", _aircraftModelPrefab);
    }
}
