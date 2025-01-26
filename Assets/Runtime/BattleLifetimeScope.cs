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
    public TargetModel targetModelPrefab;

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

        builder.Register<FireController>(Lifetime.Singleton)
            .WithParameter(bulletPrefab);

        builder.Register<BattleCameraController>(Lifetime.Singleton)
            .WithParameter(cameraBehaviourPrefab);

        builder.Register<PlayerAircraftController>(Lifetime.Singleton);

        builder.Register<StageLoader>(Lifetime.Singleton)
            .WithParameter(aircraftBehaviourPrefab)
            .WithParameter(targetModelPrefab);

        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter(aircraftBehaviourPrefab)
            .WithParameter(playerAircraftModelPrefab)
            .WithParameter(stageLayoutPrefab)
            .WithParameter(bulletPrefab);
    }
}
