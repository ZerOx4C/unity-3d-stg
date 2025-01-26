using System.Collections.Generic;
using Model;
using R3;
using UnityEngine;
using Utility;

namespace Behaviour
{
    public class TargetBehaviour : MonoBehaviour, IFragmentsOwner
    {
        private Subject<Unit> _onDead;
        public ModelLoader.Loader<TargetModel> ModelLoader { get; private set; }
        public Observable<Unit> OnDead => _onDead;

        private void Awake()
        {
            ModelLoader = GetComponent<ModelLoader>().CreateLoader<TargetModel>();
            RigidbodyReader = new RigidbodyReader(GetComponent<Rigidbody>());
            _onDead = new Subject<Unit>();
        }

        private void OnDestroy()
        {
            _onDead.Dispose();
        }

        public RigidbodyReader RigidbodyReader { get; private set; }

        public IReadOnlyList<Transform> Fragments => ModelLoader.Model.Fragments;

        public void Damage()
        {
            _onDead.OnNext(Unit.Default);
        }
    }
}
