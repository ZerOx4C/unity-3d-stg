using System;
using System.Collections.Generic;
using Behaviour;
using UnityEngine;

namespace Controller
{
    public class EnemyAircraftController : AircraftControllerBase
    {
        private readonly Queue<(float delay, Action action)> _actionQueue = new();
        private float _wait;

        public EnemyAircraftController(
            AircraftBehaviour aircraft,
            BulletBehaviour bulletPrefab)
            : base(aircraft, bulletPrefab)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.Pitch(0);
                Aircraft.Movement.Roll(1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.Pitch(1);
                Aircraft.Movement.Roll(0);
            }));
            _actionQueue.Enqueue((6, () =>
            {
                Aircraft.Movement.Pitch(0);
                Aircraft.Movement.Roll(-1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.Pitch(1);
                Aircraft.Movement.Roll(0);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.Pitch(0);
                Aircraft.Movement.Roll(1);
            }));
        }

        public override void Tick()
        {
            if (0 < _wait)
            {
                _wait -= Time.deltaTime;
            }

            if (0 < _wait)
            {
                return;
            }

            var entry = _actionQueue.Dequeue();
            _actionQueue.Enqueue(entry);

            _wait += entry.delay;
            entry.action();
        }
    }
}
