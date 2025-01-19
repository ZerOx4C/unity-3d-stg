using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class AircraftModel : MonoBehaviour
    {
        [SerializeField] private List<Transform> guns;
        [SerializeField] private List<Transform> propellers;

        public IReadOnlyList<Transform> Guns => guns;
        public float PropellerSpeed { get; set; }

        private void Update()
        {
            RotatePropellers();
        }

        private void RotatePropellers()
        {
            var angle = PropellerSpeed * Time.deltaTime;

            foreach (var propeller in propellers)
            {
                propeller.Rotate(Vector3.forward, angle);
            }
        }
    }
}
