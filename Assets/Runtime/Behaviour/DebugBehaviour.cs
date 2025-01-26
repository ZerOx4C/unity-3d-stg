using UnityEditor;
using UnityEngine;

namespace Behaviour
{
    public class DebugBehaviour : MonoBehaviour
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DebugBehaviour))]
    public class DebugBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var go = ((Component)target).gameObject;

            if (GUILayout.Button("Damage"))
            {
                if (go.TryGetComponent(out AircraftBehaviour aircraftBehaviour))
                {
                    aircraftBehaviour.Damage();
                }
                else if (go.TryGetComponent(out TargetBehaviour targetBehaviour))
                {
                    targetBehaviour.Damage();
                }
            }
        }
    }
#endif
}
