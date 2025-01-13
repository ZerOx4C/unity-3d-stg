using System;
using System.Collections.Generic;
using Behaviour;
using UnityEngine;

public class EnemyAircraftController
{
    private readonly Queue<(float delay, Action action)> _actionQueue = new();
    private readonly AircraftBehaviour _aircraft;

    private float _wait;

    public EnemyAircraftController(AircraftBehaviour aircraft)
    {
        _aircraft = aircraft;
    }

    public void Initialize()
    {
        _actionQueue.Enqueue((3, () =>
        {
            _aircraft.Movement.Pitch(0);
            _aircraft.Movement.Roll(1);
        }));
        _actionQueue.Enqueue((3, () =>
        {
            _aircraft.Movement.Pitch(1);
            _aircraft.Movement.Roll(0);
        }));
        _actionQueue.Enqueue((6, () =>
        {
            _aircraft.Movement.Pitch(0);
            _aircraft.Movement.Roll(-1);
        }));
        _actionQueue.Enqueue((3, () =>
        {
            _aircraft.Movement.Pitch(1);
            _aircraft.Movement.Roll(0);
        }));
        _actionQueue.Enqueue((3, () =>
        {
            _aircraft.Movement.Pitch(0);
            _aircraft.Movement.Roll(1);
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
