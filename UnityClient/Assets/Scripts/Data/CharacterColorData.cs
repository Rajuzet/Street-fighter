using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Phase 3: Defines a color palette variant for a character (costume/skin).
    /// Supports material swaps and tint overrides for tint-based coloring.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterColorData", menuName = "StreetFighter/Data/CharacterColorData")]
    public sealed class CharacterColorData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string colorName = "Default";

        [SerializeField]
        private int colorIndex = 0;

        [Header("Tint Colors (for tint-based materials)")]
        [SerializeField]
        private Color primaryColor = Color.white;

        [SerializeField]
        private Color secondaryColor = Color.white;

        [SerializeField]
        private Color accentColor = Color.white;

        [SerializeField]
        private Color skinColor = Color.white;

        [Header("Material Overrides")]
        [SerializeField]
        private Material primaryMaterial;

        [SerializeField]
        private Material secondaryMaterial;

        [SerializeField]
        private Material accentMaterial;

        [Header("Unlock")]
        [SerializeField]
        private bool unlockedByDefault = true;

        [SerializeField]
        private string unlockCondition = "";

        public string ColorName => colorName;
        public int ColorIndex => colorIndex;
        public Color PrimaryColor => primaryColor;
        public Color SecondaryColor => secondaryColor;
        public Color AccentColor => accentColor;
        public Color SkinColor => skinColor;
        public Material PrimaryMaterial => primaryMaterial;
        public Material SecondaryMaterial => secondaryMaterial;
        public Material AccentMaterial => accentMaterial;
        public bool UnlockedByDefault => unlockedByDefault;
        public string UnlockCondition => unlockCondition;

        /// <summary>
        /// Applies this color palette to a target material using shader properties.
        /// Assumes the shader has _PrimaryColor, _SecondaryColor, _AccentColor, _SkinColor.
        /// </summary>
        public void ApplyToMaterial(Material targetMaterial)
        {
            if (targetMaterial == null) return;

            if (targetMaterial.HasProperty("_PrimaryColor"))
                targetMaterial.SetColor("_PrimaryColor", primaryColor);
            if (targetMaterial.HasProperty("_SecondaryColor"))
                targetMaterial.SetColor("_SecondaryColor", secondaryColor);
            if (targetMaterial.HasProperty("_AccentColor"))
                targetMaterial.SetColor("_AccentColor", accentColor);
            if (targetMaterial.HasProperty("_SkinColor"))
                targetMaterial.SetColor("_SkinColor", skinColor);
        }

        /// <summary>
        /// Applies material overrides to a renderer.
        /// </summary>
        public void ApplyToRenderer(SkinnedMeshRenderer renderer)
        {
            if (renderer == null) return;

            var materials = renderer.materials;
            if (primaryMaterial != null && materials.Length > 0)
                materials[0] = primaryMaterial;
            if (secondaryMaterial != null && materials.Length > 1)
                materials[1] = secondaryMaterial;
            if (accentMaterial != null && materials.Length > 2)
                materials[2] = accentMaterial;
            renderer.materials = materials;
        }
    }
}
