using System;
using StreetFighter.Characters;
using StreetFighter.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StreetFighter.UI
{
    /// <summary>
    /// Phase 3: Character select screen controller. Handles navigation,
    /// selection lock-in, color picker cycling, and two-player selection.
    /// </summary>
    public sealed class CharacterSelectController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private CharacterDatabase database;

        [SerializeField]
        private CharacterUnlockManager unlockManager;

        [Header("UI Elements")]
        [SerializeField]
        private Transform characterGridParent;

        [SerializeField]
        private GameObject characterButtonPrefab;

        [SerializeField]
        private Image p1Portrait;

        [SerializeField]
        private Image p2Portrait;

        [SerializeField]
        private Text p1NameText;

        [SerializeField]
        private Text p2NameText;

        [Header("Events")]
        public UnityEvent<int, int> OnCharacterSelected; // playerIndex, characterIndex
        public UnityEvent<int, int> OnColorChanged;      // playerIndex, colorIndex
        public UnityEvent<int> OnConfirmed;              // playerIndex
        public UnityEvent OnBothConfirmed;

        private int p1Index;
        private int p2Index;
        private int p1Color;
        private int p2Color;
        private bool p1Locked;
        private bool p2Locked;

        private void Start()
        {
            if (database == null)
                database = FindObjectOfType<CharacterDatabase>();
            if (unlockManager == null)
                unlockManager = FindObjectOfType<CharacterUnlockManager>();

            BuildGrid();
            RefreshUI(0);
            RefreshUI(1);
        }

        private void BuildGrid()
        {
            if (database == null || characterGridParent == null || characterButtonPrefab == null) return;

            for (int i = 0; i < database.CharacterCount; i++)
            {
                var character = database.GetCharacterByIndex(i);
                if (character == null) continue;

                var buttonObj = Instantiate(characterButtonPrefab, characterGridParent);
                var button = buttonObj.GetComponent<Button>();
                var image = buttonObj.GetComponent<Image>();

                if (image != null && character.Icon != null)
                    image.sprite = character.Icon;

                bool isUnlocked = unlockManager?.IsCharacterUnlocked(character.CharacterId) ?? true;
                button.interactable = isUnlocked;

                int capturedIndex = i;
                button.onClick.AddListener(() => SelectCharacter(capturedIndex));
            }
        }

        /// <summary>
        /// Navigates character selection for a player.
        /// </summary>
        public void Navigate(int playerIndex, int direction)
        {
            if (playerIndex == 0 && p1Locked) return;
            if (playerIndex == 1 && p2Locked) return;

            int current = playerIndex == 0 ? p1Index : p2Index;
            int count = database?.CharacterCount ?? 0;
            if (count == 0) return;

            int next = current + direction;
            next = ((next % count) + count) % count;

            if (playerIndex == 0)
                p1Index = next;
            else
                p2Index = next;

            OnCharacterSelected?.Invoke(playerIndex, next);
            RefreshUI(playerIndex);
        }

        /// <summary>
        /// Cycles color palette for a player.
        /// </summary>
        public void CycleColor(int playerIndex, int direction)
        {
            if (playerIndex == 0 && p1Locked) return;
            if (playerIndex == 1 && p2Locked) return;

            var character = database?.GetCharacterByIndex(playerIndex == 0 ? p1Index : p2Index);
            if (character == null) return;

            int current = playerIndex == 0 ? p1Color : p2Color;
            int paletteCount = character.ColorPalettes?.Count ?? 1;
            if (paletteCount == 0) return;

            int next = current + direction;
            next = ((next % paletteCount) + paletteCount) % paletteCount;

            if (playerIndex == 0)
                p1Color = next;
            else
                p2Color = next;

            OnColorChanged?.Invoke(playerIndex, next);
            RefreshUI(playerIndex);
        }

        /// <summary>
        /// Locks in a player's selection.
        /// </summary>
        public void ConfirmSelection(int playerIndex)
        {
            if (playerIndex == 0)
                p1Locked = true;
            else
                p2Locked = true;

            OnConfirmed?.Invoke(playerIndex);

            if (p1Locked && p2Locked)
                OnBothConfirmed?.Invoke();
        }

        /// <summary>
        /// Direct selection by index (mouse/touch).
        /// </summary>
        public void SelectCharacter(int index)
        {
            if (!p1Locked)
            {
                p1Index = index;
                OnCharacterSelected?.Invoke(0, index);
                RefreshUI(0);
            }
            else if (!p2Locked)
            {
                p2Index = index;
                OnCharacterSelected?.Invoke(1, index);
                RefreshUI(1);
            }
        }

        private void RefreshUI(int playerIndex)
        {
            int charIndex = playerIndex == 0 ? p1Index : p2Index;
            int colorIndex = playerIndex == 0 ? p1Color : p2Color;

            var character = database?.GetCharacterByIndex(charIndex);
            if (character == null) return;

            var portrait = playerIndex == 0 ? p1Portrait : p2Portrait;
            var nameText = playerIndex == 0 ? p1NameText : p2NameText;

            if (portrait != null)
                portrait.sprite = character.Portrait;

            if (nameText != null)
                nameText.text = character.DisplayName;

            // Apply color tint to portrait preview
            if (character.ColorPalettes != null && colorIndex < character.ColorPalettes.Count)
            {
                var palette = character.ColorPalettes[colorIndex];
                if (portrait != null && palette?.TintColor != null)
                {
                    portrait.color = palette.TintColor;
                }
            }
        }

        /// <summary>
        /// Gets the selected character data for a player.
        /// </summary>
        public CharacterRosterData GetSelectedCharacter(int playerIndex)
        {
            int index = playerIndex == 0 ? p1Index : p2Index;
            return database?.GetCharacterByIndex(index);
        }

        /// <summary>
        /// Gets the selected color index for a player.
        /// </summary>
        public int GetSelectedColor(int playerIndex)
        {
            return playerIndex == 0 ? p1Color : p2Color;
        }

        /// <summary>
        /// Resets selections for a new select screen session.
        /// </summary>
        public void ResetSelections()
        {
            p1Index = 0;
            p2Index = 0;
            p1Color = 0;
            p2Color = 0;
            p1Locked = false;
            p2Locked = false;
            RefreshUI(0);
            RefreshUI(1);
        }
    }
}
