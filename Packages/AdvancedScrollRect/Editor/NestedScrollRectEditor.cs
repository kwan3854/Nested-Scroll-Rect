using System.Linq;
using AdvancedScrollRect.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedScrollRect.Editor
{
    [CustomEditor(typeof(NestedScrollRect))]
    [CanEditMultipleObjects]
    public class NestedScrollRectEditor : UnityEditor.Editor
    {
        private SerializedProperty _parentScrollRectProp;

        private void OnEnable()
        {
            _parentScrollRectProp = serializedObject.FindProperty("parentScrollRect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_parentScrollRectProp, new GUIContent("Parent Scroll Rect"));
            
            if (_parentScrollRectProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Parent ScrollRect is not assigned. " +
                    "You can assign it manually." +
                    "or you can find the nearest parent ScrollRect.",
                    MessageType.Info);

                if (GUILayout.Button("Find nearest parent ScrollRect"))
                {
                    foreach (var t in targets)
                    {
                        var nested = t as NestedScrollRect;
                        if (nested == null) continue;

                        var found = nested.GetComponentsInParent<ScrollRect>(true)
                            .FirstOrDefault(s => s != nested);

                        if (found != null)
                        {
                            _parentScrollRectProp.objectReferenceValue = found;
                        }
                    }
                }
            }
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "parentScrollRect");
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}