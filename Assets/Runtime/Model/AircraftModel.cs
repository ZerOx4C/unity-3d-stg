using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class AircraftModel : MonoBehaviour
    {
        public List<Transform> propellers;

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
