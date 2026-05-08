using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace StreetFighter.Core
{
    /// <summary>
    /// Handles asynchronous scene loading through Addressables.
    /// </summary>
    public class SceneLoader : ISceneLoader
    {
        /// <inheritdoc />
        public async Task LoadSceneAsync(string addressableLabel, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = Addressables.LoadSceneAsync(addressableLabel, mode, activateOnLoad: true);
            await handle.Task.ConfigureAwait(false);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load scene '{addressableLabel}' with status {handle.Status}");
            }
        }

        /// <inheritdoc />
        public async Task UnloadSceneAsync(string addressableLabel)
        {
            var scene = SceneManager.GetSceneByName(addressableLabel);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene '{addressableLabel}' is not loaded.");
                return;
            }

            var handle = Addressables.UnloadSceneAsync(scene);
            await handle.Task.ConfigureAwait(false);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to unload scene '{addressableLabel}' with status {handle.Status}");
            }
        }
    }

    /// <summary>
    /// Contract for scene loading services.
    /// </summary>
    public interface ISceneLoader
    {
        Task LoadSceneAsync(string addressableLabel, LoadSceneMode mode = LoadSceneMode.Single);
        Task UnloadSceneAsync(string addressableLabel);
    }
}
