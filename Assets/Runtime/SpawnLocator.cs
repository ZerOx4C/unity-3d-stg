using UnityEngine;

public class SpawnLocator : MonoBehaviour
{
    public bool isPlayer;
    public GameObject modelPrefab;

#if UNITY_EDITOR
    private static Mesh _gizmoMesh;

    private static Mesh CreateGizmoMesh()
    {
        var mesh = new Mesh
        {
            vertices = new Vector3[]
            {
                new(-5f, 0, -5f),
                new(0, 0, 5f),
                new(5f, 0, -5f),
                new(0, 1f, -2.5f)
            },
            triangles = new[]
            {
                0, 1, 3,
                3, 1, 2,
                0, 3, 1,
                3, 2, 1
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnDrawGizmos()
    {
        _gizmoMesh ??= CreateGizmoMesh();
        _gizmoMesh.UploadMeshData(false);

        Gizmos.color = isPlayer ? Color.cyan : Color.red;
        Gizmos.DrawMesh(_gizmoMesh, transform.position, transform.rotation);
    }
#endif
}
