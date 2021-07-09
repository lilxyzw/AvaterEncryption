﻿using UnityEditor;
using UnityEngine;

namespace GeoTetra.GTAvaCrypt
{
    [CustomEditor(typeof(AvaCryptRoot))]
    [CanEditMultipleObjects]
    public class AvaCryptRootEditor : Editor 
    {
        SerializedProperty _distortRatioProperty;
        SerializedProperty _key0Property;
        SerializedProperty _key1Property;
        SerializedProperty _key2Property;
        SerializedProperty _key3Property;

        void OnEnable()
        {
            _distortRatioProperty = serializedObject.FindProperty("_distortRatio");
            _key0Property = serializedObject.FindProperty("_key0");
            _key1Property = serializedObject.FindProperty("_key1");
            _key2Property = serializedObject.FindProperty("_key2");
            _key3Property = serializedObject.FindProperty("_key3");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AvaCryptRoot avaCryptRoot = target as AvaCryptRoot;
            EditorGUILayout.PropertyField(_distortRatioProperty);
            EditorGUILayout.PropertyField(_key0Property);
            EditorGUILayout.PropertyField(_key1Property);
            EditorGUILayout.PropertyField(_key2Property);
            EditorGUILayout.PropertyField(_key3Property);
            if (GUILayout.Button("Encrypt Avatar"))
            {
                avaCryptRoot.EncryptAvatar();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}