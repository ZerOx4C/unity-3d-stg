using System.Collections.Generic;
using Model;
using Movement;
using R3;
using UnityEngine;
using Utility;

namespace Behaviour
{
    public class AircraftBehaviour : MonoBehaviour, IFragmentsOwner
    {
        private bool _fire;
        private float _fireCooldown;
        private int _nextGunIndex;
        private Subject<Unit> _onDead;
        private Subject<Transform> _onFire;
        private Rigidbody _rigidbody;

        public ModelLoader.Loader<AircraftModel> ModelLoader { get; private set; }
        public AircraftMovement Movement { get; private set; }
        public Observable<Transform> OnFire => _onFire;
        public Observable<Unit> OnDead => _onDead;

        private void Awake()
        {
            ModelLoader = GetComponent<ModelLoader>().CreateLoader<AircraftModel>();
            Movement = GetComponent<AircraftMovement>();
            _rigidbody = GetComponent<Rigidbody>();
            RigidbodyReader = new RigidbodyReader(_rigidbody);
            _onFire = new Subject<Transform>();
            _onDead = new Subject<Unit>();

            Movement.enabled = false;
            _rigidbody.isKinematic = true;
        }

        private void Update()
        {
            UpdatePropeller();
            UpdateFire();
        }

        private void OnDestroy()
        {
            _onFire.Dispose();
        }

        public IReadOnlyList<Transform> Fragments => ModelLoader.Model.Fragments;
        public RigidbodyReader RigidbodyReader { get; private set; }

        public void Ready()
        {
            Movement.enabled = true;
            _rigidbody.isKinematic = false;
        }

        public void Fire(bool active)
        {
            _fire = active;
        }

        public void Damage()
        {
            _onDead.OnNext(Unit.Default);
        }

        private void UpdatePropeller()
        {
            if (!ModelLoader.IsLoaded)
            {
                return;
            }

            ModelLoader.Model.PropellerSpeed = 3600 * Movement.Throttle;
        }

        private void UpdateFire()
        {
            if (0 < _fireCooldown)
            {
                _fireCooldown -= Time.deltaTime;
                return;
            }

            if (!_fire)
            {
                return;
            }

            _fireCooldown = 0.05f;

            var model = ModelLoader.Model;
            var gun = model.Guns[_nextGunIndex++];
            _nextGunIndex %= model.Guns.Count;
            _onFire.OnNext(gun);
        }
    }
}
