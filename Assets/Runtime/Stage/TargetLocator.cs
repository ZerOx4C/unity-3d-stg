using UnityEngine;

namespace Stage
{
    public class TargetLocator : MonoBehaviour
    {
#if UNITY_EDITOR
        private static Mesh CreateGizmoTargetMesh()
        {
            var mesh = new Mesh
            {
                vertices = new Vector3[]
                {
                    new(-5f, -5f, 0),
                    new(-5f, 5f, 0),
                    new(5f, 5f, 0),
                    new(5f, -5f, 0),
                    new(-4f, -4f, 0),
                    new(-4f, 4f, 0),
                    new(4f, 4f, 0),
                    new(4f, -4f, 0),
                },
                triangles = new[]
                {
                    1, 5, 6,
                    6, 2, 1,
                    2, 6, 7,
                    7, 3, 2,
                    3, 7, 4,
                    4, 0, 3,
                    0, 4, 5,
                    5, 1, 0,

                    1, 6, 5,
                    6, 1, 2,
                    2, 7, 6,
                    7, 2, 3,
                    3, 4, 7,
                    4, 3, 0,
                    0, 5, 4,
                    5, 0, 1,
                },
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawMesh(CreateGizmoTargetMesh(), transform.position, transform.rotation);
        }
#endif
    }
}
