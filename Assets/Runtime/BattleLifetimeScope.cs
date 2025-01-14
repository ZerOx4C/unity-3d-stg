using Behaviour;
using Controller;
using Input;
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
    public GameObject stagePrefab;
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
        builder.Register<AircraftInput>(Lifetime.Singleton);

        builder.Register<BattleCameraController>(Lifetime.Singleton)
            .WithParameter(cameraBehaviourPrefab);

        builder.Register<PlayerAircraftController>(Lifetime.Singleton);

        builder.Register<StageLoader>(Lifetime.Singleton)
            .WithParameter(aircraftBehaviourPrefab);

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftBehaviourPrefab)
            .WithParameter("aircraftModelPrefab", aircraftModelPrefab)
            .WithParameter("stagePrefab", stagePrefab)
            .WithParameter(bulletPrefab);
    }
}
