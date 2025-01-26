using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Model
{
    public class AircraftModel : MonoBehaviour
    {
        [SerializeField] private Transform[] guns;
        [SerializeField] private Transform[] fragments;
        [SerializeField] private Transform[] propellers;

        public IReadOnlyList<Transform> Guns => guns;
        public IReadOnlyList<Transform> Fragments => fragments;
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

#if UNITY_EDITOR
        [ContextMenu("Setup")]
        private void Setup()
        {
            guns = ModelPartTag.GetTransformsByFlags(transform, ModelPartFlags.Gun);
            fragments = ModelPartTag.GetTransformsByFlags(transform, ModelPartFlags.Fragment);
            propellers = ModelPartTag.GetTransformsByFlags(transform, ModelPartFlags.Propeller);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
