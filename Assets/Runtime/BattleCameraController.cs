using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

public class BattleCameraController : IAsyncStartable
{
    private readonly BattleCameraBehaviour _cameraBehaviourPrefab;
    private BattleCameraBehaviour _cameraBehaviour;

    [Inject]
    public BattleCameraController(BattleCameraBehaviour cameraBehaviourPrefab)
    {
        _cameraBehaviourPrefab = cameraBehaviourPrefab;
    }

    public bool Ready { get; private set; }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var instances = await Object.InstantiateAsync(_cameraBehaviourPrefab);
        _cameraBehaviour = instances[0];

        Ready = true;
    }

    public void SetFollowTarget(Transform target)
    {
        _cameraBehaviour.followCamera.Target.TrackingTarget = target;
    }
}
