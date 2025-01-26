using Behaviour;
using Controller;
using Input;
using Model;
using Stage;
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
    public StageLayout stageLayoutPrefab;
    public AircraftModel playerAircraftModelPrefab;
    public TargetBehaviour targetBehaviourPrefab;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        Instantiate(debugHudPrefab);
#endif
    }

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(debugHudPrefab);
        builder.RegisterInstance(cameraBehaviourPrefab);
        builder.RegisterInstance(aircraftBehaviourPrefab);
        builder.RegisterInstance(bulletPrefab);
        builder.RegisterInstance(stageLayoutPrefab);
        builder.RegisterInstance(playerAircraftModelPrefab);
        builder.RegisterInstance(targetBehaviourPrefab);

        builder.Register<AircraftInput>(Lifetime.Singleton);
        builder.Register<FireController>(Lifetime.Singleton);
        builder.Register<BattleCameraController>(Lifetime.Singleton);
        builder.Register<PlayerAircraftController>(Lifetime.Singleton);
        builder.Register<TargetController>(Lifetime.Singleton);
        builder.Register<CollisionController>(Lifetime.Singleton);
        builder.Register<StageLoader>(Lifetime.Singleton);
        builder.RegisterEntryPoint<BattleEntryPoint>();
    }
}
