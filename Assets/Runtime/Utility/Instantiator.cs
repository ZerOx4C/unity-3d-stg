using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utility
{
    public static class Instantiator
    {
        public static Config<T> Create<T>(T prefab) where T : Object
        {
            return new Config<T>(prefab);
        }

        public readonly struct Result<T> where T : Object
        {
            public Result(UniTask<T[]> task)
            {
                _task = UniTask.Lazy(async () => await task);
            }

            private readonly AsyncLazy<T[]> _task;

            public async UniTask<T> First()
            {
                return (await _task)[0];
            }

            public async UniTask<T[]> All()
            {
                return await _task;
            }
        }

        public class Config<T> where T : Object
        {
            private readonly T _prefab;
            private int _count;
            private Transform _parent;
            private Vector3[] _positions;
            private Quaternion[] _rotations;

            public Config(T prefab)
            {
                _prefab = prefab;
                SetTransforms(1, Vector3.zero, Quaternion.identity);
            }

            public Config<T> SetParent(Transform parent)
            {
                _parent = parent;
                return this;
            }

            public Config<T> SetTransforms(Vector3 position, Quaternion rotation)
            {
                return SetTransforms(1, position, rotation);
            }

            public Config<T> SetTransforms(Transform transform)
            {
                return SetTransforms(1, transform);
            }

            public Config<T> SetTransforms(int count, Vector3 position, Quaternion rotation)
            {
                _count = count;
                _positions = new[] { position };
                _rotations = new[] { rotation };
                return this;
            }

            public Config<T> SetTransforms(int count, Transform transform)
            {
                transform.GetPositionAndRotation(out var position, out var rotation);
                return SetTransforms(count, position, rotation);
            }

            public Config<T> SetTransforms(IReadOnlyList<Vector3> positions, IReadOnlyList<Quaternion> rotations)
            {
                Assert.AreEqual(positions.Count, rotations.Count);
                _count = positions.Count;
                _positions = positions.ToArray();
                _rotations = rotations.ToArray();
                return this;
            }

            public Config<T> SetTransforms(IReadOnlyList<Transform> transforms)
            {
                _count = transforms.Count;
                _positions = new Vector3[transforms.Count];
                _rotations = new Quaternion[transforms.Count];

                for (var i = 0; i < transforms.Count; i++)
                {
                    transforms[i].GetPositionAndRotation(out _positions[i], out _rotations[i]);
                }

                return this;
            }

            public Result<T> InstantiateAsync(CancellationToken cancellation)
            {
                var task = Object.InstantiateAsync(_prefab, _count, _parent, _positions, _rotations, cancellation)
                    .ToUniTask(cancellationToken: cancellation);

                return new Result<T>(task);
            }
        }
    }
}
