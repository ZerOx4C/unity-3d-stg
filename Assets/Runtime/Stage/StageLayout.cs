using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stage
{
    public class StageLayout : MonoBehaviour
    {
        [SerializeField] private PlayerLocator playerLocator;
        [SerializeField] private EnemyAircraftLocator[] enemyAircraftLocators;
        [SerializeField] private TargetLocator[] targetLocators;

        public PlayerLocator PlayerLocator => playerLocator;
        public IReadOnlyList<EnemyAircraftLocator> EnemyAircraftLocators => enemyAircraftLocators;
        public IReadOnlyList<TargetLocator> TargetLocators => targetLocators;

#if UNITY_EDITOR
        [ContextMenu("Setup")]
        private void Setup()
        {
            playerLocator = transform.GetComponentInChildren<PlayerLocator>();
            enemyAircraftLocators = transform.GetComponentsInChildren<EnemyAircraftLocator>();
            targetLocators = transform.GetComponentsInChildren<TargetLocator>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
