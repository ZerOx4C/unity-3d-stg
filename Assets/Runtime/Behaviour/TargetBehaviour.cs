using Model;
using R3;
using UnityEngine;

namespace Behaviour
{
    public class TargetBehaviour : MonoBehaviour
    {
        private TargetModel _model;
        private Subject<Unit> _onDead;

        public ModelLoader.Loader<TargetModel> ModelLoader { get; private set; }
        public Observable<Unit> OnDead => _onDead;

        private void Awake()
        {
            ModelLoader = GetComponent<ModelLoader>().CreateLoader<TargetModel>();
            _onDead = new Subject<Unit>();
        }

        private void OnDestroy()
        {
            _onDead.Dispose();
        }

        public void Damage()
        {
            _onDead.OnNext(Unit.Default);
        }
    }
}
