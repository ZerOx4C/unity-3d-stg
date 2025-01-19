using Model;
using UnityEngine;

namespace Stage
{
    public class EnemyAircraftLocator : MonoBehaviour
    {
        public AircraftModel modelPrefab;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawMesh(Utility.CreateGizmoAircraftMesh(), transform.position, transform.rotation);
        }
#endif
    }
}
