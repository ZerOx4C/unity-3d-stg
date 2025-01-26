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
            FireController fireController)
            : base(aircraft, fireController)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.SetPitch(0);
                Aircraft.Movement.SetRoll(1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.SetPitch(1);
                Aircraft.Movement.SetRoll(0);
            }));
            _actionQueue.Enqueue((6, () =>
            {
                Aircraft.Movement.SetPitch(0);
                Aircraft.Movement.SetRoll(-1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.SetPitch(1);
                Aircraft.Movement.SetRoll(0);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                Aircraft.Movement.SetPitch(0);
                Aircraft.Movement.SetRoll(1);
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
