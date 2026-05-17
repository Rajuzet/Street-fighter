using System.Collections.Generic;
using StreetFighter.Core;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 3: Tracks character and color unlock state across game sessions.
    /// Integrates with CharacterDatabase and can persist to backend/cloud save.
    /// </summary>
    public sealed class CharacterUnlockManager : MonoBehaviour
    {
        [Header("Database")]
        [SerializeField]
        private CharacterDatabase database;

        private readonly HashSet<string> unlockedCharacters = new();
        private readonly Dictionary<string, HashSet<int>> unlockedColors = new();

        private void Awake()
        {
            if (database == null)
                database = FindObjectOfType<CharacterDatabase>();

            InitializeDefaults();
        }

        /// <summary>
        /// Unlocks all characters and colors marked as default-unlocked in roster data.
        /// </summary>
        private void InitializeDefaults()
        {
            if (database == null) return;

            foreach (var character in database.Roster)
            {
                if (character == null) continue;

                if (character.UnlockedByDefault)
                    unlockedCharacters.Add(character.CharacterId);

                if (!unlockedColors.ContainsKey(character.CharacterId))
                    unlockedColors[character.CharacterId] = new HashSet<int>();

                for (int i = 0; i < character.ColorPalettes.Count; i++)
                {
                    if (character.ColorPalettes[i]?.UnlockedByDefault == true)
                        unlockedColors[character.CharacterId].Add(i);
                }
            }
        }

        /// <summary>
        /// Checks if a character is unlocked.
        /// </summary>
        public bool IsCharacterUnlocked(string characterId)
        {
            return unlockedCharacters.Contains(characterId);
        }

        /// <summary>
        /// Unlocks a character by ID.
        /// </summary>
        public void UnlockCharacter(string characterId)
        {
            unlockedCharacters.Add(characterId);

            // Auto-unlock first color if not already set
            if (!unlockedColors.ContainsKey(characterId))
                unlockedColors[characterId] = new HashSet<int>();
            unlockedColors[characterId].Add(0);
        }

        /// <summary>
        /// Checks if a specific color variant is unlocked for a character.
        /// </summary>
        public bool IsColorUnlocked(string characterId, int colorIndex)
        {
            return unlockedColors.TryGetValue(characterId, out var colors) && colors.Contains(colorIndex);
        }

        /// <summary>
        /// Unlocks a specific color variant.
        /// </summary>
        public void UnlockColor(string characterId, int colorIndex)
        {
            if (!unlockedColors.ContainsKey(characterId))
                unlockedColors[characterId] = new HashSet<int>();
            unlockedColors[characterId].Add(colorIndex);
        }

        /// <summary>
        /// Gets all unlocked characters.
        /// </summary>
        public IEnumerable<string> GetUnlockedCharacters()
        {
            return unlockedCharacters;
        }

        /// <summary>
        /// Gets all unlocked color indices for a character.
        /// </summary>
        public IEnumerable<int> GetUnlockedColors(string characterId)
        {
            if (unlockedColors.TryGetValue(characterId, out var colors))
                return colors;
            return new List<int>();
        }

        /// <summary>
        /// Clears all unlocks (for testing or new game).
        /// </summary>
        public void ResetUnlocks()
        {
            unlockedCharacters.Clear();
            unlockedColors.Clear();
        }

        /// <summary>
        /// Serializes unlock state to a save-friendly format.
        /// </summary>
        public UnlockSaveData ExportSaveData()
        {
            return new UnlockSaveData
            {
                UnlockedCharacters = new List<string>(unlockedCharacters),
                UnlockedColors = new Dictionary<string, List<int>>(
                    unlockedColors.ConvertAll(kvp => new KeyValuePair<string, List<int>>(kvp.Key, new List<int>(kvp.Value)))
                )
            };
        }

        /// <summary>
        /// Restores unlock state from save data.
        /// </summary>
        public void ImportSaveData(UnlockSaveData data)
        {
            if (data == null) return;

            unlockedCharacters.Clear();
            unlockedColors.Clear();

            foreach (var charId in data.UnlockedCharacters)
                unlockedCharacters.Add(charId);

            foreach (var kvp in data.UnlockedColors)
            {
                unlockedColors[kvp.Key] = new HashSet<int>(kvp.Value);
            }
        }
    }

    /// <summary>
    /// Serializable container for unlock state.
    /// </summary>
    [System.Serializable]
    public class UnlockSaveData
    {
        public List<string> UnlockedCharacters;
        public Dictionary<string, List<int>> UnlockedColors;
    }
}
