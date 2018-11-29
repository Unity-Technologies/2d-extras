using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    [CustomEditor(typeof(RuleTile), true)]
    [CanEditMultipleObjects]
    internal class RuleTileEditor : Editor
    {
        internal const float k_DefaultElementHeight = 48f;
        internal const float k_PaddingBetweenRules = 26f;
        internal const float k_SingleLineHeight = 16f;
        internal const float k_ObjectFieldLineHeight = 20f;
        internal const float k_LabelWidth = 80f;

        private const string s_MirrorX = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
        private const string s_MirrorY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
        private const string s_Rotated = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";

        private static Texture2D[] s_AutoTransforms;
        public static Texture2D[] autoTransforms
        {
            get
            {
                if (s_AutoTransforms == null)
                {
                    s_AutoTransforms = new Texture2D[3];
                    s_AutoTransforms[0] = RuleTile.Base64ToTexture(s_Rotated);
                    s_AutoTransforms[1] = RuleTile.Base64ToTexture(s_MirrorX);
                    s_AutoTransforms[2] = RuleTile.Base64ToTexture(s_MirrorY);
                }
                return s_AutoTransforms;
            }
        }

        private RuleTile tile { get { return (target as RuleTile); } }

        private ReorderableList m_ReorderableList;

        public void OnEnable()
        {
            if (tile.m_TilingRules == null)
                tile.m_TilingRules = new List<RuleTile.TilingRule>();

            m_ReorderableList = new ReorderableList(tile.m_TilingRules, typeof(RuleTile.TilingRule), true, true, true, true);
            m_ReorderableList.drawHeaderCallback = OnDrawHeader;
            m_ReorderableList.drawElementCallback = OnDrawElement;
            m_ReorderableList.elementHeightCallback = GetElementHeight;
            m_ReorderableList.onReorderCallback = ListUpdated;
            m_ReorderableList.onAddCallback = OnAddElement;
        }

        private void ListUpdated(ReorderableList list)
        {
            SaveTile();
        }

        private float GetElementHeight(int index)
        {
            if (tile.m_TilingRules != null && tile.m_TilingRules.Count > 0)
            {
                switch (tile.m_TilingRules[index].m_Output)
                {
                    case RuleTile.TilingRule.OutputSprite.Random:
                        return k_DefaultElementHeight + k_SingleLineHeight*(tile.m_TilingRules[index].m_Sprites.Length + 3) + k_PaddingBetweenRules;
                    case RuleTile.TilingRule.OutputSprite.Animation:
                        return k_DefaultElementHeight + k_SingleLineHeight*(tile.m_TilingRules[index].m_Sprites.Length + 2) + k_PaddingBetweenRules;
                }
            }
            return k_DefaultElementHeight + k_PaddingBetweenRules;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            RuleTile.TilingRule rule = tile.m_TilingRules[index];

            float yPos = rect.yMin + 2f;
            float height = rect.height - k_PaddingBetweenRules;
            float matrixWidth = k_DefaultElementHeight;
            
            Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
            Rect matrixRect = new Rect(rect.xMax - matrixWidth * 2f - 10f, yPos, matrixWidth, k_DefaultElementHeight);
            Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

            EditorGUI.BeginChangeCheck();
            RuleInspectorOnGUI(inspectorRect, rule);
            DoRuleMatrixOnGUI(tile, matrixRect, rule);
            SpriteOnGUI(spriteRect, rule);
            if (EditorGUI.EndChangeCheck())
                SaveTile();
        }

        private void OnAddElement(ReorderableList list)
        {
            RuleTile.TilingRule rule = new RuleTile.TilingRule();
            rule.m_Output = RuleTile.TilingRule.OutputSprite.Single;
            rule.m_Sprites[0] = tile.m_DefaultSprite;
            rule.m_GameObject = tile.m_DefaultGameObject;
            rule.m_ColliderType = tile.m_DefaultColliderType;
            tile.m_TilingRules.Add(rule);
        }

        private void SaveTile()
        {
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Tiling Rules");
        }

        public override void OnInspectorGUI()
        {
            tile.m_DefaultSprite = EditorGUILayout.ObjectField("Default Sprite", tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            tile.m_DefaultGameObject = EditorGUILayout.ObjectField("Default Game Object", tile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            tile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", tile.m_DefaultColliderType);

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var baseFields = typeof(RuleTile).GetFields().Select(field => field.Name);
            var fields = target.GetType().GetFields().Select(field => field.Name).Where(field => !baseFields.Contains(field));
            foreach (var field in fields)
                EditorGUILayout.PropertyField(serializedObject.FindProperty(field), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (m_ReorderableList != null && tile.m_TilingRules != null)
                m_ReorderableList.DoLayoutList();
        }

        protected virtual void DoRuleMatrixOnGUI(RuleTile tile, Rect rect, RuleTile.TilingRule tilingRule)
        {
            RuleMatrixOnGUI(tile, rect, tilingRule);
        }

        internal static void RuleMatrixOnGUI(RuleTile tile, Rect rect, RuleTile.TilingRule tilingRule)
        {
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            int index = 0;
            float w = rect.width / 3f;
            float h = rect.height / 3f;

            for (int y = 0; y <= 3; y++)
            {
                float top = rect.yMin + y * h;
                Handles.DrawLine(new Vector3(rect.xMin, top), new Vector3(rect.xMax, top));
            }
            for (int x = 0; x <= 3; x++)
            {
                float left = rect.xMin + x * w;
                Handles.DrawLine(new Vector3(left, rect.yMin), new Vector3(left, rect.yMax));
            }
            Handles.color = Color.white;

            for (int y = 0; y <= 2; y++)
            {
                for (int x = 0; x <= 2; x++)
                {
                    Rect r = new Rect(rect.xMin + x * w, rect.yMin + y * h, w - 1, h - 1);
                    if (x != 1 || y != 1)
                    {
                        tile.RuleOnGUI(r, new Vector2Int(x, y), tilingRule.m_Neighbors[index]);
                        if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                        {
                            int change = 1;
                            if (Event.current.button == 1)
                                change = -1;

                            var allConsts = tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                            var neighbors = allConsts.Select(c => (int)c.GetValue(null)).ToList();
                            neighbors.Sort();

                            int oldIndex = neighbors.IndexOf(tilingRule.m_Neighbors[index]);
                            int newIndex = (int)Mathf.Repeat(oldIndex + change, neighbors.Count);
                            tilingRule.m_Neighbors[index] = neighbors[newIndex];
                            GUI.changed = true;
                            Event.current.Use();
                        }

                        index++;
                    }
                    else
                    {
                        switch (tilingRule.m_RuleTransform)
                        {
                            case RuleTile.TilingRule.Transform.Rotated:
                                GUI.DrawTexture(r, autoTransforms[0]);
                                break;
                            case RuleTile.TilingRule.Transform.MirrorX:
                                GUI.DrawTexture(r, autoTransforms[1]);
                                break;
                            case RuleTile.TilingRule.Transform.MirrorY:
                                GUI.DrawTexture(r, autoTransforms[2]);
                                break;
                        }

                        if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                        {
                            tilingRule.m_RuleTransform = (RuleTile.TilingRule.Transform)(((int)tilingRule.m_RuleTransform + 1) % 4);
                            GUI.changed = true;
                            Event.current.Use();
                        }
                    }
                }
            }
        }

        internal static void SpriteOnGUI(Rect rect, RuleTile.TilingRule tilingRule)
        {
            tilingRule.m_Sprites[0] = EditorGUI.ObjectField(new Rect(rect.xMax - rect.height, rect.yMin, rect.height, rect.height), tilingRule.m_Sprites[0], typeof (Sprite), false) as Sprite;
        }

        internal static void RuleInspectorOnGUI(Rect rect, RuleTile.TilingRule tilingRule)
        {
            float y = rect.yMin;
            EditorGUI.BeginChangeCheck();
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Game Object");
            tilingRule.m_GameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "", tilingRule.m_GameObject, typeof(GameObject), true);
            y += k_ObjectFieldLineHeight;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Rule");
            tilingRule.m_RuleTransform = (RuleTile.TilingRule.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_RuleTransform);
            y += k_SingleLineHeight;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
            tilingRule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_ColliderType);
            y += k_SingleLineHeight;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Output");
            tilingRule.m_Output = (RuleTile.TilingRule.OutputSprite)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Output);
            y += k_SingleLineHeight;

            if (tilingRule.m_Output == RuleTile.TilingRule.OutputSprite.Animation)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Speed");
                tilingRule.m_AnimationSpeed = EditorGUI.FloatField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_AnimationSpeed);
                y += k_SingleLineHeight;
            }
            if (tilingRule.m_Output == RuleTile.TilingRule.OutputSprite.Random)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Noise");
                tilingRule.m_PerlinScale = EditorGUI.Slider(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_PerlinScale, 0.001f, 0.999f);
                y += k_SingleLineHeight;

                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Shuffle");
                tilingRule.m_RandomTransform = (RuleTile.TilingRule.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_RandomTransform);
                y += k_SingleLineHeight;
            }

            if (tilingRule.m_Output != RuleTile.TilingRule.OutputSprite.Single)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Size");
                EditorGUI.BeginChangeCheck();
                int newLength = EditorGUI.DelayedIntField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites.Length);
                if (EditorGUI.EndChangeCheck())
                    Array.Resize(ref tilingRule.m_Sprites, Math.Max(newLength, 1));
                y += k_SingleLineHeight;

                for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
                {
                    tilingRule.m_Sprites[i] = EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites[i], typeof(Sprite), false) as Sprite;
                    y += k_SingleLineHeight;
                }
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (tile.m_DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] {typeof (Sprite), typeof (Color), typeof (int), typeof (int)});
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] {tile.m_DefaultSprite, Color.white, width, height});
                        if (ret is Texture2D)
                            return ret as Texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        private static Type GetType(string TypeName)
        {
            var type = Type.GetType(TypeName);
            if (type != null)
                return type;

            if (TypeName.Contains("."))
            {
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }
        
        [Serializable]
        class RuleTileRuleWrapper
        {
            [SerializeField]
            public List<RuleTile.TilingRule> rules = new List<RuleTile.TilingRule>();
        }
        
        [MenuItem("CONTEXT/RuleTile/Copy All Rules")]
        private static void CopyAllRules(MenuCommand item)
        {
            RuleTile tile = item.context as RuleTile;
            if (tile == null)
                return;
            
            RuleTileRuleWrapper rulesWrapper = new RuleTileRuleWrapper();
            rulesWrapper.rules = tile.m_TilingRules;
            var rulesJson = EditorJsonUtility.ToJson(rulesWrapper);
            EditorGUIUtility.systemCopyBuffer = rulesJson;
        }
        
        [MenuItem("CONTEXT/RuleTile/Paste Rules")]
        private static void PasteRules(MenuCommand item)
        {
            RuleTile tile = item.context as RuleTile;
            if (tile == null)
                return;

            try
            {
                RuleTileRuleWrapper rulesWrapper = new RuleTileRuleWrapper();
                EditorJsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, rulesWrapper);
                tile.m_TilingRules.AddRange(rulesWrapper.rules);
            }
            catch (Exception)
            {
                Debug.LogError("Unable to paste rules from system copy buffer");
            }
        }
    }
}
