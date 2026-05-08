using StreetFighter.Audio;
using StreetFighter.Events;
using StreetFighter.Input;
using StreetFighter.Save;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Initializes core runtime systems and registers dependencies.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private SaveSettings saveSettings = null;

        [SerializeField]
        private AudioBank runtimeAudioBank = null;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            ServiceLocator.Reset();
            RegisterCoreServices();
            SceneFlowManager.Initialize();
            ServiceLocator.Resolve<IInputService>()?.Initialize();
            ServiceLocator.Resolve<IAudioService>()?.Initialize(runtimeAudioBank);
            ServiceLocator.Resolve<ISaveService>()?.Initialize(saveSettings);
            GameStateManager.Instance.SetState(GameState.Bootstrap);
        }

        private void RegisterCoreServices()
        {
            ServiceLocator.Register<IInputService>(new InputManager());
            ServiceLocator.Register<ISceneLoader>(new SceneLoader());
            ServiceLocator.Register<IAudioService>(new AudioManager());
            ServiceLocator.Register<ISaveService>(new SaveSystem());
            ServiceLocator.Register<IEventBus>(new EventBus());
            ServiceLocator.Register<GameStateManager>(GameStateManager.Instance);
        }
    }
}
