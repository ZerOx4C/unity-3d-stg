using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    private GameObject _model;
    private Rigidbody _rigidbody;

    public float Pitch { get; set; }
    public float Roll { get; set; }
    public float Yaw { get; set; }
    public float Throttle { get; set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.linearVelocity = transform.forward;
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity += Physics.gravity.magnitude * Time.fixedDeltaTime * Vector3.up;
        _rigidbody.angularVelocity = Time.fixedDeltaTime *
                                     (-10 * Roll * transform.forward +
                                      -10 * Pitch * transform.right +
                                      10 * Yaw * transform.up);
    }

    public async UniTask SetModel(GameObject prefab, CancellationToken cancellation)
    {
        if (_model != null)
        {
            Destroy(_model);
            _model = null;
        }

        var instances = await InstantiateAsync(prefab, 1, transform, Vector3.zero, Quaternion.identity, cancellation);
        _model = instances[0];
    }
}
