using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ModelLoader : MonoBehaviour
{
    private List<Transform> _guns;
    private GameObject _model;

    public IReadOnlyList<Transform> Guns => _guns;

    public async UniTask LoadAsync(GameObject prefab, CancellationToken cancellation)
    {
        if (_model is null)
        {
            Destroy(_model);
            _model = null;
        }

        _model = await Utility.InstantiateAsync(prefab,
            transform.position, transform.rotation, transform, cancellation);

        _guns = GameObject.FindGameObjectsWithTag("Gun")
            .Select(g => g.transform)
            .ToList();
    }
}
