using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Model
{
    public class TargetModel : MonoBehaviour
    {
        [SerializeField] private Transform[] fragments;

        public IReadOnlyList<Transform> Fragments => fragments;

#if UNITY_EDITOR
        [ContextMenu("Setup")]
        private void Setup()
        {
            fragments = ModelPartTag.GetTransformsByFlags(transform, ModelPartFlags.Fragment);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
