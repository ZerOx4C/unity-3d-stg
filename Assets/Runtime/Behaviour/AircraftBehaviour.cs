using Model;
using Movement;
using R3;
using UnityEngine;

namespace Behaviour
{
    public class AircraftBehaviour : MonoBehaviour
    {
        private bool _fire;
        private float _fireCooldown;
        private int _nextGunIndex;
        private Subject<Transform> _onFire;
        private Rigidbody _rigidbody;

        public AircraftMovement Movement { get; private set; }
        public ModelLoader Loader { get; private set; }
        public Observable<Transform> OnFire => _onFire;

        private void Awake()
        {
            Movement = GetComponent<AircraftMovement>();
            Loader = GetComponent<ModelLoader>();
            _rigidbody = GetComponent<Rigidbody>();
            _onFire = new Subject<Transform>();

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

        private void UpdatePropeller()
        {
            if (Loader.Model is null)
            {
                return;
            }

            Loader.Model.PropellerSpeed = 3600 * Movement.Throttle;
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

            var gun = Loader.Guns[_nextGunIndex++];
            _nextGunIndex %= Loader.Guns.Count;
            _onFire.OnNext(gun);
        }

        public void Ready()
        {
            Movement.enabled = true;
            _rigidbody.isKinematic = false;
        }

        public void Fire(bool active)
        {
            _fire = active;
        }
    }
}
