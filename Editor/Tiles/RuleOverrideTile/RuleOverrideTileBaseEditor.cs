using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomEditor(typeof(RuleOverrideTileBase))]
    public class RuleOverrideTileBaseEditor : Editor
    {

        public RuleOverrideTileBase overrideTile { get { return (target as RuleOverrideTileBase); } }
        public RuleTileEditor ruleTileEditor
        {
            get
            {
                if (m_RuleTileEditorTile != overrideTile.m_Tile)
                {
                    DestroyImmediate(m_RuleTileEditor);
                    m_RuleTileEditor = Editor.CreateEditor(overrideTile.m_InstanceTile) as RuleTileEditor;
                    m_RuleTileEditorTile = overrideTile.m_Tile;
                }
                return m_RuleTileEditor;
            }
        }

        RuleTileEditor m_RuleTileEditor;
        RuleTile m_RuleTileEditorTile;

        public virtual void OnDisable()
        {
            DestroyImmediate(ruleTileEditor);
            m_RuleTileEditorTile = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawSourceTileField();
            DrawCustomFields();
        }

        public void DrawSourceTileField()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Tile"));
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
                SaveTile();
        }
        public void DrawCustomFields()
        {
            if (overrideTile.m_InstanceTile)
            {
                SerializedObject instanceTileSerializedObject = new SerializedObject(overrideTile.m_InstanceTile);
                overrideTile.m_InstanceTile.hideFlags = HideFlags.None;
                RuleTileEditor.DrawCustomFields(overrideTile.m_InstanceTile, instanceTileSerializedObject);
                overrideTile.m_InstanceTile.hideFlags = HideFlags.NotEditable;
                instanceTileSerializedObject.ApplyModifiedProperties();
            }
        }

        public void SaveInstanceTileAsset()
        {
            bool assetChanged = false;

            if (overrideTile.m_InstanceTile)
            {
                if (!overrideTile.m_Tile || overrideTile.m_InstanceTile.GetType() != overrideTile.m_Tile.GetType())
                {
                    DestroyImmediate(overrideTile.m_InstanceTile, true);
                    overrideTile.m_InstanceTile = null;
                    assetChanged = true;
                }
            }
            if (!overrideTile.m_InstanceTile)
            {
                if (overrideTile.m_Tile)
                {
                    var t = overrideTile.m_Tile.GetType();
                    RuleTile instanceTile = ScriptableObject.CreateInstance(t) as RuleTile;
                    instanceTile.hideFlags = HideFlags.NotEditable;
                    AssetDatabase.AddObjectToAsset(instanceTile, overrideTile);
                    overrideTile.m_InstanceTile = instanceTile;
                    assetChanged = true;
                }
            }

            if (overrideTile.m_InstanceTile)
            {
                string instanceTileName = overrideTile.m_Tile.name + " (Override)";
                if (overrideTile.m_InstanceTile.name != instanceTileName)
                {
                    overrideTile.m_InstanceTile.name = instanceTileName;
                    assetChanged = true;
                }
            }

            if (assetChanged)
                AssetDatabase.SaveAssets();
        }

        public void SaveTile()
        {
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();

            SaveInstanceTileAsset();
            if (overrideTile.m_InstanceTile)
            {
                overrideTile.Override();
                RuleTileEditor.UpdateAffectedOverrideTiles(overrideTile.m_InstanceTile);
            }
        }
    }
}
