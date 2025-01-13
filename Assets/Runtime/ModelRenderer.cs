using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ModelRenderer : MonoBehaviour
{
    private GameObject _model;

    public async UniTask LoadAsync(GameObject prefab, CancellationToken cancellation)
    {
        if (_model is null)
        {
            Destroy(_model);
            _model = null;
        }

        var instances = await InstantiateAsync(prefab, 1, transform, Vector3.zero, Quaternion.identity, cancellation);
        _model = instances[0];
    }
}
