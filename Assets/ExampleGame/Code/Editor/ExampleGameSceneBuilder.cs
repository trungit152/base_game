using ExampleGame.Bootstrap;
using ExampleGame.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ExampleGame.EditorTools
{
    /// <summary>
    /// Menu Editor dựng sẵn scene test cho ExampleGame. Để CHÍNH Unity tạo GameObject/Component (GUID,
    /// fileID, và phần serialize của LifetimeScope đều do Unity sinh) — chắc chắn đúng hơn nhiều so với
    /// viết tay file <c>.unity</c>.
    /// </summary>
    public static class ExampleGameSceneBuilder
    {
        private const string ScenePath = "Assets/ExampleGame/ExampleGame.unity";

        [MenuItem("Tools/ExampleGame/Tạo Scene Test")]
        public static void CreateTestScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var go = new GameObject("ExampleGame");
            go.AddComponent<ExampleGameLifetimeScope>(); // composition root (autoRun -> build container khi Play)
            go.AddComponent<ExampleGameDebugHud>();       // HUD nút bấm để test

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log("[ExampleGame] Đã tạo scene test tại '" + ScenePath + "'. Bấm Play rồi dùng HUD ở góc trên trái.");
        }
    }
}
