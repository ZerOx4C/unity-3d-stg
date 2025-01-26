using System;
using System.Collections.Generic;
using Behaviour;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace Controller
{
    public class CollisionController : IDisposable
    {
        private readonly Queue<(BulletBehaviour bullet, AircraftBehaviour aircraft)> _aircraftHitQueue = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly Queue<(BulletBehaviour bullet, TargetBehaviour target)> _targetHitQueue = new();

        [Inject]
        public CollisionController()
        {
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void RegisterBullet(BulletBehaviour bullet)
        {
            bullet.OnTriggerEnterAsObservable()
                .Subscribe(c => OnHit(bullet, c))
                .AddTo(_disposables);
        }

        public void Tick()
        {
            var releaseBullets = new HashSet<BulletBehaviour>();
            var deadAircrafts = new HashSet<AircraftBehaviour>();
            var deadTargets = new HashSet<TargetBehaviour>();

            while (_aircraftHitQueue.TryDequeue(out var pair))
            {
                if (pair.bullet.Owner == pair.aircraft)
                {
                    continue;
                }

                releaseBullets.Add(pair.bullet);
                deadAircrafts.Add(pair.aircraft);
            }

            while (_targetHitQueue.TryDequeue(out var pair))
            {
                releaseBullets.Add(pair.bullet);
                deadTargets.Add(pair.target);
            }

            foreach (var bullet in releaseBullets)
            {
                bullet.Hit();
            }

            foreach (var aircraft in deadAircrafts)
            {
                aircraft.Damage();
            }

            foreach (var target in deadTargets)
            {
                target.Damage();
            }
        }

        private void OnHit(BulletBehaviour bullet, Collider collider)
        {
            var go = collider.attachedRigidbody is not null
                ? collider.attachedRigidbody.gameObject
                : collider.gameObject;

            if (go.TryGetComponent(out AircraftBehaviour aircraft))
            {
                _aircraftHitQueue.Enqueue((bullet, aircraft));
            }
            else if (go.TryGetComponent(out TargetBehaviour target))
            {
                _targetHitQueue.Enqueue((bullet, target));
            }
        }
    }
}
