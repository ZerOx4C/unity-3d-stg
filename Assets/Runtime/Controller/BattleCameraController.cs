using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;
using VContainer;

namespace Controller
{
    public class BattleCameraController
    {
        private readonly BattleCameraBehaviour _cameraBehaviourPrefab;
        private BattleCameraBehaviour _cameraBehaviour;

        [Inject]
        public BattleCameraController(BattleCameraBehaviour cameraBehaviourPrefab)
        {
            _cameraBehaviourPrefab = cameraBehaviourPrefab;
        }

        public async UniTask ReadyAsync(CancellationToken cancellation)
        {
            _cameraBehaviour = await Instantiator.Create(_cameraBehaviourPrefab)
                .InstantiateAsync(cancellation)
                .First();
        }

        public void SetFollowTarget(Transform target)
        {
            _cameraBehaviour.followCamera.Target.TrackingTarget = target;
        }
    }
}
