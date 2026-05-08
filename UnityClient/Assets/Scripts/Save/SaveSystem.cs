using System;
using System.IO;
using UnityEngine;

namespace StreetFighter.Save
{
    /// <summary>
    /// Provides file-based JSON persistence for runtime data.
    /// </summary>
    public class SaveSystem : ISaveService
    {
        private SaveSettings settings;

        /// <inheritdoc />
        public void Initialize(SaveSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
        }

        /// <inheritdoc />
        public bool Save<T>(string key, T record) where T : class
        {
            var filePath = GetFilePath(key);
            try
            {
                var json = JsonUtility.ToJson(record, true);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save '{key}': {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public T Load<T>(string key) where T : class, new()
        {
            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
            {
                return new T();
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load '{key}': {ex.Message}");
                return new T();
            }
        }

        private string GetFilePath(string key)
        {
            return Path.Combine(Application.persistentDataPath, $"{settings.FilePrefix}_{key}.json");
        }
    }
}
