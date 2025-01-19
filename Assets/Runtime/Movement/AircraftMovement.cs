using UnityEngine;

namespace Movement
{
    public class AircraftMovement : MonoBehaviour, IMovement
    {
        public float throttleAcceleration = 1;
        public float minDragFactor = 1;
        public float maxDragFactor = 1;
        public float liftFactor = 1;
        public float pitchFactor = 1;
        public float rollFactor = 1;
        public float yawFactor = 1;

        private Rigidbody _rigidbody;

        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public float Yaw { get; private set; }
        public float Throttle { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            SetPitch(0);
            SetRoll(0);
            SetYaw(0);
            SetThrottle(0);
        }

        private void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            var velocity = _rigidbody.linearVelocity;
            velocity += dt * Throttle * throttleAcceleration * transform.forward;
            velocity += dt * CalcLift(velocity, transform, liftFactor);
            velocity += dt * CalcDrag(velocity, transform, minDragFactor, maxDragFactor);
            _rigidbody.linearVelocity = velocity;

            _rigidbody.angularVelocity = Time.fixedDeltaTime *
                                         (Roll * rollFactor * -transform.forward +
                                          Pitch * pitchFactor * -transform.right +
                                          Yaw * yawFactor * transform.up);
        }

        public Vector3 LinearVelocity => _rigidbody.linearVelocity;

        public void SetPitch(float input)
        {
            Pitch = Mathf.Clamp(input, -1, 1);
        }

        public void SetRoll(float input)
        {
            Roll = Mathf.Clamp(input, -1, 1);
        }

        public void SetYaw(float input)
        {
            Yaw = Mathf.Clamp(input, -1, 1);
        }

        public void SetThrottle(float input)
        {
            Throttle = 0.5f + 0.5f * Mathf.Clamp(input, -1, 1);
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
