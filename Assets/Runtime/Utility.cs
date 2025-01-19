using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Utility
{
    public static async UniTask<T> InstantiateAsync<T>(T prefab,
        Vector3 position, Quaternion rotation, Transform parent = null, CancellationToken cancellationToken = default) where T : Object
    {
        var instances = await Object.InstantiateAsync(prefab, 1, parent, position, rotation, cancellationToken);
        return instances[0];
    }

    public static async UniTask<T> InstantiateAsync<T>(T prefab,
        Transform parent = null, CancellationToken cancellationToken = default) where T : Object
    {
        return await InstantiateAsync(prefab, Vector3.zero, Quaternion.identity, parent, cancellationToken);
    }

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
#endif
}
