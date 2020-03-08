using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BloodBond {
    [CustomEditor(typeof(MapInteract))]
    [CanEditMultipleObjects]
    public class MapInteractEditor : Editor
    {

        MapInteract m_Target;

        ReorderableList VCameralist;
        ReorderableList Interactlist;


        void OnEnable()
        {
            var serProp = this.serializedObject.FindProperty("VCameraPoints");
            this.VCameralist = new ReorderableList(serProp.serializedObject, serProp, true, true, true, true);
            this.VCameralist.drawHeaderCallback = this.DrawVCameraPointListHeader;
            this.VCameralist.drawElementCallback = this.DrawVCameraPointListElement;

            serProp = this.serializedObject.FindProperty("interactPoints");
            this.Interactlist = new ReorderableList(serProp.serializedObject, serProp, true, true, true, true);
            this.Interactlist.drawHeaderCallback = this.DrawInteractPointListHeader;
            this.Interactlist.drawElementCallback = this.DrawInteractPointListElement;

        }
        void DrawVCameraPointListHeader(Rect rect)
        {
            var spacing = 10f;
            var arect = rect;
            arect.height = EditorGUIUtility.singleLineHeight;
            arect.x += 15;
            arect.width = 120;
            EditorGUI.LabelField(arect, "ChangeLine");
            arect.x += arect.width + spacing;
            arect.width = 80;
            EditorGUI.LabelField(arect, "Distance");
            arect.x += arect.width + spacing;
            arect.width = 120;
            EditorGUI.LabelField(arect, "LastVCamera");
            arect.x += arect.width + spacing;
            arect.width = 120;
            EditorGUI.LabelField(arect, "NextVCamera");
        }

        void DrawVCameraPointListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var spacing = 10f;
            var arect = rect;
            var serElem = this.VCameralist.serializedProperty.GetArrayElementAtIndex(index);
            arect.height = EditorGUIUtility.singleLineHeight;
            arect.width = 120;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("colliderPoint"), GUIContent.none);
            arect.x += arect.width + spacing;
            arect.width = 80;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("distance"), GUIContent.none);
            arect.x += arect.width + spacing;
            arect.width = 120;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("lastVCamera"), GUIContent.none);
            arect.x += arect.width + spacing;
            arect.width = 120;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("nextVCamera"), GUIContent.none);
        }

        void DrawInteractPointListHeader(Rect rect)
        {
            var spacing = 10f;
            var arect = rect;
            arect.height = EditorGUIUtility.singleLineHeight;
            arect.x += 15;
            arect.width = 100;
            EditorGUI.LabelField(arect, "ColliderPoint");
            arect.x += arect.width + spacing;
            arect.width = 100;
            EditorGUI.LabelField(arect, "InteractType");
            arect.x += arect.width + spacing;
            arect.width = 100;
            EditorGUI.LabelField(arect, "Timeline");
            arect.x += arect.width + spacing;
            arect.width = 100;
            EditorGUI.LabelField(arect, "InfoText");
            arect.x += arect.width + spacing;
            arect.width = 100;
            EditorGUI.LabelField(arect, "ShowTextTime");
        }

        void DrawInteractPointListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var spacing = 10f;
            var arect = rect;
            var serElem = this.Interactlist.serializedProperty.GetArrayElementAtIndex(index);
            arect.height = EditorGUIUtility.singleLineHeight;
            arect.width = 100;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("colliderPoint"), GUIContent.none);
            arect.x += arect.width + spacing;
            arect.width = 100;
            EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("interactType"), GUIContent.none);

            if (serElem.FindPropertyRelative("interactType").enumValueIndex < 2)
            {
                if (serElem.FindPropertyRelative("interactType").enumValueIndex == 0)
                {
                    arect.x += (arect.width + spacing);
                    arect.width = 100;
                    EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("timeline"), GUIContent.none);
                }
                else if (serElem.FindPropertyRelative("interactType").enumValueIndex == 1)
                {
                    arect.x += (arect.width + spacing);
                    arect.x += (arect.width + spacing);
                    arect.width = 100;
                    EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("infoText"), GUIContent.none);
                }

            }
            else
            {

                arect.x += (arect.width + spacing);
                arect.width = 100;
                EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("timeline"), GUIContent.none);
                arect.x += (arect.width + spacing);
                arect.width = 100;
                EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("infoText"), GUIContent.none);
                arect.x += (arect.width + spacing);
                arect.width = 100;
                EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("textShowTime"), GUIContent.none);
                serElem.FindPropertyRelative("textShowTime").floatValue = Mathf.Clamp01(serElem.FindPropertyRelative("textShowTime").floatValue);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.serializedObject.Update();
            var property = VCameralist.serializedProperty;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
            if (property.isExpanded)
            {
                this.VCameralist.DoLayoutList();
            }
            property = Interactlist.serializedProperty;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
            if (property.isExpanded)
            {
                this.Interactlist.DoLayoutList();
            }
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}


