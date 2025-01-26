using System;
using System.Linq;
using UnityEngine;

namespace Model
{
    [Flags]
    public enum ModelPartFlags
    {
        None = 0,
        Gun = 1 << 0,
        Propeller = 1 << 1,
        Fragment = 1 << 2,
    }

    public class ModelPartTag : MonoBehaviour
    {
        public ModelPartFlags flags;

#if UNITY_EDITOR
        public static Transform[] GetTransformsByFlags(Transform root, ModelPartFlags flags)
        {
            return root.GetComponentsInChildren<ModelPartTag>(false)
                .Where(t => t.flags.HasFlag(flags))
                .Select(t => t.transform)
                .ToArray();
        }
#endif
    }
}
