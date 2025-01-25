using System.Threading;
using Cysharp.Threading.Tasks;
using Model;
using Movement;
using R3;
using UnityEngine;
using Utility;

namespace Behaviour
{
    public class AircraftBehaviour : MonoBehaviour
    {
        private bool _fire;
        private float _fireCooldown;
        private AircraftModel _model;
        private int _nextGunIndex;
        private Subject<Transform> _onFire;
        private Rigidbody _rigidbody;

        public AircraftMovement Movement { get; private set; }
        public Observable<Transform> OnFire => _onFire;

        private void Awake()
        {
            Movement = GetComponent<AircraftMovement>();
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

        public async UniTask LoadModelAsync(AircraftModel modelPrefab, CancellationToken cancellation)
        {
            if (_model is not null)
            {
                Destroy(_model.gameObject);
                _model = null;
            }

            _model = await Instantiator.Create(modelPrefab)
                .SetParent(transform)
                .SetTransforms(transform)
                .InstantiateAsync(cancellation)
                .First();
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

        private void UpdatePropeller()
        {
            if (_model is null)
            {
                return;
            }

            _model.PropellerSpeed = 3600 * Movement.Throttle;
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

            var gun = _model.Guns[_nextGunIndex++];
            _nextGunIndex %= _model.Guns.Count;
            _onFire.OnNext(gun);
        }
    }
}
