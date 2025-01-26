using UnityEngine;

namespace Utility
{
    public class RigidbodyReader
    {
        private readonly Rigidbody _rigidbody;

        public RigidbodyReader(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public Vector3 LinearVelocity => _rigidbody.linearVelocity;
        public Vector3 AngularVelocity => _rigidbody.angularVelocity;
    }
}
