using UnityEngine;

namespace Utility
{
    public static class MiscUtility
    {
#if UNITY_EDITOR
        public static Mesh CreateGizmoAircraftMesh()
        {
            var mesh = new Mesh
            {
                vertices = new Vector3[]
                {
                    new(-5f, 0, -5f),
                    new(0, 0, 5f),
                    new(5f, 0, -5f),
                    new(0, 1f, -2.5f),
                },
                triangles = new[]
                {
                    0, 1, 3,
                    3, 1, 2,
                    0, 3, 1,
                    3, 2, 1,
                },
            };
            mesh.RecalculateNormals();
            return mesh;
        }
#endif
    }
}
