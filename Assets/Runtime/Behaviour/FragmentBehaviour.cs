using UnityEngine;

namespace Behaviour
{
    public class FragmentBehaviour : MonoBehaviour
    {
        public float lifetime;
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (0 < lifetime)
            {
                lifetime -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
