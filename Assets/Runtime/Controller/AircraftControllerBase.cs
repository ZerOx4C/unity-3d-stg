using System;
using Behaviour;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Controller
{
    public abstract class AircraftControllerBase : IDisposable
    {
        private readonly BulletBehaviour _bulletPrefab;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        protected AircraftControllerBase(
            AircraftBehaviour aircraft,
            BulletBehaviour bulletPrefab)
        {
            Aircraft = aircraft;
            _bulletPrefab = bulletPrefab;
        }

        protected AircraftBehaviour Aircraft { get; }

        public virtual void Dispose()
        {
            _disposables.Dispose();
        }

        public virtual void Initialize()
        {
            Aircraft.OnFire.Subscribe(OnFire).AddTo(_disposables);
        }

        public abstract void Tick();

        private void OnFire(Transform gun)
        {
            var bullet = Object.Instantiate(_bulletPrefab);
            bullet.Movement.Initialize(Aircraft.Movement, gun, 100);
            bullet.Initialize();
        }
    }
}
