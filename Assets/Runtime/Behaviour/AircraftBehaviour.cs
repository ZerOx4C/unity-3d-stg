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

        public ModelLoader.Loader<AircraftModel> ModelLoader { get; private set; }
        public AircraftMovement Movement { get; private set; }
        public Observable<Transform> OnFire => _onFire;

        private void Awake()
        {
            ModelLoader = GetComponent<ModelLoader>().CreateLoader<AircraftModel>();
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
            // TODO: ダメージを受けて適宜破壊イベント発行？
            // ビューと混ざってきてる感じがある
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
