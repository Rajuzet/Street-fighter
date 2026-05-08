#if UNITY_EDITOR
using System.IO;
using StreetFighter.Audio;
using StreetFighter.Characters;
using StreetFighter.Core;
using StreetFighter.Input;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StreetFighter.EditorTools
{
    /// <summary>
    /// Editor utility to construct a multiplayer-ready player prefab with required components.
    /// </summary>
    public static class PlayerPrefabBuilderEditor
    {
        [MenuItem("StreetFighter/Build Player Prefab")]
        public static void BuildPlayerPrefab()
        {
            var playerRoot = new GameObject("Player_Root");
            var controller = playerRoot.AddComponent<CharacterController>();
            controller.height = 1.9f;
            controller.radius = 0.35f;

            var playerController = playerRoot.AddComponent<PlayerController>();
            var combatController = playerRoot.AddComponent<PlayerCombatController>();
            var animationController = playerRoot.AddComponent<CharacterAnimationController>();

            var cameraRigObject = new GameObject("CameraRig");
            cameraRigObject.transform.SetParent(playerRoot.transform, false);
            var cameraRig = cameraRigObject.AddComponent<StreetFighter.Camera.ThirdPersonCameraRig>();
            playerController.GetType().GetField("cameraRig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(playerController, cameraRig);
            playerController.GetType().GetField("animationController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(playerController, animationController);
            combatController.GetType().GetField("animationController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(combatController, animationController);

            var prefabDirectory = "Assets/Prefabs/Player";
            if (!Directory.Exists(prefabDirectory))
            {
                Directory.CreateDirectory(prefabDirectory);
            }

            var prefabPath = Path.Combine(prefabDirectory, "PlayerCharacter.prefab");
            PrefabUtility.SaveAsPrefabAsset(playerRoot, prefabPath);
            Object.DestroyImmediate(playerRoot);
            AssetDatabase.SaveAssets();
            Debug.Log($"Player prefab created at {prefabPath}");
        }
    }
}
#endif
