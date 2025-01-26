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

        public async UniTask BreakAsync(IFragmentsOwner owner, CancellationToken cancellation)
        {
            var transforms = owner.Fragments;
            var fragments = await _fragmentInstantiator
                .SetTransforms(transforms)
                .InstantiateAsync(cancellation).All;

            for (var i = 0; i < transforms.Count; i++)
            {
                var fragment = fragments[i];
                fragment.lifetime = 5;
                fragment.Rigidbody.linearVelocity = owner.RigidbodyReader.LinearVelocity;
                fragment.Rigidbody.angularVelocity = owner.RigidbodyReader.AngularVelocity;
                fragment.Rigidbody.AddForce(50 * Random.insideUnitSphere, ForceMode.Impulse);
                fragment.Rigidbody.AddTorque(5 * Random.insideUnitSphere, ForceMode.Impulse);
                transforms[i].SetParent(fragment.transform, true);
            }

            Object.Destroy(owner.gameObject);
        }
    }
}
