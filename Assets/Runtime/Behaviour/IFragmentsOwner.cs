using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Behaviour
{
    public interface IFragmentsOwner
    {
        public GameObject gameObject { get; }
        public IReadOnlyList<Transform> Fragments { get; }
        public RigidbodyReader RigidbodyReader { get; }
    }
}
