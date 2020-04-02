using FIMSpace.FEditor;
using FIMSpace.FLook;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(FLookAnimator))]
/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
public class FLookAnimator_Editor : UnityEditor.Editor
{
    static bool drawDefaultInspector = false;

    static bool drawMain = true;
    static bool drawCorrecting = false;
    static bool drawAdditional = true;
    static bool drawOptional = false;

    static bool drawBackBones = true;
    static bool drawLimiting = false;

    Transform previousHead = null;

    int compensationBonesCount = 0;

    private float bothX = 70f;
    private float lastBothX = 70f;
    private float bothY = 60f;
    private float lastBothY = 60f;

    // We checking difference in keys for curve because we can't detect change in curve by preCurve != falloffCurve
    Keyframe[] preKeyframes = new Keyframe[0];

    protected SerializedProperty sp_tofollow;
    protected SerializedProperty sp_leadbone;
    protected SerializedProperty sp_basetr;
    protected SerializedProperty sp_rotspd;
    protected SerializedProperty sp_usmooth;
    protected SerializedProperty sp_backbcount;
    protected SerializedProperty sp_falloff;
    protected SerializedProperty sp_clamph;
    protected SerializedProperty sp_clampv;
    protected SerializedProperty sp_maxdiff;
    protected SerializedProperty sp_deltaacc;
    protected SerializedProperty sp_chretspeed;
    protected SerializedProperty sp_animwithsource;
    protected SerializedProperty sp_angoff;
    protected SerializedProperty sp_axesmul;

    protected SerializedProperty sp_blend;
    protected SerializedProperty sp_eyespos;
    protected SerializedProperty sp_ancheyes;
    protected SerializedProperty sp_anchrefr;
    protected SerializedProperty sp_leadblend;
    protected SerializedProperty sp_rotaoff;
    protected SerializedProperty sp_monitor;

    protected SerializedProperty sp_compensblend;
    protected SerializedProperty sp_poscompens;
    protected SerializedProperty sp_weighmul;
    protected SerializedProperty sp_bonesnotanim;

    protected SerializedProperty sp_auto;
    protected SerializedProperty sp_manfromax;
    protected SerializedProperty sp_mantoax;
    protected SerializedProperty sp_lookdirection;

    protected SerializedProperty sp_autooff;
    protected SerializedProperty sp_autofrom;
    protected SerializedProperty sp_fixpres;

    protected SerializedProperty sp_elasticX;
    protected SerializedProperty sp_elasticY;
    protected SerializedProperty sp_starttpose;
    protected SerializedProperty sp_dray;

    protected SerializedProperty sp_usecurve;
    protected SerializedProperty sp_fallvall;
    protected SerializedProperty sp_backoff;
    protected SerializedProperty sp_180prev;
    protected SerializedProperty sp_bigsmooth;
    protected SerializedProperty sp_maxdist;

    protected SerializedProperty sp_bird;
    protected SerializedProperty sp_birdlag;
    protected SerializedProperty sp_birddel;
    protected SerializedProperty sp_birmaxdist;
    protected SerializedProperty sp_birdfreq;
    protected SerializedProperty sp_birdspd;
    protected SerializedProperty sp_birdlagprog;
    protected SerializedProperty sp_birddelgospeed;
    protected SerializedProperty sp_animphys;
    protected SerializedProperty sp_MinHeadLookAngle;

    protected bool animatorDetected = false;

