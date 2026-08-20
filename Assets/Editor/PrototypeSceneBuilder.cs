using CarpenterSim.Timber;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarpenterSim.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype.unity";

        public static void CreatePrototypeScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;

            GameObject lightObject = new GameObject("Directional Light");
            Light directionalLight = lightObject.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject cameraObject = new GameObject("Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(4.5f, 3.2f, -5.5f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.05f, 0f));
            camera.fieldOfView = 50f;

            GameObject timberObject = new GameObject("TimberPiece 48x98x4800");
            TimberPiece timber = timberObject.AddComponent<TimberPiece>();
            timber.Configure(48f, 98f, 4800f, TimberPiece.DefaultDensityKgPerCubicMetre);
            timberObject.transform.position = new Vector3(0f, timber.HeightMm / TimberPiece.MillimetresPerMetre * 0.5f, 0f);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new System.InvalidOperationException($"Failed to save prototype scene to {ScenePath}.");
            }

            Debug.Log($"Created CarpenterSim prototype scene at {ScenePath}");
        }
    }
}
