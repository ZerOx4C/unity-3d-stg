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
    public BulletBehaviour bulletPrefab;
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

        builder.Register<BattleCameraController>(Lifetime.Singleton)
            .WithParameter(cameraBehaviourPrefab);

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftControllerPrefab)
            .WithParameter(aircraftModelPrefab)
            .WithParameter(bulletPrefab);
    }
}
