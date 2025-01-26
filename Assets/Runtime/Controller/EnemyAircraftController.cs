using System;
using System.Collections.Generic;
using Behaviour;
using UnityEngine;

namespace Controller
{
    public class EnemyAircraftController
    {
        private readonly Queue<(float delay, Action action)> _actionQueue = new();
        private float _wait;

        public EnemyAircraftController(AircraftBehaviour aircraft)
        {
            _actionQueue.Enqueue((3, () =>
            {
                aircraft.Movement.SetPitch(0);
                aircraft.Movement.SetRoll(1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                aircraft.Movement.SetPitch(1);
                aircraft.Movement.SetRoll(0);
            }));
            _actionQueue.Enqueue((6, () =>
            {
                aircraft.Movement.SetPitch(0);
                aircraft.Movement.SetRoll(-1);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                aircraft.Movement.SetPitch(1);
                aircraft.Movement.SetRoll(0);
            }));
            _actionQueue.Enqueue((3, () =>
            {
                aircraft.Movement.SetPitch(0);
                aircraft.Movement.SetRoll(1);
            }));
        }

        public void Tick()
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
