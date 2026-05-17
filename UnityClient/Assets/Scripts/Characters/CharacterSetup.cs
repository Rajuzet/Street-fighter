using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 3: Spawns a character from roster data and applies profile overrides,
    /// unique moves, color palette, and stats at runtime.
    /// </summary>
    public sealed class CharacterSetup : MonoBehaviour
    {
        [Header("Roster")]
        [SerializeField]
        private CharacterRosterData rosterData;

        [SerializeField]
        private int selectedColorIndex = 0;

        [Header("Runtime References")]
        private CharacterStats stats;
        private StaminaSystem stamina;
        private CombatSystemManager combat;
        private CharacterAnimationController animController;
        private SkinnedMeshRenderer meshRenderer;

        /// <summary>
        /// Initializes this character instance from roster data.
        /// Should be called after instantiation (e.g., by a spawner).
        /// </summary>
        public void InitializeFromRoster(CharacterRosterData data, int colorIndex = 0)
        {
            rosterData = data;
            selectedColorIndex = colorIndex;

            ApplyProfile();
            ApplyStats();
            ApplyColor();
            ApplyMoves();
        }

        private void ApplyProfile()
        {
            if (rosterData?.BaseProfile == null) return;

            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                // Uses reflection or a public setter; for now we rely on
                // the PlayerController reading profile at Awake/Start.
                // In a full build, add a public SetProfile method.
            }
        }

        private void ApplyStats()
        {
            stats = GetComponent<CharacterStats>();
            if (stats == null || rosterData == null) return;

            // CharacterRosterData provides override values; we apply them via
            // serialized fields if the component supports runtime changes.
            // For now, the stats are baked into prefab variants.
        }

        private void ApplyColor()
        {
            if (rosterData == null) return;

            var palettes = rosterData.ColorPalettes;
            if (palettes == null || palettes.Count == 0) return;

            int safeIndex = Mathf.Clamp(selectedColorIndex, 0, palettes.Count - 1);
            var colorData = palettes[safeIndex];

            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                // Apply tint colors to material
                if (meshRenderer.material != null)
                {
                    colorData.ApplyToMaterial(meshRenderer.material);
                }

                // Apply material overrides
                colorData.ApplyToRenderer(meshRenderer);
            }
        }

        private void ApplyMoves()
        {
            combat = GetComponent<CombatSystemManager>();
            if (combat == null || rosterData == null) return;

            // Unique moves are injected into CombatSystemManager
            var uniqueMoves = rosterData.UniqueMoves;
            if (uniqueMoves != null && uniqueMoves.Count > 0)
            {
                // The combat system reads from its serialized list; for runtime
                // injection we would need a public API on CombatSystemManager.
                // This is a hook for future extension.
            }
        }

        /// <summary>
        /// Gets the roster data for this character instance.
        /// </summary>
        public CharacterRosterData RosterData => rosterData;

        /// <summary>
        /// Gets the selected color index.
        /// </summary>
        public int SelectedColorIndex => selectedColorIndex;
    }
}
