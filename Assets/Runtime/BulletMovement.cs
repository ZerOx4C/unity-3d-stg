using UnityEngine;

public class BulletMovement : MonoBehaviour, IMovement
{
    public float dragFactor = 1;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var velocity = _rigidbody.linearVelocity;
        velocity += dt * CalcDrag(velocity, dragFactor);
        _rigidbody.linearVelocity = velocity;
    }

    public Vector3 LinearVelocity => _rigidbody.linearVelocity;

    public void Initialize(IMovement ownerMovement, Transform gunTransform, float initialSpeed)
    {
        gunTransform.GetPositionAndRotation(out var position, out var rotation);
        transform.SetPositionAndRotation(position, rotation);
        _rigidbody.linearVelocity = initialSpeed * transform.forward + ownerMovement.LinearVelocity;
    }

    private static Vector3 CalcDrag(Vector3 velocity, float dragFactor)
    {
        if (velocity.sqrMagnitude == 0)
        {
            return Vector3.zero;
        }

        return -dragFactor * velocity;
    }
}
