using UnityEngine;

namespace StreetFighter.Core
{
    public class GameInitializer : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            ServiceRegistry.RegisterAllServices();
            SceneFlowManager.Initialize();
            InputManager.Setup();
            AudioManager.Instance.Initialize();
        }
    }
}
