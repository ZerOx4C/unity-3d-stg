using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
    public BattleCameraBehaviour cameraBehaviourPrefab;
    public AircraftController aircraftControllerPrefab;
    public GameObject aircraftModelPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<BattleCameraController>()
            .WithParameter(cameraBehaviourPrefab)
            .AsSelf();

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftControllerPrefab)
            .WithParameter(aircraftModelPrefab);
    }
}
