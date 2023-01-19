using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    public static class TileAssetConverter
    {
        public static void Convert(List<TileBase> tileAssetsToConvert, Type tileType)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                var newTile = ScriptableObject.CreateInstance(tileType);
                var newSO = new SerializedObject(newTile);
                var scriptSP = newSO.FindProperty("m_Script");

                foreach (var tileAssetToConvert in tileAssetsToConvert)
                {
                    if (tileAssetToConvert == null)
                        continue;

                    if (tileAssetToConvert.GetType() == tileType)
                        continue;

                    var assetPath = AssetDatabase.GetAssetPath(tileAssetToConvert);
                    if (String.IsNullOrWhiteSpace(assetPath))
                    {
                        Debug.LogWarningFormat("{0} is not a valid asset and cannot be converted", tileAssetToConvert);
                        continue;
                    }

                    if (!AssetDatabase.IsMainAsset(tileAssetToConvert))
                    {
                        Debug.LogWarningFormat("{0} is not the main asset and cannot be converted", tileAssetToConvert);
                        continue;
                    }

                    var result = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tileAssetToConvert,
                        out var originalGuid, out long localId);
                    if (!result)
                    {
                        Debug.LogWarningFormat("Unable to get GUID for {0}", tileAssetToConvert);
                        continue;
                    }
                    var metaFilePath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
                    if (String.IsNullOrWhiteSpace(assetPath))
                    {
                        Debug.LogWarningFormat("{0} is not a valid asset and cannot be converted", tileAssetToConvert);
                        continue;
                    }

                    var oldSO = new SerializedObject(tileAssetToConvert);
                    oldSO.CopyFromSerializedProperty(scriptSP);
                    oldSO.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();    
            }
            AssetDatabase.Refresh();
        }
    }

    public class TileAssetConverterEditor : EditorWindow
    {
        private TypeCache.TypeCollection tileTypes;
        private List<String> tileTypeNames;

        private ListView convertingObjects;
        private DropdownField typeDropdown;

        private TileStore tileStore;
        private SerializedObject serializedTileStore;
        
        [Serializable]
        private class TileStore : ScriptableObject
        {
            [SerializeField]
            public List<TileBase> tilesToConvert = new List<TileBase>();
        }
        
        [MenuItem("Window/2D/Tile Asset Converter", false, 5)]
        public static void OpenTilemapPalette()
        {
            GetWindow<TileAssetConverterEditor>();
        }

        public void OnEnable()
        {
            tileTypes = TypeCache.GetTypesDerivedFrom<TileBase>();
            tileTypeNames = new List<string>(tileTypes.Count);
            foreach (var tileType in tileTypes)
            {
                tileTypeNames.Add(tileType.FullName);
            }
            tileStore = CreateInstance<TileStore>();
            serializedTileStore = new SerializedObject(tileStore);
        }

        private void OnDisable()
        {
            DestroyImmediate(tileStore);
        }

        public void CreateGUI()
        {
            var ve = new VisualElement();
            ve.style.flexDirection = FlexDirection.Column;

            typeDropdown = new DropdownField("Convert Type", tileTypeNames, "UnityEngine.Tilemaps.Tile");
            ve.Add(typeDropdown);
            
            convertingObjects = new ListView()
            {
                reorderable = false,
                showBorder = true,
                showFoldoutHeader = true,
                showBoundCollectionSize = true,
                headerTitle = "Tiles To Convert",
                showAddRemoveFooter = true,
                bindingPath = "tilesToConvert"
            };
            convertingObjects.Bind(serializedTileStore);
            ve.Add(convertingObjects);

            var foldout = convertingObjects.Q<Foldout>();
            foldout.value = true;

            var convertButton = new Button(ConvertTiles);
            convertButton.text = "Convert";
            ve.Add(convertButton);
            
            rootVisualElement.Add(ve);
        }

        private void ConvertTiles()
        {
            var tileType = tileTypes[typeDropdown.index];
            TileAssetConverter.Convert(tileStore.tilesToConvert, tileType);
            tileStore.tilesToConvert.Clear();
        }
    }
}