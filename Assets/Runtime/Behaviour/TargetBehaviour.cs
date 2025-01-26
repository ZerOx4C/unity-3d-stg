using System.Threading;
using Cysharp.Threading.Tasks;
using Model;
using R3;
using UnityEngine;
using Utility;

namespace Behaviour
{
    public class TargetBehaviour : MonoBehaviour
    {
        private TargetModel _model;
        private Subject<Unit> _onDead;

        public Observable<Unit> OnDead => _onDead;

        private void Awake()
        {
            _onDead = new Subject<Unit>();
        }

        private void OnDestroy()
        {
            _onDead.Dispose();
        }

        public async UniTask LoadModelAsync(TargetModel modelPrefab, CancellationToken cancellation)
        {
            if (_model is not null)
            {
                Destroy(_model.gameObject);
                _model = null;
            }

            _model = await Instantiator.Create(modelPrefab)
                .SetParent(transform)
                .SetTransforms(transform)
                .InstantiateAsync(cancellation).First;
        }

        public void Damage()
        {
            _onDead.OnNext(Unit.Default);
        }
    }
}
