using Data;
using UnityEditor;
using UnityEngine;

namespace EditorScripts
{
    [CustomEditor(typeof(MapSettings))]
    public class MapSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var settings = (MapSettings) target;

            if (GUILayout.Button("Create planets on scene", EditorStyles.miniButtonLeft))
            {
                settings.CreatePlanets();
            }
            
            if (GUILayout.Button("Create collectable stars on scene", EditorStyles.miniButtonLeft))
            {
                settings.CreateStars();
            }
        }

    }
}