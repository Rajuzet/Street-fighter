using System.Collections.Generic;
using StreetFighter.Core;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 3: Runtime character roster database. Loads all CharacterRosterData assets
    /// and provides lookup by characterId. Acts as a service for character select and spawners.
    /// </summary>
    public sealed class CharacterDatabase : MonoBehaviour, IGameService
    {
        [Header("Roster")]
        [SerializeField]
        private List<CharacterRosterData> rosterEntries = new();

        private readonly Dictionary<string, CharacterRosterData> idLookup = new();
        private readonly Dictionary<int, CharacterRosterData> indexLookup = new();
        private bool isInitialized;

        public IReadOnlyList<CharacterRosterData> Roster => rosterEntries;
        public int CharacterCount => rosterEntries.Count;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Builds lookup tables from the serialized roster list.
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;

            idLookup.Clear();
            indexLookup.Clear();

            for (int i = 0; i < rosterEntries.Count; i++)
            {
                var entry = rosterEntries[i];
                if (entry == null || string.IsNullOrEmpty(entry.CharacterId)) continue;

                idLookup[entry.CharacterId] = entry;
                indexLookup[i] = entry;
            }

            isInitialized = true;
        }

        /// <summary>
        /// Gets a character by its unique ID.
        /// </summary>
        public CharacterRosterData GetCharacter(string characterId)
        {
            if (!isInitialized) Initialize();
            idLookup.TryGetValue(characterId, out var result);
            return result;
        }

        /// <summary>
        /// Gets a character by its roster index.
        /// </summary>
        public CharacterRosterData GetCharacterByIndex(int index)
        {
            if (!isInitialized) Initialize();
            indexLookup.TryGetValue(index, out var result);
            return result;
        }

        /// <summary>
        /// Checks if a character exists in the roster.
        /// </summary>
        public bool HasCharacter(string characterId)
        {
            if (!isInitialized) Initialize();
            return idLookup.ContainsKey(characterId);
        }

        /// <summary>
        /// Gets the index of a character in the roster.
        /// </summary>
        public int GetCharacterIndex(string characterId)
        {
            if (!isInitialized) Initialize();
            return rosterEntries.FindIndex(c => c != null && c.CharacterId == characterId);
        }

        /// <summary>
        /// Adds a character to the roster at runtime (e.g., DLC unlock).
        /// </summary>
        public void AddCharacter(CharacterRosterData character)
        {
            if (character == null || string.IsNullOrEmpty(character.CharacterId)) return;
            if (idLookup.ContainsKey(character.CharacterId)) return;

            rosterEntries.Add(character);
            int index = rosterEntries.Count - 1;
            idLookup[character.CharacterId] = character;
            indexLookup[index] = character;
        }
    }
}
