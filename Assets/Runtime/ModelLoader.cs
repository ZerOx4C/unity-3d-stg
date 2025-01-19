using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;

public class ModelLoader : MonoBehaviour
{
    private List<Transform> _guns;

    public AircraftModel Model { get; private set; }
    public IReadOnlyList<Transform> Guns => _guns;

    public async UniTask LoadAsync(AircraftModel modelPrefab, CancellationToken cancellation)
    {
        if (Model is not null)
        {
            Destroy(Model);
            Model = null;
        }

        Model = await Utility.InstantiateAsync(modelPrefab,
            transform.position, transform.rotation, transform, cancellation);

        _guns = GameObject.FindGameObjectsWithTag("Gun")
            .Select(g => g.transform)
            .ToList();
    }
}
