using System;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(TintTextureGenerator))]
    public class TintTextureGeneratorEditor : Editor
    {
        private const int k_TextureSizeLimit = 4096;
        private SerializedProperty m_ScaleFactor;

        private SerializedProperty m_TintMapSize;

        public void OnEnable()
        {
            m_TintMapSize = serializedObject.FindProperty("k_TintMapSize");
            m_ScaleFactor = serializedObject.FindProperty("k_ScaleFactor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.IntSlider(m_TintMapSize, 256, k_TextureSizeLimit, Styles.tintMapSize);
            EditorGUILayout.IntSlider(m_ScaleFactor, 1,
                Math.Min(8, Math.Max(1, k_TextureSizeLimit / m_TintMapSize.intValue)), Styles.scaleFactor);
            serializedObject.ApplyModifiedProperties();
        }

        private static class Styles
        {
            public static readonly GUIContent tintMapSize = new("Tint Map Size", "Size of the Tint Map in cells");

            public static readonly GUIContent scaleFactor = new("Scale Factor",
                "Mapping scale for cells to Tint map texture. Adjust to get better definition.");
        }
    }
}