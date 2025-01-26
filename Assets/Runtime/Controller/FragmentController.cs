using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;
using VContainer;

namespace Controller
{
    public class FragmentController
    {
        private readonly Instantiator.Config<FragmentBehaviour> _fragmentInstantiator;
        private Transform _root;

        [Inject]
        public FragmentController(FragmentBehaviour fragmentBehaviourPrefab)
        {
            _fragmentInstantiator = Instantiator.Create(fragmentBehaviourPrefab);
        }

        public void Ready()
        {
            _root = new GameObject().transform;
            _root.name = "Fragments";

            _fragmentInstantiator.SetParent(_root);
        }

        public void Break(TargetBehaviour target)
        {
            BreakAsync(target.gameObject, target.ModelLoader.Model.Fragments, CancellationToken.None).Forget();
        }

        private async UniTask BreakAsync(GameObject owner, IReadOnlyList<Transform> transforms, CancellationToken cancellation)
        {
            var fragments = await _fragmentInstantiator
                .SetTransforms(transforms)
                .InstantiateAsync(cancellation).All;

            for (var i = 0; i < transforms.Count; i++)
            {
                var fragment = fragments[i];
                transforms[i].SetParent(fragment.transform, true);

                fragment.lifetime = 5;
                fragment.Rigidbody.AddForce(50 * Random.insideUnitSphere, ForceMode.Impulse);
                fragment.Rigidbody.AddTorque(5 * Random.insideUnitSphere, ForceMode.Impulse);
            }

            Object.Destroy(owner);
        }
    }
}
