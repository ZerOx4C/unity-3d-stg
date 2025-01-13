using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
#if UNITY_EDITOR
    public DebugHud debugHudPrefab;
#endif
    public BattleCameraBehaviour cameraBehaviourPrefab;
    public AircraftBehaviour aircraftBehaviourPrefab;
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

        builder.Register<PlayerAircraftController>(Lifetime.Singleton);

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftBehaviourPrefab)
            .WithParameter(aircraftModelPrefab)
            .WithParameter(bulletPrefab);
    }
}
