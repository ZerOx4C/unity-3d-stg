using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public class BattleCameraController
{
    private readonly BattleCameraBehaviour _cameraBehaviourPrefab;
    private BattleCameraBehaviour _cameraBehaviour;

    [Inject]
    public BattleCameraController(BattleCameraBehaviour cameraBehaviourPrefab)
    {
        _cameraBehaviourPrefab = cameraBehaviourPrefab;
    }

    public async UniTask Ready(CancellationToken cancellation)
    {
        var instances = await Object.InstantiateAsync(_cameraBehaviourPrefab);
        _cameraBehaviour = instances[0];
    }

    public void SetFollowTarget(Transform target)
    {
        _cameraBehaviour.followCamera.Target.TrackingTarget = target;
    }
}
