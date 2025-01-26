using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

public class ModelLoader : MonoBehaviour
{
    public Loader<T> CreateLoader<T>() where T : Component
    {
        return new Loader<T>(transform);
    }

    public class Loader<T> where T : Component
    {
        private readonly Transform _owner;

        public Loader(Transform owner)
        {
            _owner = owner;
        }

        public bool IsLoaded { get; private set; }
        public T Model { get; private set; }

        public async UniTask LoadAsync(T modelPrefab, CancellationToken cancellation)
        {
            if (IsLoaded)
            {
                Destroy(Model.gameObject);
            }

            Model = await Instantiator.Create(modelPrefab)
                .SetParent(_owner)
                .SetTransforms(_owner)
                .InstantiateAsync(cancellation).First;

            IsLoaded = true;
        }
    }
}
