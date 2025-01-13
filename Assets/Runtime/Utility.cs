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
}
