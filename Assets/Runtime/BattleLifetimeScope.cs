using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
#if UNITY_EDITOR
    public DebugHud debugHudPrefab;
#endif
    public BattleCameraBehaviour cameraBehaviourPrefab;
    public AircraftController aircraftControllerPrefab;
    public GameObject aircraftModelPrefab;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        Instantiate(debugHudPrefab);
#endif
    }

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<AircraftInput>()
            .AsSelf();

        builder.RegisterEntryPoint<BattleCameraController>()
            .WithParameter(cameraBehaviourPrefab)
            .AsSelf();

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftControllerPrefab)
            .WithParameter(aircraftModelPrefab);
    }
}
