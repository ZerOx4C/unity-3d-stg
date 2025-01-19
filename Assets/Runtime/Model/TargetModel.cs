using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Model
{
    public class TargetModel : MonoBehaviour
    {
        [SerializeField] private Transform[] fragments;

#if UNITY_EDITOR
        [ContextMenu("Setup")]
        private void Setup()
        {
            fragments = ModelPartTag.GetTransformsByFlags(transform, ModelPartFlags.Fragment).ToArray();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
