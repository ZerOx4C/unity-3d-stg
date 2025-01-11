using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    private GameObject _model;

    public async UniTask SetModel(GameObject prefab, CancellationToken cancellation)
    {
        if (_model != null)
        {
            Destroy(_model);
            _model = null;
        }

        var instances = await InstantiateAsync(prefab, 1, transform, Vector3.zero, Quaternion.identity, cancellation);
        _model = instances[0];
    }
}
