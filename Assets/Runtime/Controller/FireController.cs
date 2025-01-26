using System.Collections.Generic;
using System.Threading;
using Behaviour;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Utility;
using VContainer;

namespace Controller
{
    public class FireController
    {
        private readonly Instantiator.Config<BulletBehaviour> _bulletInstantiator;
        private readonly Stack<BulletBehaviour> _bulletPool = new();
        private readonly CompositeDisposable _disposable = new();
        private int _bulletCount;
        private Transform _root;
        private bool _warming;

        [Inject]
        public FireController(BulletBehaviour bulletBehaviourPrefab)
        {
            _bulletInstantiator = Instantiator.Create(bulletBehaviourPrefab);
        }

        public async UniTask ReadyAsync(CancellationToken cancellation)
        {
            _root = new GameObject().transform;
            _root.name = "Bullets";

            _bulletInstantiator.SetParent(_root);

            await WarmAsync(10, cancellation);
        }

        public void Fire(AircraftBehaviour owner, Transform gun)
        {
            var bullet = ObtainBullet();
            bullet.Movement.Initialize(owner.Movement, gun, 100);
            bullet.Initialize();
        }

        private async UniTask WarmAsync(int count, CancellationToken cancellation)
        {
            if (_warming)
            {
                return;
            }

            _warming = true;

            var bullets = await _bulletInstantiator
                .SetTransforms(count, Vector3.zero, Quaternion.identity)
                .InstantiateAsync(cancellation).All;

            foreach (var bullet in bullets)
            {
                InitializeBullet(bullet);
                _bulletPool.Push(bullet);
            }

            _warming = false;
        }

        private BulletBehaviour ObtainBullet()
        {
            if (_bulletPool.TryPop(out var bullet))
            {
                bullet.gameObject.SetActive(true);
            }
            else
            {
                bullet = _bulletInstantiator.Instantiate().First;
                InitializeBullet(bullet);
            }

            if (_bulletPool.Count < 20)
            {
                WarmAsync(10, CancellationToken.None).Forget();
            }

            return bullet;
        }

        private void InitializeBullet(BulletBehaviour bullet)
        {
            bullet.name = $"Bullet_{++_bulletCount}";
            bullet.OnRelease
                .Subscribe(_ =>
                {
                    bullet.gameObject.SetActive(false);
                    _bulletPool.Push(bullet);
                })
                .AddTo(_disposable);
        }
    }
}
