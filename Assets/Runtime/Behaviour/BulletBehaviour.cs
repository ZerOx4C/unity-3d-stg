using Movement;
using R3;
using UnityEngine;

namespace Behaviour
{
    public class BulletBehaviour : MonoBehaviour
    {
        public float lifetime = 1;

        private float _lifetime;
        private Subject<Unit> _onRelease;

        public AircraftBehaviour Owner { get; private set; }
        public BulletMovement Movement { get; private set; }
        public Observable<Unit> OnRelease => _onRelease;

        private void Awake()
        {
            _onRelease = new Subject<Unit>();
            Movement = GetComponent<BulletMovement>();
        }

        private void Update()
        {
            if (0 < _lifetime)
            {
                _lifetime -= Time.deltaTime;
            }
            else
            {
                _onRelease.OnNext(Unit.Default);
            }
        }

        private void OnDestroy()
        {
            _onRelease.Dispose();
        }

        public void Initialize(AircraftBehaviour owner)
        {
            _lifetime = lifetime;
            Owner = owner;
        }

        public void Hit()
        {
            _onRelease.OnNext(Unit.Default);
        }
    }
}
