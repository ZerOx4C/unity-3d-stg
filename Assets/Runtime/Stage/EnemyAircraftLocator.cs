using Model;
using UnityEngine;
using Utility;

namespace Stage
{
    public class EnemyAircraftLocator : MonoBehaviour
    {
        public AircraftModel modelPrefab;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawMesh(MiscUtility.CreateGizmoAircraftMesh(), transform.position, transform.rotation);
        }
#endif
    }
}
