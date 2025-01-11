using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
    [SerializeField] private GameObject _aircraftControllerPrefab;
    [SerializeField] private GameObject _aircraftModelPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<BattleEntryPoint>()
            .WithParameter("aircraftControllerPrefab", _aircraftControllerPrefab)
            .WithParameter("aircraftModelPrefab", _aircraftModelPrefab);
    }
}
