using System;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Harpia.CampingPack
{
    public class CampingPackWelcome : MonoBehaviour
    {
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CampingPackWelcome))]
    public class CampingPackWelcomeEditor : Editor
    {
        public static Texture2D test;
        private Texture2D _downloadedTexture;
        

        async void LoadTexture(string link, Texture2D tex)
        {
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(link))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone) await System.Threading.Tasks.Task.Yield();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    _downloadedTexture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(request);
                    tex = new Texture2D(_downloadedTexture.width, _downloadedTexture.height);
                    tex.SetPixels(_downloadedTexture.GetPixels());
                    tex.Apply();
                    Debug.Log("Texture loaded successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to load texture: {request.error}");
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Thank You for Using Camping Pack!", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("\u2665", new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter });
            GUILayout.Space(10);

            GUILayout.Label("Check out our other tools:");
            GUILayout.Space(5);
            
            
            if (GUILayout.Button("Level Design Tool"))
            {
                Application.OpenURL(CampingPackCanvas.prefabBrushLink);
            }

            if (GUILayout.Button("Icon Creator Tool"))
            {
                Application.OpenURL(CampingPackCanvas.iconCreatorLink);
            }

            if (GUILayout.Button("Change Low Poly Colors Tool"))
            {
                Application.OpenURL(CampingPackCanvas.lowPolyColorChangerLink);
            }

            if (GUILayout.Button("Animation Events Tool"))
            {
                Application.OpenURL(CampingPackCanvas.quickAnimationEventsLink);
            }
        }
    }
#endif
}