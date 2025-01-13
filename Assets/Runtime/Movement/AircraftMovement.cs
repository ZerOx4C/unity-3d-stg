using UnityEngine;

namespace Movement
{
    public class AircraftMovement : MonoBehaviour, IMovement
    {
        private static readonly Vector3 Gravity = Physics.gravity;

        public float throttleAcceleration = 1;
        public float minDragFactor = 1;
        public float maxDragFactor = 1;
        public float liftFactor = 1;
        public float pitchFactor = 1;
        public float rollFactor = 1;
        public float yawFactor = 1;

        private float _pitch;
        private Rigidbody _rigidbody;
        private float _roll;
        private float _throttle;
        private float _yaw;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

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
            velocity += dt * Gravity;
            velocity += dt * CalcDrag(velocity, transform, minDragFactor, maxDragFactor);
            velocity -= dt * Gravity;
            _rigidbody.linearVelocity = velocity;

            _rigidbody.angularVelocity = Time.fixedDeltaTime *
                                         (_roll * rollFactor * -transform.forward +
                                          _pitch * pitchFactor * -transform.right +
                                          _yaw * yawFactor * transform.up);
        }

        public Vector3 LinearVelocity => _rigidbody.linearVelocity;

        public void Pitch(float input)
        {
            _pitch = Mathf.Clamp(input, -1, 1);
        }

        public void Roll(float input)
        {
            _roll = Mathf.Clamp(input, -1, 1);
        }

        public void Yaw(float input)
        {
            _yaw = Mathf.Clamp(input, -1, 1);
        }

        public void Throttle(float input)
        {
            _throttle = 0.5f + 0.5f * Mathf.Clamp(input, -1, 1);
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
    }
}
