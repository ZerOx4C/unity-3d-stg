using UnityEngine;
using Utility;

namespace Stage
{
    public class PlayerLocator : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawMesh(MiscUtility.CreateGizmoAircraftMesh(), transform.position, transform.rotation);
        }
#endif
    }
}