    protected virtual void OnEnable()
    {
        sp_tofollow = serializedObject.FindProperty("ObjectToFollow");
        sp_leadbone = serializedObject.FindProperty("LeadBone");
        sp_basetr = serializedObject.FindProperty("BaseTransform");
        sp_rotspd = serializedObject.FindProperty("RotationSpeed");
        sp_usmooth = serializedObject.FindProperty("UltraSmoother");
        sp_backbcount = serializedObject.FindProperty("BackBonesCount");
        sp_falloff = serializedObject.FindProperty("BackBonesFalloff");
        sp_clamph = serializedObject.FindProperty("XRotationLimits");
        sp_clampv = serializedObject.FindProperty("YRotationLimits");
        sp_maxdiff = serializedObject.FindProperty("MaxRotationDiffrence");
        sp_deltaacc = serializedObject.FindProperty("DeltaAccelerationRange");
        sp_chretspeed = serializedObject.FindProperty("ChangeTargetSpeed");
        sp_animwithsource = serializedObject.FindProperty("AnimateWithSource");
        sp_angoff = serializedObject.FindProperty("RotationOffset");
        sp_axesmul = serializedObject.FindProperty("RotCorrectionMultiplier");

        sp_blend = serializedObject.FindProperty("BlendToOriginal");
        sp_eyespos = serializedObject.FindProperty("LookReferenceOffset");
        sp_ancheyes = serializedObject.FindProperty("AnchorReferencePoint");
        sp_anchrefr = serializedObject.FindProperty("RefreshAnchor");
        sp_leadblend = serializedObject.FindProperty("LeadBoneWeight");
        sp_rotaoff = serializedObject.FindProperty("AnimateWithSource");
        sp_monitor = serializedObject.FindProperty("MonitorAnimator");

        sp_compensblend = serializedObject.FindProperty("CompensationWeight");
        sp_poscompens = serializedObject.FindProperty("CompensatePositions");
        sp_weighmul = serializedObject.FindProperty("WeightsMultiplier");
        sp_bonesnotanim = serializedObject.FindProperty("bonesNotAnimated");

        sp_auto = serializedObject.FindProperty("AutomaticFix");
        sp_manfromax = serializedObject.FindProperty("ManualFromAxis");
        sp_mantoax = serializedObject.FindProperty("ManualToAxis");
        sp_lookdirection = serializedObject.FindProperty("LookDirection");

        sp_autooff = serializedObject.FindProperty("OffsetAuto");
        sp_autofrom = serializedObject.FindProperty("FromAuto");
        sp_fixpres = serializedObject.FindProperty("FixingPreset");

        sp_elasticX = serializedObject.FindProperty("XElasticRange");
        sp_elasticY = serializedObject.FindProperty("YElasticRange");
        sp_starttpose = serializedObject.FindProperty("StartAfterTPose");
        sp_dray = serializedObject.FindProperty("DebugRays");

        sp_usecurve = serializedObject.FindProperty("CurveSpread");
        sp_fallvall = serializedObject.FindProperty("FaloffValue");
        sp_backoff = serializedObject.FindProperty("BackBonesAddOffset");
        sp_180prev = serializedObject.FindProperty("Prevent180Error");
        sp_bigsmooth = serializedObject.FindProperty("BigAngleSmoother");
        sp_maxdist = serializedObject.FindProperty("MaximumDistance");

        sp_bird = serializedObject.FindProperty("BirdMode");
        sp_birdlag = serializedObject.FindProperty("LagMovement");
        sp_birdfreq = serializedObject.FindProperty("LagFrequency");
        sp_birdspd = serializedObject.FindProperty("LaggySpeed");
        sp_birddel = serializedObject.FindProperty("DelayPosition");
        sp_birmaxdist = serializedObject.FindProperty("DelayMaxDistance");
        sp_birdlagprog = serializedObject.FindProperty("LagProgressSpeed");
        sp_birddelgospeed = serializedObject.FindProperty("DelayGoSpeed");
        sp_animphys = serializedObject.FindProperty("AnimatePhysics");
        sp_MinHeadLookAngle = serializedObject.FindProperty("MinHeadLookAngle");

        CheckForAnimator();

        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Look Animator Inspector");

        // Update component from last changes
        serializedObject.Update();

        FLookAnimator lookT = (FLookAnimator)target;

        #region Default Inspector
        if (drawDefaultInspector)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Draw default inspector");

            #region Exluding from view not needed properties

            List<string> excludedVars = new List<string>();

            if (lookT.BackBonesCount < 1)
            {
                excludedVars.Add("BackBonesTransforms");
                excludedVars.Add("BackBonesFalloff");
            }

            if (!lookT.AnimateWithSource) excludedVars.Add("MonitorAnimator");

            if (lookT.CompensationBones == null || lookT.CompensationBones.Count < 1) excludedVars.Add("CompensationWeight");

            #endregion

            GUILayout.Space(5f);

            if (GUILayout.Button("Auto Find Head Bone")) FindHeadBone(lookT);
            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);

