using FIMSpace.FEditor;
using FIMSpace.FLook;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(FLookAnimatorWEyes))]
/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
public class FLookAnimatorWEyes_Editor : FLookAnimator_Editor
{
    protected SerializedProperty sp_eyeL;
    protected SerializedProperty sp_eyeLInv;
    protected SerializedProperty sp_eyeR;
    protected SerializedProperty sp_eyeRInv;
    protected SerializedProperty sp_head;
    protected SerializedProperty sp_eyesTarget;
    protected SerializedProperty sp_eyesSpeed;
    protected SerializedProperty sp_EyesOffsetRotation;
    protected SerializedProperty sp_eyesBlend;

    protected SerializedProperty sp_EyesXRange;
    protected SerializedProperty sp_EyesYRange;
    protected SerializedProperty sp_EyesNoKeyframes;

    static bool drawEyesSettings = true;

    private float eyesbothX = 70f;
    private float eyeslastBothX = 70f;
    private float eyesbothY = 60f;
    private float eyeslastBothY = 60f;

    protected override void OnEnable()
    {
        base.OnEnable();

        sp_eyeL = serializedObject.FindProperty("LeftEye");
        sp_eyeLInv = serializedObject.FindProperty("InvertLeftEye");
        sp_eyeR = serializedObject.FindProperty("RightEye");
        sp_eyeRInv = serializedObject.FindProperty("InvertRightEye");
        sp_head = serializedObject.FindProperty("HeadReference");
        sp_eyesTarget = serializedObject.FindProperty("EyesTarget");
        sp_eyesSpeed = serializedObject.FindProperty("EyesSpeed");
        sp_eyesBlend = serializedObject.FindProperty("EyesBlend");
        sp_EyesOffsetRotation = serializedObject.FindProperty("EyesOffsetRotation");
        

        sp_EyesXRange = serializedObject.FindProperty("EyesXRange");
        sp_EyesYRange = serializedObject.FindProperty("EyesYRange");
        sp_EyesNoKeyframes = serializedObject.FindProperty("EyesNoKeyframes");
    }


    protected override void UnderMainTab()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.BeginHorizontal(FEditor_Styles.GrayBackground);
        drawEyesSettings = EditorGUILayout.Foldout(drawEyesSettings, new GUIContent("Eyes Settings", "Settings for eyes transforms"), true);
        GUILayout.EndHorizontal();

        if (drawEyesSettings)
        {
            Color preCol = GUI.color;
            FLookAnimatorWEyes lookT = (FLookAnimatorWEyes)target;

            EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            GUILayout.Space(3);
            EditorGUILayout.PropertyField(sp_eyeL);
            EditorGUILayout.PropertyField(sp_eyeLInv);
            EditorGUILayout.PropertyField(sp_eyeR);
            EditorGUILayout.PropertyField(sp_eyeRInv);

            if (lookT.HeadReference == null) GUI.color = new Color(1f, 0.9f, 0.9f, 0.95f);
            EditorGUILayout.PropertyField(sp_head);
            GUI.color = preCol;
            EditorGUILayout.PropertyField(sp_EyesOffsetRotation);

            if (lookT.EyesTarget == null)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("EyesTarget empty so ObjectToFollow used");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.PropertyField(sp_eyesTarget);

            EditorGUILayout.PropertyField(sp_eyesSpeed);
            EditorGUILayout.PropertyField(sp_eyesBlend);

            GUILayout.Space(5);

            #region Clamping angles

            bool wrongLimit = false;
            if (Mathf.Abs(lookT.EyesXRange.x) > lookT.MaxRotationDiffrence) wrongLimit = true;
            if (Mathf.Abs(lookT.EyesXRange.y) > lookT.MaxRotationDiffrence) wrongLimit = true;


            if (!wrongLimit)
                GUI.color = new Color(0.55f, 0.9f, 0.75f, 0.8f);
            else
                GUI.color = new Color(0.9f, 0.55f, 0.55f, 0.8f);

            // X
            GUILayout.BeginVertical(FEditor_Styles.Emerald);

            GUILayout.BeginHorizontal();
            GUILayout.Label("  Clamp Angle Horizontal (X)", GUILayout.MaxWidth(170f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(Mathf.Round(lookT.EyesXRange.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
            FEditor_CustomInspectorHelpers.DrawMinMaxSphere(lookT.EyesXRange.x, lookT.EyesXRange.y, 14);
            GUILayout.Label(Mathf.Round(lookT.EyesXRange.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref lookT.EyesXRange.x, ref lookT.EyesXRange.y, -90f, 90f);

            eyesbothX = EditorGUILayout.Slider("Adjust symmetrical", eyesbothX, 1f, 90f);

            if (eyeslastBothX != eyesbothX)
            {
                lookT.EyesXRange.x = -eyesbothX;
                lookT.EyesXRange.y = eyesbothX;
                eyeslastBothX = eyesbothX;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();

            GUILayout.Space(7f);

            GUI.color = new Color(0.6f, 0.75f, 0.9f, 0.8f);

            // Y
            GUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            GUILayout.BeginHorizontal();
            GUILayout.Label("  Clamp Angle Vertical (Y)", GUILayout.MaxWidth(170f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(Mathf.Round(lookT.EyesYRange.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
            FEditor_CustomInspectorHelpers.DrawMinMaxVertSphere(-lookT.EyesYRange.y, -lookT.EyesYRange.x, 14);
            GUILayout.Label(Mathf.Round(lookT.EyesYRange.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref lookT.EyesYRange.x, ref lookT.EyesYRange.y, -90f, 90f);
            eyesbothY = EditorGUILayout.Slider("Adjust symmetrical", eyesbothY, 1f, 90f);

            if (eyeslastBothY != eyesbothY)
            {
                lookT.EyesYRange.x = -eyesbothY;
                lookT.EyesYRange.y = eyesbothY;
                eyeslastBothY = eyesbothY;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();

            #endregion

            GUI.color = preCol;

            GUILayout.Space(4f);
            EditorGUIUtility.labelWidth = 144;
            EditorGUILayout.PropertyField(sp_EyesNoKeyframes);
            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndVertical();
        }

        GUILayout.EndVertical();
    }

}