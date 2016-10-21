using System;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [CustomEditor (typeof(EdgeDetection))]
    class EdgeDetectionEditor : Editor
    {
        SerializedObject serObj;

        SerializedProperty mode;
        SerializedProperty sensitivityDepth;
        SerializedProperty sensitivityNormals;

        SerializedProperty lumThreshold;

        SerializedProperty edgesOnly;
        SerializedProperty edgesOnlyBgColor;

        SerializedProperty edgeExp;
        SerializedProperty sampleDist;

		SerializedProperty depthEffect;
		SerializedProperty BGdepthEffectRange;

        void OnEnable () {
            serObj = new SerializedObject (target);

            mode = serObj.FindProperty("mode");

            sensitivityDepth = serObj.FindProperty("sensitivityDepth");
            sensitivityNormals = serObj.FindProperty("sensitivityNormals");

            lumThreshold = serObj.FindProperty("lumThreshold");

            edgesOnly = serObj.FindProperty("edgesOnly");
            edgesOnlyBgColor = serObj.FindProperty("edgesOnlyBgColor");

            edgeExp = serObj.FindProperty("edgeExp");
            sampleDist = serObj.FindProperty("sampleDist");

			depthEffect = serObj.FindProperty ("depthEffect");
			BGdepthEffectRange = serObj.FindProperty ("BGdepthEffectRange");
        }


        public override void OnInspectorGUI () {
            serObj.Update ();

            GUILayout.Label("Detects spatial differences and converts into black outlines", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField (mode, new GUIContent("Mode"));

            if (mode.intValue < 2) {
                EditorGUILayout.PropertyField (sensitivityDepth, new GUIContent(" Depth Sensitivity"));
                EditorGUILayout.PropertyField (sensitivityNormals, new GUIContent(" Normals Sensitivity"));
				EditorGUILayout.PropertyField (depthEffect, new GUIContent (" Depth Effect"));
				EditorGUILayout.PropertyField (BGdepthEffectRange, new GUIContent (" BG Depth Effect Range"));
            }
            else if (mode.intValue < 4) {
                EditorGUILayout.PropertyField (edgeExp, new GUIContent(" Edge Exponent"));
            }
            else {
                // lum based mode
                EditorGUILayout.PropertyField (lumThreshold, new GUIContent(" Luminance Threshold"));
            }

            EditorGUILayout.PropertyField (sampleDist, new GUIContent(" Sample Distance"));

            EditorGUILayout.Separator ();

            GUILayout.Label ("Background Options");
            edgesOnly.floatValue = EditorGUILayout.Slider (" Edges only", edgesOnly.floatValue, 0.0f, 1.0f);
            EditorGUILayout.PropertyField (edgesOnlyBgColor, new GUIContent (" Color"));

            serObj.ApplyModifiedProperties();
        }
    }
}
