using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(AutoTileTextureImporter))]
    public class AutoTileTextureImporterEditor : Editor
    {
        private AutoTileTexture m_TargetAsset;
        private SerializedObject m_SerializedAssetObject;
        private string m_AssetPath;

        private AutoTileTexture autoTileTexture
        {
            get { return m_TargetAsset; }
        }

        private AutoTileTextureImporterElement m_MainElement;

        private string LoadSourceAsset()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var loadedObjects = InternalEditorUtility.LoadSerializedFileAndForget(assetPath);
            if (loadedObjects.Length > 0)
                m_TargetAsset = loadedObjects[0] as AutoTileTexture;
            return assetPath;
        }

        private SerializedObject serializedAssetObject
        {
            get
            {
                return GetSerializedAssetObject();
            }
        }

        private SerializedObject GetSerializedAssetObject()
        {
            if (m_SerializedAssetObject == null)
            {
                try
                {
                    m_SerializedAssetObject = new SerializedObject(autoTileTexture);
                }
                catch (System.ArgumentException e)
                {
                    m_SerializedAssetObject = null;
                    throw e;
                }
            }
            return m_SerializedAssetObject;
        }

        private void OnEnable()
        {
            LoadAutoTileTexture();
        }

        private void LoadAutoTileTexture()
        {
            m_AssetPath = LoadSourceAsset();
        }

        public override VisualElement CreateInspectorGUI()
        {
            m_MainElement = new AutoTileTextureImporterElement();
            m_MainElement.Bind(serializedAssetObject);
            m_MainElement.onRevert = Revert;
            m_MainElement.onApply = ApplyAndImport;
            return m_MainElement;
        }

        private void Revert()
        {
            m_SerializedAssetObject = null;
            LoadAutoTileTexture();
            m_MainElement.Bind(serializedAssetObject);
        }

        private void ApplyAndImport()
        {
            serializedAssetObject.ApplyModifiedPropertiesWithoutUndo();
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {
                (Object) autoTileTexture
            }, m_AssetPath, EditorSettings.serializationMode != SerializationMode.ForceBinary);
            AssetDatabase.ImportAsset(m_AssetPath);
        }
    }
}