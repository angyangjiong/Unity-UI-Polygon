using System;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI.Extensions;
//https://bitbucket.org/Unity-Technologies/ui/src/0651862509331da4e85f519de88c99d0529493a5/UnityEditor.UI/UI/ImageEditor.cs
namespace UnityEditor.UI.Extensions
{
    [CustomEditor(typeof(Polygon), true)]
    [CanEditMultipleObjects]
    public class PolygonEditor: GraphicEditor
    {
        SerializedProperty _texture;
        SerializedProperty _fillCenter;
        SerializedProperty _thickness;
        SerializedProperty _rotation;

        SerializedProperty _sides;
        SerializedProperty _verticesDistances;

        GUIContent _textureContent;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textureContent = EditorGUIUtility.TrTextContent("Texture");

            _texture = serializedObject.FindProperty("_texture");
            _fillCenter = serializedObject.FindProperty("_fillCenter");
            _thickness = serializedObject.FindProperty("_thickness");
            _rotation = serializedObject.FindProperty("_rotation");

            _sides = serializedObject.FindProperty("_sides");
            _verticesDistances = serializedObject.FindProperty("_verticesDistances");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextureGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_fillCenter);
            if (!_fillCenter.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_thickness);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_rotation);

            EditorGUILayout.Space();

            if (EditorGUILayout.PropertyField(_verticesDistances))
            {
                EditorGUI.indentLevel++;

                _sides.intValue = EditorGUILayout.IntField("Sides", _sides.intValue);
                _sides.intValue = Mathf.Clamp(_sides.intValue, 3, 360);

                for (int i = 0; i < _verticesDistances.arraySize - 1; i++)
                {
                    EditorGUILayout.PropertyField(_verticesDistances.GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel--;

            }

            NativeSizeButtonGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected void TextureGUI()
        {
            //EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_texture, _textureContent);
            //if (EditorGUI.EndChangeCheck())
            //{
            //}
        }

        #region PreviewGUI

        //https://bitbucket.org/Unity-Technologies/ui/src/0651862509331da4e85f519de88c99d0529493a5/UnityEditor.UI/UI/RawImageEditor.cs

        private static readonly Rect uvRect = new Rect(0f, 0f, 1f, 1f);

        public override bool HasPreviewGUI() { return true; }

        private static Rect Outer(Polygon polygon)
        {
            Rect outer = uvRect;
            outer.xMin *= polygon.rectTransform.rect.width;
            outer.xMax *= polygon.rectTransform.rect.width;
            outer.yMin *= polygon.rectTransform.rect.height;
            outer.yMax *= polygon.rectTransform.rect.height;
            return outer;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            Polygon polygon = target as Polygon;
            if (polygon == null) return;

            Texture tex = polygon.mainTexture;

            if (tex == null)
                return;

            var outer = Outer(polygon);

            Type spriteDrawUtilityType = Assembly.Load("UnityEditor.UI").GetType("UnityEditor.UI.SpriteDrawUtility");
            
            //void DrawSprite(Texture tex, Rect drawArea, Vector4 padding, Rect outer, Rect inner, Rect uv, Color color, Material mat)
            MethodInfo mathodInfo = spriteDrawUtilityType.GetMethod("DrawSprite", BindingFlags.NonPublic | BindingFlags.Static);
            mathodInfo.Invoke(null, new object[] { tex , rect, Vector4.zero, outer, outer, uvRect, polygon.canvasRenderer.GetColor(), null });
        }

        #endregion

    }
}
