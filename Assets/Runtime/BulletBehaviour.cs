using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float lifetime = 1;

    private float _lifetime;

    public BulletMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<BulletMovement>();
    }

    private void Update()
    {
        if (0 < _lifetime)
        {
            _lifetime -= Time.deltaTime;
        }
    }

    public void Initialize()
    {
        _lifetime = lifetime;
    }
}
