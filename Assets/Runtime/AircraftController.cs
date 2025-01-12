using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    public float minDragFactor = 1;
    public float maxDragFactor = 5;
    public float liftFactor = 1;
    public float throttleAcceleration = 10;

    private GameObject _model;
    private float _pitch;
    private Rigidbody _rigidbody;
    private float _roll;
    private float _throttle;
    private float _yaw;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        DebugHud.OnReset
            .Subscribe(_ =>
            {
                _rigidbody.linearVelocity = Vector3.zero;
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            })
            .AddTo(this);

        Pitch(0);
        Roll(0);
        Yaw(0);
        Throttle(0);
    }

    private void Update()
    {
        DebugHud.Log("pitch", $"pitch:{_pitch}");
        DebugHud.Log("roll", $"roll:{_roll}");
        DebugHud.Log("yaw", $"yaw:{_yaw}");
        DebugHud.Log("throttle", $"throttle:{_throttle}");
        DebugHud.Log("velocity", $"velocity:{_rigidbody.linearVelocity}");
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var velocity = _rigidbody.linearVelocity;
        velocity += dt * _throttle * throttleAcceleration * transform.forward;
        velocity += dt * CalcLift(velocity, transform, liftFactor);
        velocity += dt * Physics.gravity;
        velocity += dt * CalcDrag(velocity, transform, minDragFactor, maxDragFactor);
        velocity -= dt * Physics.gravity;
        _rigidbody.linearVelocity = velocity;

        _rigidbody.angularVelocity = Time.fixedDeltaTime *
                                     (-20 * _roll * transform.forward +
                                      -10 * _pitch * transform.right +
                                      5 * _yaw * transform.up);
    }

    public async UniTask SetModel(GameObject prefab, CancellationToken cancellation)
    {
        if (_model is null)
        {
            Destroy(_model);
            _model = null;
        }

        var instances = await InstantiateAsync(prefab, 1, transform, Vector3.zero, Quaternion.identity, cancellation);
        _model = instances[0];
    }

    public void Pitch(float input)
    {
        _pitch = input;
    }

    public void Roll(float input)
    {
        _roll = input;
    }

    public void Yaw(float input)
    {
        _yaw = input;
    }

    public void Throttle(float input)
    {
        _throttle = 0.5f + 0.5f * input;
    }

    private static Vector3 CalcDrag(Vector3 velocity, Transform transform, float minDragFactor, float maxDragFactor)
    {
        if (velocity.sqrMagnitude == 0)
        {
            return Vector3.zero;
        }

        var rotationFactor = 1 - Mathf.Abs(Vector3.Dot(transform.forward, velocity)) / velocity.magnitude;
        var dragFactor = minDragFactor + rotationFactor * (maxDragFactor - minDragFactor);
        return -dragFactor * velocity;
    }

    private static Vector3 CalcLift(Vector3 velocity, Transform transform, float liftFactor)
    {
        if (velocity.sqrMagnitude == 0)
        {
            return Vector3.zero;
        }

        var aeroSpeed = Vector3.Dot(transform.forward, velocity);
        return liftFactor * aeroSpeed * aeroSpeed * transform.up;
    }
}
