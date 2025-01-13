using R3;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    private bool _fire;
    private float _fireCooldown;
    private int _nextGunIndex;
    private Subject<Transform> _onFire;
    private Rigidbody _rigidbody;

    public AircraftMovement Movement { get; private set; }
    public ModelLoader Loader { get; private set; }
    public Observable<Transform> OnFire => _onFire;

    private void Awake()
    {
        Movement = GetComponent<AircraftMovement>();
        Loader = GetComponent<ModelLoader>();
        _rigidbody = GetComponent<Rigidbody>();
        _onFire = new Subject<Transform>();
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

    private void Update()
    {
        if (0 < _fireCooldown)
        {
            _fireCooldown -= Time.deltaTime;
            return;
        }

        if (!_fire)
        {
            return;
        }

        _fireCooldown = 0.05f;

        var gun = Loader.Guns[_nextGunIndex++];
        _nextGunIndex %= Loader.Guns.Count;
        _onFire.OnNext(gun);
    }

    private void OnDestroy()
    {
        _onFire.Dispose();
    }

    public void Fire(bool active)
    {
        _fire = active;
    }
}