            if (lookT.BackBonesCount < 0) lookT.BackBonesCount = 0;

            // Draw default inspector without not needed properties
            DrawPropertiesExcluding(serializedObject, excludedVars.ToArray());
        }
        else
        #endregion
        {
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUILayout.BeginHorizontal();
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Default inspector");

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 80;
            lookT.drawGizmos = GUILayout.Toggle(lookT.drawGizmos, "Draw Gizmos");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);

            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUI.indentLevel++;

            drawMain = EditorGUILayout.Foldout(drawMain, "Main Animation Parameters", true);

            #region Main tab

            if (drawMain)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (!lookT.ObjectToFollow)
                    GUILayout.BeginHorizontal(FEditor_Styles.YellowBackground);
                else
                {
                    GUILayout.Space(5f);
                    GUILayout.BeginHorizontal();
                }

                EditorGUIUtility.labelWidth = 133f;

                EditorGUILayout.PropertyField(sp_tofollow);

                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();

                if (!lookT.LeadBone)
                    GUILayout.BeginHorizontal(FEditor_Styles.RedBackground);
                else
                    GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(sp_leadbone);


                if (lookT.LeadBone != previousHead)
                {
                    previousHead = lookT.LeadBone;

                    lookT.UpdateForCustomInspector();
                }

                if (GUILayout.Button(new GUIContent("Auto Find", "By pressing this button, algorithm will go trough hierarchy and try to find object which name includes 'head' or 'neck', be aware, this bone can not be correct but sure it will help you find right one quicker"), new GUILayoutOption[2] { GUILayout.MaxWidth(90), GUILayout.MaxHeight(15) }))
                {
                    FindHeadBone(lookT);
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (lookT.BaseTransform != lookT.transform || !lookT.BaseTransform)
                {
                    if (!lookT.BaseTransform)
                        GUILayout.BeginHorizontal(FEditor_Styles.YellowBackground);
                    else
                        GUILayout.BeginHorizontal();

                    EditorGUILayout.PropertyField(sp_basetr);

                    if (GUILayout.Button("Try Find", new GUILayoutOption[2] { GUILayout.MaxWidth(90), GUILayout.MaxHeight(15) }))
                    {
                        lookT.FindBaseTransform();
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndHorizontal();
                Color preColor = GUI.color;

                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(sp_rotspd);
                GUILayout.Space(3f);
                if (lookT.BirdMode) GUI.color = new Color(1f, 0.88f, 0.88f, 0.95f);
                EditorGUILayout.PropertyField(sp_usmooth);
                GUI.color = preColor;
                EditorGUILayout.PropertyField(sp_starttpose);


                if (!lookT.Prevent180Error) if (lookT.UltraSmoother > 0f) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);

                GUILayout.Space(2f);
                EditorGUILayout.PropertyField(sp_180prev);
                GUI.color = preColor;

                EditorGUIUtility.labelWidth = 0f;

                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
                EditorGUILayout.HelpBox("Using more bones (back bones) to help rotate head (head-neck-spine)", MessageType.None);
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                int preCount = lookT.BackBonesCount;

                EditorGUIUtility.labelWidth = 130f;
                EditorGUILayout.LabelField(new GUIContent("Backbones: (" + lookT.BackBonesCount + ")"));
                EditorGUIUtility.labelWidth = 0f;

                if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                {
                    lookT.BackBonesCount++;
                    serializedObject.ApplyModifiedProperties();
                    if (!UpdateCustomInspector(lookT, false)) Debug.Log("[LOOK ANIMATOR] Don't change backbones count in playmode");
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("-", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                {
                    lookT.BackBonesCount--;
                    serializedObject.ApplyModifiedProperties();
                    if (!UpdateCustomInspector(lookT, false)) Debug.Log("[LOOK ANIMATOR] Don't change backbones count in playmode");
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();

                if (lookT.BackBonesCount < 0) lookT.BackBonesCount = 0;

                if (preCount != lookT.BackBonesCount) UpdateCustomInspector(lookT);

                if (lookT.BackBonesCount > 0)
                {
                    drawBackBones = EditorGUILayout.Foldout(drawBackBones, "View back bones", true);
                    if (drawBackBones)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = 150f;
                        for (int i = 0; i < lookT.BackBonesTransforms.Length; i++)
                        {
                            string weightString = " " + Mathf.Round(lookT.RotationWeights[i] * 100f * lookT.WeightsMultiplier) + "%";

                            Transform preTr = lookT.BackBonesTransforms[i];
                            Transform newTr = (Transform)EditorGUILayout.ObjectField("Back bone [" + i + "]" + weightString, lookT.BackBonesTransforms[i], typeof(Transform), true);

                            // If we assigned own bone
                            if (preTr != newTr)
                            {
                                lookT.BackBonesTransforms[i] = newTr;
                                lookT.AddCustomBackbone(lookT.BackBonesTransforms[i], i);
                                EditorUtility.SetDirty(target);
                            }
                        }
                        EditorGUIUtility.labelWidth = 0f;

                        EditorGUI.indentLevel--;
                    }

                    GUILayout.Space(5f);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUIUtility.labelWidth = 105f;
                    EditorGUILayout.PropertyField(sp_usecurve);
                    EditorGUIUtility.labelWidth = 0f;

                    if (lookT.CurveSpread)
                    {
                        EditorGUILayout.PropertyField(sp_falloff, new GUIContent(""), new GUILayoutOption[2] { GUILayout.MaxHeight(40f), GUILayout.MinHeight(30f) });

                        bool curveChanged = false;
                        if (preKeyframes.Length != lookT.BackBonesFalloff.keys.Length) curveChanged = true;

                        if (!curveChanged)
                        {
                            for (int i = 0; i < preKeyframes.Length; i++)
                            {
                                Keyframe pre = preKeyframes[i];
                                Keyframe curr = lookT.BackBonesFalloff.keys[i];

                                if (pre.value != curr.value || pre.time != curr.time || pre.inTangent != curr.inTangent || pre.outTangent != curr.outTangent)
                                {
                                    curveChanged = true;
                                    break;
                                }
                            }
                        }

                        if (curveChanged)
                            if (preKeyframes != lookT.BackBonesFalloff.keys)
                            {
                                preKeyframes = lookT.BackBonesFalloff.keys;
                                UpdateCustomInspector(lookT);
                                EditorUtility.SetDirty(target);
                            }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(sp_fallvall, new GUIContent(""));
                    }

                    GUILayout.Space(8f);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(8f);
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                UnderMainTab();

                #region Limiting angles

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal(FEditor_Styles.GrayBackground);
                drawLimiting = EditorGUILayout.Foldout(drawLimiting, "Limiting & Others Tab", true);
                GUILayout.EndHorizontal();

                if (drawLimiting)
                {
                    #region Clamping angles

                    bool wrongLimit = false;
                    if (Mathf.Abs(lookT.XRotationLimits.x) > lookT.MaxRotationDiffrence) wrongLimit = true;
                    if (Mathf.Abs(lookT.XRotationLimits.y) > lookT.MaxRotationDiffrence) wrongLimit = true;

                    Color preCol = GUI.color;

                    GUILayout.Space(4f);
                    GUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

                    if (lookT.MaximumDistance > 0f)
                        EditorGUILayout.PropertyField(sp_maxdist);
                    else
                    {
                        GUILayout.BeginHorizontal();
                        //GUILayout.FlexibleSpace();

                        //EditorGUIUtility.labelWidth = 80;
                        EditorGUILayout.PropertyField(sp_maxdist);
                        //EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.LabelField("(Infinity)", new GUILayoutOption[] { GUILayout.Width(70f) });

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Space(4f);
                    EditorGUILayout.PropertyField(sp_MinHeadLookAngle);

                    if (lookT.MaximumDistance < 0f)
                    {
                        lookT.MaximumDistance = 0f;
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(4f);

                    if (!wrongLimit)
                        GUI.color = new Color(0.55f, 0.9f, 0.75f, 0.8f);
                    else
                        GUI.color = new Color(0.9f, 0.55f, 0.55f, 0.8f);

                    // X
                    GUILayout.BeginVertical(FEditor_Styles.Emerald);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("  Clamp Angle Horizontal (X)", GUILayout.MaxWidth(170f));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Mathf.Round(lookT.XRotationLimits.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    //FEditor_CustomInspectorHelpers.DrawMinMaxSphere(lookT.XRotationLimits.x, lookT.XRotationLimits.y, 14, lookT.XElasticRange);
                    FEditor_CustomInspectorHelpers.DrawMinMaxSphere(lookT.XRotationLimits.x, lookT.XRotationLimits.y, 14, lookT.XElasticRange);
                    GUILayout.Label(Mathf.Round(lookT.XRotationLimits.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.MinMaxSlider(ref lookT.XRotationLimits.x, ref lookT.XRotationLimits.y, -180f, 180f);

                    //if (Mathf.Abs(lookT.XRotationLimits.x) > lookT.MaxRotationDiffrence) lookT.MaxRotationDiffrence = Mathf.Abs(lookT.XRotationLimits.x);
                    //if (Mathf.Abs(lookT.XRotationLimits.y) > lookT.MaxRotationDiffrence) lookT.MaxRotationDiffrence = Mathf.Abs(lookT.XRotationLimits.y);

                    bothX = EditorGUILayout.Slider("Adjust symmetrical", bothX, 1f, 180f);
                    EditorGUILayout.PropertyField(sp_elasticX);

                    if (lastBothX != bothX)
                    {
                        lookT.XRotationLimits.x = -bothX;
                        lookT.XRotationLimits.y = bothX;
                        lastBothX = bothX;
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
                    GUILayout.Label(Mathf.Round(lookT.YRotationLimits.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    FEditor_CustomInspectorHelpers.DrawMinMaxVertSphere(-lookT.YRotationLimits.y, -lookT.YRotationLimits.x, 14, lookT.YElasticRange);
                    GUILayout.Label(Mathf.Round(lookT.YRotationLimits.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.MinMaxSlider(ref lookT.YRotationLimits.x, ref lookT.YRotationLimits.y, -90f, 90f);
                    bothY = EditorGUILayout.Slider("Adjust symmetrical", bothY, 1f, 90f);
                    EditorGUILayout.PropertyField(sp_elasticY);

                    if (lastBothY != bothY)
                    {
                        lookT.YRotationLimits.x = -bothY;
                        lookT.YRotationLimits.y = bothY;
                        lastBothY = bothY;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndVertical();

                    #endregion

                    GUI.color = preCol;

                    GUILayout.Space(15f);

                    GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
                    EditorGUILayout.HelpBox("If this angle is exceeded character will look forward", MessageType.None);
                    GUILayout.EndHorizontal();

                    if (!wrongLimit)
                        GUI.color = new Color(0.55f, 0.9f, 0.75f, 0.8f);
                    else
                        GUI.color = new Color(0.9f, 0.55f, 0.55f, 0.8f);

                    EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                    GUILayout.BeginHorizontal();

                    lookT.MaxRotationDiffrence = EditorGUILayout.Slider("Max Angle Diff", lookT.MaxRotationDiffrence, 25f, 180f);
                    FEditor_CustomInspectorHelpers.DrawMinMaxSphere(-lookT.MaxRotationDiffrence, lookT.MaxRotationDiffrence, 14);
                    GUILayout.EndHorizontal();

                    lookT.DeltaAccelerationRange = EditorGUILayout.Slider(new GUIContent("Delta Accelerate", "If head have to rotate more than this value it's animation speed for rotating increases, small touch to make animation more realistic"), lookT.DeltaAccelerationRange, 25f, 70f);
                    EditorGUILayout.PropertyField(sp_chretspeed);
                    EditorGUILayout.PropertyField(sp_bigsmooth);

                    GUILayout.Space(8f);

                    EditorGUILayout.EndVertical();
                    GUI.color = preCol;
                    serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndVertical();
            }

            #endregion

            #endregion


            #region Correcting bones orientations


            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUI.indentLevel++;

            drawCorrecting = EditorGUILayout.Foldout(drawCorrecting, "Correcting rotations", true);

            if (drawCorrecting)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(3f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(sp_fixpres);

                EditorGUILayout.BeginVertical(new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(20) });

                if (GUILayout.Button("▲", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(10) }))
                {
                    if ((int)lookT.FixingPreset == 0)
                    {
                        lookT.FixingPreset = (FLookAnimator.EFAxisFixOrder)(Enum.GetValues(typeof(FLookAnimator.EFAxisFixOrder)).Length - 1);
                    }
                    else
                        lookT.FixingPreset--;

                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("▼", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(10) }))
                {
                    if ((int)lookT.FixingPreset + 1 >= Enum.GetValues(typeof(FLookAnimator.EFAxisFixOrder)).Length)
                    {
                        lookT.FixingPreset = 0;
                    }
                    else
                        lookT.FixingPreset++;

                    EditorUtility.SetDirty(target);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(3f);

                if (lookT.FixingPreset == FLookAnimator.EFAxisFixOrder.FullManual)
                {
                    EditorGUILayout.PropertyField(sp_axesmul);
                    GUILayout.Space(5f);
                    EditorGUILayout.PropertyField(sp_manfromax);
                    EditorGUILayout.PropertyField(sp_mantoax);
                    GUILayout.Space(5f);
                }

                if (lookT.FixingPreset == FLookAnimator.EFAxisFixOrder.FromBased)
                {
                    EditorGUILayout.LabelField("Auto Offset: " + RoundVector(lookT.OffsetAuto));
                    EditorGUILayout.LabelField("Auto From Axis: " + RoundVector(lookT.FromAuto));
                }

                EditorGUILayout.PropertyField(sp_angoff);
                EditorGUILayout.PropertyField(sp_backoff);
                GUILayout.Space(5f);

                EditorGUILayout.PropertyField(sp_dray);
                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
            }

            EditorGUILayout.EndVertical();


            #endregion


            #region Additional parameters


            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUI.indentLevel++;

            drawAdditional = EditorGUILayout.Foldout(drawAdditional, "Additional controll parameters", true);

            if (drawAdditional)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(5f);
                EditorGUIUtility.labelWidth = 140;

                //lookT.BlendToOriginal = EditorGUILayout.Slider(new GUIContent("Animation Blend", "Main blend value for new head look rotations"), lookT.BlendToOriginal, 0f, 1f);
                EditorGUILayout.PropertyField(sp_blend);

                EditorGUIUtility.labelWidth = 180;
                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(sp_eyespos);

                EditorGUILayout.PropertyField(sp_ancheyes);

                if (lookT.AnchorReferencePoint)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_anchrefr);
                    EditorGUI.indentLevel--;
                }

                GUILayout.Space(3f);
                EditorGUIUtility.labelWidth = 140;
                EditorGUILayout.PropertyField(sp_leadblend);

                GUILayout.Space(3f);

                bool wrong = false;
                if (!animatorDetected) if (lookT.AnimateWithSource) wrong = true;

                EditorGUILayout.BeginVertical(FEditor_Styles.GreenBackground);
                EditorGUIUtility.labelWidth = 160;

                Color preCol = GUI.color;
                if (wrong)
                {
                    EditorGUILayout.HelpBox("Component can't find animator attached to your character, you should untoggle this variable if there isn't any animator working on character's animation", MessageType.Warning);
                    GUI.color = new Color(1f, 1f, 0.35f, 0.8f);
                }

                EditorGUILayout.PropertyField(sp_animwithsource);

                if (lookT.AnimateWithSource)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_monitor);
                    EditorGUI.indentLevel--;
                }

                GUI.color = preCol;


                EditorGUIUtility.labelWidth = 0;

                EditorGUILayout.EndVertical();

                GUILayout.Space(5f);


                // v1.0.7 - Bird Mode
                EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);
                EditorGUIUtility.labelWidth = 160;

                EditorGUILayout.PropertyField(sp_bird);

                if (lookT.BirdMode)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_birdlag);
                    EditorGUILayout.PropertyField(sp_birdspd);
                    EditorGUILayout.PropertyField(sp_birdlagprog);
                    EditorGUILayout.PropertyField(sp_birdfreq);
                    EditorGUILayout.PropertyField(sp_birddel);
                    EditorGUILayout.PropertyField(sp_birmaxdist);
                    EditorGUILayout.PropertyField(sp_birddelgospeed);
                    EditorGUI.indentLevel--;
                }

                EditorGUIUtility.labelWidth = 0;

                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);


                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
                EditorGUILayout.HelpBox("If you don't want arms to be rotated when spine, bone is rotated by script", MessageType.None);
                GUILayout.EndHorizontal();

                #region Supporting list in custom editor

                if (lookT.CompensationBones != null)
                    compensationBonesCount = lookT.CompensationBones.Count;
                else
                    compensationBonesCount = 0;

                GUILayout.BeginHorizontal();
                compensationBonesCount = EditorGUILayout.IntField("Compensation Bones", compensationBonesCount);
                if (GUILayout.Button(new GUIContent("Find", "By pressing this button, algorithm will try to find bones with names 'Shoulder', 'Upper Arm' be aware, because you can have other objects inside including this names"), new GUILayoutOption[2] { GUILayout.MaxWidth(58), GUILayout.MaxHeight(14) })) FindCompensationBones(lookT);
                if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                {
                    serializedObject.ApplyModifiedProperties();
                    compensationBonesCount++;
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("-", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                {
                    if (lookT.CompensationBones.Count == 1)
                    {
                        lookT.CompensationBones = new List<Transform>();
                        compensationBonesCount = 0;
                        //lookT.BackBonesCount = 0;
                    }
                    else
                        compensationBonesCount--;

                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();

                if (compensationBonesCount <= 0) compensationBonesCount = 0;
                else
                {
                    if (compensationBonesCount > lookT.CompensationBones.Count)
                    {
                        for (int i = lookT.CompensationBones.Count; i < compensationBonesCount; i++) lookT.CompensationBones.Add(null);
                    }
                    else if (compensationBonesCount < lookT.CompensationBones.Count)
                    {
                        for (int i = lookT.CompensationBones.Count - 1; i >= compensationBonesCount; i--) lookT.CompensationBones.RemoveAt(i);
                    }

                    if (lookT.CompensationBones.Count > 0)
                    {
                        EditorGUI.indentLevel++;

                        for (int i = 0; i < lookT.CompensationBones.Count; i++)
                        {
                            lookT.CompensationBones[i] = (Transform)EditorGUILayout.ObjectField("Bone [" + i + "]", lookT.CompensationBones[i], typeof(Transform), true);
                        }

                        EditorGUI.indentLevel--;

                        GUILayout.Space(5f);

                        EditorGUILayout.PropertyField(sp_compensblend);
                        EditorGUILayout.PropertyField(sp_poscompens);
                    }
                }

                #endregion

                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
            }

            EditorGUILayout.EndVertical();


            #endregion


            #region Optional parameters


            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUI.indentLevel++;

            drawOptional = EditorGUILayout.Foldout(drawOptional, "Optional parameters", true);

            if (drawOptional)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(10f);

                EditorGUILayout.PropertyField(sp_weighmul);

                GUILayout.Space(3f);

                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("If animation is not animating some head bones (very rare case)", MessageType.None);
                GUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(sp_bonesnotanim);
                EditorGUILayout.PropertyField(sp_animphys);

                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndVertical();
        }

        // Apply changed parameters variables
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Updating component from custom inspector helper method
    /// </summary>
    private bool UpdateCustomInspector(FLookAnimator lookT, bool allowInPlaymode = true)
    {
        if (!allowInPlaymode)
        {
            if (Application.isPlaying) return false;
        }
        else
            lookT.UpdateForCustomInspector();

        return true;
    }

    /// <summary>
    /// Searching through component's owner to find head or neck bone
    /// </summary>
    private void FindHeadBone(FLookAnimator headLook)
    {
        // First let's check if it's humanoid character, then we can get head bone transform from it
        Transform root = headLook.transform;
        if (headLook.BaseTransform) root = headLook.BaseTransform;

        Animator animator = root.GetComponentInChildren<Animator>();
        Transform animatorHeadBone = null;
        if (animator)
        {
            if (animator.isHuman)
            {
                animatorHeadBone = animator.GetBoneTransform(HumanBodyBones.Head);
            }
        }


        Transform leadBone = null;
        Transform probablyWrongTransform = null;

        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name.ToLower().Contains("head"))
            {
                if (t.GetComponent<SkinnedMeshRenderer>())
                {
                    if (t.parent == root) continue; // If it's just mesh object from first depths
                    probablyWrongTransform = t;
                    continue;
                }

                leadBone = t;
                break;
            }
        }

        if (!leadBone)
            foreach (Transform t in root.GetComponentsInChildren<Transform>())
            {
                if (t.name.ToLower().Contains("neck"))
                {
                    leadBone = t;
                    break;
                }
            }

        if (leadBone == null && animatorHeadBone != null)
            leadBone = animatorHeadBone;
        else
        if (leadBone != null && animatorHeadBone != null)
        {
            if (animatorHeadBone.name.ToLower().Contains("head")) leadBone = animatorHeadBone;
            else
                if (!leadBone.name.ToLower().Contains("head")) leadBone = animatorHeadBone;
        }

        if (leadBone)
        {
            headLook.LeadBone = leadBone;
        }
        else
        {
            if (probablyWrongTransform) headLook.LeadBone = probablyWrongTransform;
            Debug.LogWarning("[LOOK ANIMATOR] Found " + probablyWrongTransform + " but it's probably wrong transform");
        }
    }

    protected virtual void UnderMainTab()
    {

    }

    /// <summary>
    /// Searching through component's owner to find clavicle / shoulder and upperarm bones
    /// </summary>
    private void FindCompensationBones(FLookAnimator headLook)
    {
        // First let's check if it's humanoid character, then we can get head bone transform from it
        Transform root = headLook.transform;
        if (headLook.BaseTransform) root = headLook.BaseTransform;

        Animator animator = root.GetComponentInChildren<Animator>();

        List<Transform> compensationBones = new List<Transform>();

        Transform headBone = headLook.LeadBone;

        if (animator)
        {
            if (animator.isHuman)
            {
                Transform b = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                if (b) compensationBones.Add(b);

                b = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                if (b) compensationBones.Add(b);

                b = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                if (b) compensationBones.Add(b);

                b = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                if (b) compensationBones.Add(b);

                if (!headBone) animator.GetBoneTransform(HumanBodyBones.Head);
            }
            else
            {
                if (animator)
                {
                    foreach (Transform t in animator.transform.GetComponentsInChildren<Transform>())
                    {
                        if (t.name.ToLower().Contains("clav"))
                        {
                            if (!compensationBones.Contains(t)) compensationBones.Add(t);
                        }
                        else
                        if (t.name.ToLower().Contains("shoulder"))
                        {
                            if (!compensationBones.Contains(t)) compensationBones.Add(t);
                        }
                        else
                        if (t.name.ToLower().Contains("uppera"))
                        {
                            if (!compensationBones.Contains(t)) compensationBones.Add(t);
                        }
                    }
                }
            }
        }

        if (compensationBones.Count != 0)
        {
            headLook.CompensationBones = compensationBones;
            compensationBonesCount = compensationBones.Count;
        }
    }

    protected void CheckForAnimator()
    {
        FLookAnimator look = (FLookAnimator)target;

        Transform root = look.transform;
        if (look.BaseTransform) root = look.BaseTransform;

        Animation animation = null;
        Animator animator = root.GetComponentInChildren<Animator>();
        if (!animator) if (root.transform.parent) if (root.transform.parent.parent) animator = root.transform.parent.parent.GetComponentInChildren<Animator>(); else animator = root.transform.parent.GetComponent<Animator>();

        if (!animator)
        {
            animation = root.GetComponentInChildren<Animation>();
            if (!animation) if (root.transform.parent) if (root.transform.parent.parent) animation = root.transform.parent.parent.GetComponentInChildren<Animation>(); else animation = root.transform.parent.GetComponent<Animation>();
        }

        if (animator != null || animation != null) animatorDetected = true; else animatorDetected = false;
    }

    public Vector3 RoundVector(Vector3 v)
    {
        return new Vector3((float)Math.Round(v.x, 1), (float)Math.Round(v.y, 1), (float)Math.Round(v.z, 1));
    }
}