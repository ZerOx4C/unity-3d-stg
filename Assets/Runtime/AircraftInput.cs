using System;
using UnityEngine.InputSystem;
using VContainer.Unity;

public class AircraftInput : InputSystem_Actions.IAircraftActions, ITickable, IDisposable
{
    private readonly InputSystem_Actions _inputActions = new();
    private readonly InputSubject _pitch = new();
    private readonly InputSubject _roll = new();
    private readonly InputSubject _throttle = new();
    private readonly InputSubject _yaw = new();

    public AircraftInput()
    {
        _inputActions.Aircraft.AddCallbacks(this);
        _inputActions.Aircraft.Enable();
    }

    public IInputObservable Pitch => _pitch;
    public IInputObservable Roll => _roll;
    public IInputObservable Yaw => _yaw;
    public IInputObservable Throttle => _throttle;

    public void OnPitch(InputAction.CallbackContext context)
    {
        _pitch.OnNext(context);
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        _roll.OnNext(context);
    }

    public void OnYaw(InputAction.CallbackContext context)
    {
        _yaw.OnNext(context);
    }

    public void OnThrottle(InputAction.CallbackContext context)
    {
        _throttle.OnNext(context);
    }

    public void Dispose()
    {
        _inputActions.Aircraft.Disable();
        _inputActions.Aircraft.RemoveCallbacks(this);
        _pitch.Dispose();
        _roll.Dispose();
        _yaw.Dispose();
        _throttle.Dispose();
    }

    public void Tick()
    {
        _pitch.Tick();
        _roll.Tick();
        _yaw.Tick();
        _throttle.Tick();
    }
}
