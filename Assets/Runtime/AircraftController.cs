using R3;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public AircraftMovement Movement { get; private set; }
    public ModelRenderer Renderer { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<AircraftMovement>();
        Renderer = GetComponent<ModelRenderer>();
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
    }
}
