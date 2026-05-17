using System.Collections;
using StreetFighter.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace StreetFighter.UI
{
    /// <summary>
    /// Phase 3: Spawns floating damage numbers at hit locations.
    /// Pools text objects and supports critical/combo color variations.
    /// </summary>
    public sealed class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField]
        private GameObject damageNumberPrefab;

        [Header("Settings")]
        [SerializeField]
        private int poolSize = 20;

        [SerializeField]
        private float displayDuration = 1.2f;

        [SerializeField]
        private float floatSpeed = 60f;

        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color criticalColor = Color.red;

        [SerializeField]
        private Color comboColor = Color.yellow;

        [SerializeField]
        private float criticalScale = 1.5f;

        private Transform[] pool;
        private Text[] textPool;
        private int poolIndex;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (damageNumberPrefab == null) return;

            pool = new Transform[poolSize];
            textPool = new Text[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(damageNumberPrefab, transform);
                obj.SetActive(false);
                pool[i] = obj.transform;
                textPool[i] = obj.GetComponentInChildren<Text>();
            }
        }

        /// <summary>
        /// Spawns a damage number at the given world position.
        /// </summary>
        public void Spawn(Vector3 worldPosition, int damage, bool isCritical = false, int comboCount = 0)
        {
            if (pool == null || pool.Length == 0) return;

            var obj = pool[poolIndex];
            var text = textPool[poolIndex];
            poolIndex = (poolIndex + 1) % poolSize;

            if (text == null) return;

            // Convert world position to screen space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            obj.position = screenPos;
            obj.gameObject.SetActive(true);

            // Set color based on hit type
            if (isCritical)
            {
                text.color = criticalColor;
                text.text = $"{damage}!";
                obj.localScale = Vector3.one * criticalScale;
            }
            else if (comboCount >= 3)
            {
                text.color = comboColor;
                text.text = $"{damage} x{comboCount}";
                obj.localScale = Vector3.one;
            }
            else
            {
                text.color = normalColor;
                text.text = damage.ToString();
                obj.localScale = Vector3.one;
            }

            StopAllCoroutines();
            StartCoroutine(AnimateNumber(obj, text));
        }

        private IEnumerator AnimateNumber(Transform obj, Text text)
        {
            float elapsed = 0f;
            Vector3 startPos = obj.position;
            Color startColor = text.color;

            while (elapsed < displayDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / displayDuration;

                // Float upward
                obj.position = startPos + Vector3.up * floatSpeed * t;

                // Fade out
                text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

                yield return null;
            }

            obj.gameObject.SetActive(false);
        }

        /// <summary>
        /// Callback for combat hit events to auto-spawn numbers.
        /// </summary>
        public void OnHitEvent(DamageResult result)
        {
            if (result == null) return;

            Spawn(result.HitPosition, result.DamageDealt, result.IsCritical, result.ComboCount);
        }
    }
}
