using FIMSpace.FEditor;
using FIMSpace.FEyes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(FEyesAnimator))]
public class FEyesAnimator_Editor : UnityEditor.Editor
{
    protected static bool drawDefaultInspector = false;

    protected bool drawEyesTarget = true;
    protected bool drawRanges = true;
    protected bool drawClamping = true;

    static bool drawSetup = true;
    static bool showEyes = true;
    static bool showAnimationSettings = true;
    static bool showClampingAndOthers = false;

    float bothX = 70f;
    float lastBothX = 70f;
    float bothY = 60f;
    float lastBothY = 60f;

    protected SerializedProperty sp_HeadReference;
    protected SerializedProperty sp_StartLookOffset;
    protected SerializedProperty sp_EyesTarget;
    protected SerializedProperty sp_Eyes;
    protected SerializedProperty sp_EyesSpeed;
    protected SerializedProperty sp_EyesBlend;
    protected SerializedProperty sp_EyesRandomMovement;
    protected SerializedProperty sp_RandomMovementAxisScale;
    protected SerializedProperty sp_RandomMovementPreset;
    protected SerializedProperty sp_RandomizingSpeed;
    protected SerializedProperty sp_EyesRandomMovementIndividual;
    protected SerializedProperty sp_EyesLagAmount;
    protected SerializedProperty sp_IndividualLags;
    protected SerializedProperty sp_EyesMaxRange;
    protected SerializedProperty sp_BlendTransitionSpeed;
    protected SerializedProperty sp_EyesClampHorizontal;
    protected SerializedProperty sp_EyesClampVertical;
    protected SerializedProperty sp_SquintPreventer;
    protected SerializedProperty sp_CorrectionOffsets;
    protected SerializedProperty sp_EyesMaxDistance;
    protected SerializedProperty sp_LagStiffness;
    //protected SerializedProperty sp_RootFixer;

    protected virtual void OnEnable()
    {
        sp_HeadReference = serializedObject.FindProperty("HeadReference");
        sp_StartLookOffset = serializedObject.FindProperty("StartLookOffset");
        sp_EyesTarget = serializedObject.FindProperty("EyesTarget");
        sp_Eyes = serializedObject.FindProperty("Eyes");
        sp_EyesSpeed = serializedObject.FindProperty("EyesSpeed");
        sp_EyesBlend = serializedObject.FindProperty("EyesBlend");
        sp_EyesRandomMovement = serializedObject.FindProperty("EyesRandomMovement");
        sp_RandomMovementAxisScale = serializedObject.FindProperty("RandomMovementAxisScale");
        sp_RandomMovementPreset = serializedObject.FindProperty("RandomMovementPreset");
        sp_RandomizingSpeed = serializedObject.FindProperty("RandomizingSpeed");
        sp_EyesRandomMovementIndividual = serializedObject.FindProperty("EyesRandomMovementIndividual");
        sp_EyesLagAmount = serializedObject.FindProperty("EyesLagAmount");
        sp_IndividualLags = serializedObject.FindProperty("IndividualLags");
        sp_EyesMaxRange = serializedObject.FindProperty("EyesMaxRange");
        sp_BlendTransitionSpeed = serializedObject.FindProperty("BlendTransitionSpeed");
        sp_EyesClampHorizontal = serializedObject.FindProperty("EyesClampHorizontal");
        sp_EyesClampVertical = serializedObject.FindProperty("EyesClampVertical");
        sp_SquintPreventer = serializedObject.FindProperty("SquintPreventer");
        sp_EyesClampVertical = serializedObject.FindProperty("EyesClampVertical");
        sp_CorrectionOffsets = serializedObject.FindProperty("CorrectionOffsets");
        sp_EyesMaxDistance = serializedObject.FindProperty("EyesMaxDistance");
        sp_LagStiffness = serializedObject.FindProperty("LagStiffness");
        //sp_RootFixer = serializedObject.FindProperty("RootFixer");
        EditorUtility.SetDirty(target);

        FEyesAnimator eyesAnim = (FEyesAnimator)target;

        if (eyesAnim.Eyes == null) showEyes = false;
        else
        if (eyesAnim.Eyes.Count != 0) showEyes = false;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Eyes Animator Inspector Operations");

        // Update component from last changes
        serializedObject.Update();

        FEyesAnimator eyesAnim = (FEyesAnimator)target;

        #region Default Inspector
        if (drawDefaultInspector)
        {
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, " Draw default inspector");
            EditorGUILayout.EndVertical();

            DrawDefaultInspector();
        }
        else
        #endregion
        {
            Color preCol = GUI.color;

            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, " Draw default inspector");
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground); //
            EditorGUI.indentLevel++;

            DrawPre();

            drawSetup = EditorGUILayout.Foldout(drawSetup, "Setup Component", true);

            #region Component Setup Tab

            if (eyesAnim.Eyes == null) eyesAnim.Eyes = new List<Transform>();

            if (drawSetup)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);

                if (drawEyesTarget)
                {
                    EditorGUILayout.PropertyField(sp_EyesTarget, true);
                    GUILayout.Space(4f);
                }

                GUILayout.BeginHorizontal();

                if (!eyesAnim.HeadReference)
                    GUILayout.BeginHorizontal(FEditor_Styles.RedBackground);
                else
                    GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(sp_HeadReference, true);

                if (GUILayout.Button(new GUIContent("Auto Find", "By pressing this button, algorithm will go trough hierarchy and try to find object which name includes 'head' or 'neck', be aware, this bone can not be correct but sure it will help you find right one quicker"), new GUILayoutOption[2] { GUILayout.MaxWidth(90), GUILayout.MaxHeight(15) }))
                {
                    FindHeadBone(eyesAnim);
                    EditorUtility.SetDirty(target);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(sp_StartLookOffset);
                GUILayout.Space(6f);

                GUILayout.BeginVertical(FEditor_Styles.Emerald);

                GUILayout.BeginHorizontal();
                showEyes = EditorGUILayout.Foldout(showEyes, "Eye Game Objects (" + eyesAnim.Eyes.Count + ")", true);

                if (showEyes)
                {
                    if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = preCol;
                    if (GUILayout.Button(new GUIContent("Lock Inspector", "Locking Inspector Window to help Drag & Drop operations"), new GUILayoutOption[2] { GUILayout.Width(106), GUILayout.Height(16) })) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                    GUI.color = preCol;

                    if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                    {
                        eyesAnim.Eyes.Add(null);
                        eyesAnim.UpdateLists();
                        EditorUtility.SetDirty(target);
                    }
                }

                GUILayout.EndHorizontal();
                GUI.color = preCol;

                if (showEyes)
                {
                    GUI.color = new Color(0.5f, 1f, 0.5f, 0.9f);

                    var drop = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                    GUI.Box(drop, "Drag & Drop your eye GameObjects here", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fixedHeight = 22 });
                    var dropEvent = Event.current;

                    switch (dropEvent.type)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if (!drop.Contains(dropEvent.mousePosition)) break;

                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (dropEvent.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                foreach (var dragged in DragAndDrop.objectReferences)
                                {
                                    GameObject draggedObject = dragged as GameObject;

                                    if (draggedObject)
                                    {
                                        if (!eyesAnim.Eyes.Contains(draggedObject.transform)) eyesAnim.Eyes.Add(draggedObject.transform);
                                        EditorUtility.SetDirty(target);
                                    }
                                }

                                eyesAnim.UpdateLists();
                            }

                            Event.current.Use();
                            break;
                    }

                    GUI.color = preCol;

                    for (int i = 0; i < eyesAnim.Eyes.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        string name;
                        if (!eyesAnim.Eyes[i])
                        {
                            name = "Assign Object";
                            GUI.color = new Color(0.9f, 0.7f, 0.4f, 0.9f);
                        }
                        else
                        {
                            name = eyesAnim.Eyes[i].name;
                            if (name.Length > 16) name = eyesAnim.Eyes[i].name.Substring(0, 12) + "...";
                        }


                        eyesAnim.Eyes[i] = (Transform)EditorGUILayout.ObjectField("[" + i + "] " + name, eyesAnim.Eyes[i], typeof(Transform), true);

                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                        {
                            eyesAnim.Eyes.RemoveAt(i);
                            eyesAnim.UpdateLists();
                            EditorUtility.SetDirty(target);
                        }

                        GUI.color = preCol;

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Space(3);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Animation Settings Tab

            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground); ///
            EditorGUI.indentLevel++;

            showAnimationSettings = EditorGUILayout.Foldout(showAnimationSettings, "Show Animation Settings", true);

            if (showAnimationSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(4f);
                EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.6f, 0.4f, 0.8f, 0.07f)));
                GUI.color = new Color(0.96f, 0.94f, 0.98f, 0.95f);
                EditorGUILayout.PropertyField(sp_EyesBlend);
                EditorGUILayout.PropertyField(sp_EyesSpeed);
                EditorGUILayout.PropertyField(sp_SquintPreventer);
                EditorGUILayout.EndVertical();
                GUILayout.Space(4f);

                EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

                var drop = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                GUI.Box(drop, "Eyes Random Movement Settings", new GUIStyle(FEditor_Styles.LBlueBackground) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fixedHeight = 22 });

                GUI.color = new Color(0.95f, 0.95f, 1f, 0.92f);

                EditorGUILayout.PropertyField(sp_EyesRandomMovement, new GUIContent("Amount", "Random movement for eyes in addition to current direction - you can crank it up for example when there is no target for eyes, or when character is talking with someone"));
                EditorGUILayout.PropertyField(sp_RandomMovementAxisScale, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(sp_RandomMovementPreset, new GUIContent("Preset"));
                EditorGUILayout.PropertyField(sp_RandomizingSpeed, new GUIContent("Frequency"));
                EditorGUILayout.PropertyField(sp_EyesRandomMovementIndividual, new GUIContent("Individual", "Option for monsters, each eye will have individual random rotation direction"));

                EditorGUILayout.EndVertical();

                GUILayout.Space(4f);
                EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                GUI.color = new Color(0.95f, 1f, 0.95f, 0.92f);

                EditorGUILayout.PropertyField(sp_EyesLagAmount, new GUIContent("Lag Amount", "When we rotate eyes, they're reaching target with kinda jumpy movement depending of point of interest, but for more toon/not real effect you can left this value at 0"));
                EditorGUILayout.PropertyField(sp_LagStiffness);

                EditorGUILayout.PropertyField(sp_IndividualLags, new GUIContent("Individual", "Option for monsters, each eye will have individual random delay for movement"));

                EditorGUILayout.EndVertical();

                GUI.color = preCol;
                GUILayout.Space(3f);

                EditorGUILayout.EndVertical();
                GUILayout.Space(3f);
            }

            EditorGUILayout.EndVertical(); //


            #endregion


            #region Clamping and others Tab

            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground); ///
            EditorGUI.indentLevel++;

            showClampingAndOthers = EditorGUILayout.Foldout(showClampingAndOthers, "Show Clamping and Others", true);

            if (showClampingAndOthers)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox); ///
                bool wrongLimit = false;


                #region Clamping angles

                if (Mathf.Abs(eyesAnim.EyesClampHorizontal.x) > eyesAnim.EyesMaxRange) wrongLimit = true;
                if (Mathf.Abs(eyesAnim.EyesClampHorizontal.y) > eyesAnim.EyesMaxRange) wrongLimit = true;

                if (drawRanges)
                {
                    GUILayout.Space(4f);
                    GUILayout.BeginVertical(FEditor_Styles.LBlueBackground); ////

                    if (eyesAnim.EyesMaxDistance > 0f)
                        EditorGUILayout.PropertyField(sp_EyesMaxDistance);
                    else
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(sp_EyesMaxDistance);
                        EditorGUILayout.LabelField("(Infinity)", new GUILayoutOption[] { GUILayout.Width(70f) });
                        GUILayout.EndHorizontal();
                    }

                    if (eyesAnim.EyesMaxDistance < 0f)
                    {
                        eyesAnim.EyesMaxDistance = 0f;
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndVertical(); ///
                }

                GUILayout.Space(4f);

                if (drawClamping)
                {
                    if (!wrongLimit) GUI.color = new Color(0.55f, 0.9f, 0.75f, 0.8f); else GUI.color = new Color(0.9f, 0.55f, 0.55f, 0.8f);

                    GUILayout.BeginVertical(FEditor_Styles.Emerald); ////
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("  Clamp Angle Horizontal (X)", GUILayout.MaxWidth(170f));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Mathf.Round(eyesAnim.EyesClampHorizontal.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    FEditor_CustomInspectorHelpers.DrawMinMaxSphere(eyesAnim.EyesClampHorizontal.x, eyesAnim.EyesClampHorizontal.y, 14, 0f);
                    GUILayout.Label(Mathf.Round(eyesAnim.EyesClampHorizontal.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.MinMaxSlider(ref eyesAnim.EyesClampHorizontal.x, ref eyesAnim.EyesClampHorizontal.y, -180f, 180f);
                    bothX = EditorGUILayout.Slider("Adjust symmetrical", bothX, 1f, 180f);

                    if (lastBothX != bothX)
                    {
                        eyesAnim.EyesClampHorizontal.x = -bothX;
                        eyesAnim.EyesClampHorizontal.y = bothX;
                        lastBothX = bothX;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndVertical(); ///




                    GUILayout.Space(7f);

                    GUI.color = new Color(0.6f, 0.75f, 0.9f, 0.8f);

                    GUILayout.BeginVertical(FEditor_Styles.LBlueBackground); ////
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("  Clamp Angle Vertical (Y)", GUILayout.MaxWidth(170f));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Mathf.Round(eyesAnim.EyesClampVertical.x) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    FEditor_CustomInspectorHelpers.DrawMinMaxVertSphere(-eyesAnim.EyesClampVertical.y, -eyesAnim.EyesClampVertical.x, 14, 0f);
                    GUILayout.Label(Mathf.Round(eyesAnim.EyesClampVertical.y) + "°", FEditor_Styles.GrayBackground, GUILayout.MaxWidth(40f));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.MinMaxSlider(ref eyesAnim.EyesClampVertical.x, ref eyesAnim.EyesClampVertical.y, -90f, 90f);
                    bothY = EditorGUILayout.Slider("Adjust symmetrical", bothY, 1f, 90f);
                    //EditorGUILayout.PropertyField(sp_EyesClampVertical);

                    if (lastBothY != bothY)
                    {
                        eyesAnim.EyesClampVertical.x = -bothY;
                        eyesAnim.EyesClampVertical.y = bothY;
                        lastBothY = bothY;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndVertical(); ///

                }

                #endregion

                if (drawRanges)
                {
                    GUILayout.Space(4f);

                    EditorGUILayout.BeginVertical(FEditor_Styles.Emerald); ////

                    var drop = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                    GUI.Box(drop, "This angle exceeded - eyes look back at default rotation", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fixedHeight = 22 });

                    if (!wrongLimit)
                        GUI.color = new Color(0.55f, 0.9f, 0.75f, 0.8f);
                    else
                        GUI.color = new Color(0.9f, 0.55f, 0.55f, 0.8f);

                    GUILayout.BeginHorizontal();

                    eyesAnim.EyesMaxRange = EditorGUILayout.Slider("Max Angle Diff", eyesAnim.EyesMaxRange, 15f, 180f);
                    FEditor_CustomInspectorHelpers.DrawMinMaxSphere(-eyesAnim.EyesMaxRange, eyesAnim.EyesMaxRange, 14);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical(); ///
                }


                EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground); ////
                GUI.color = preCol;
                EditorGUILayout.PropertyField(sp_CorrectionOffsets, true);

                //if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_RootFixer);

                EditorGUILayout.EndVertical(); ///
                GUILayout.Space(1f);
                EditorGUILayout.EndVertical(); //
            }

            EditorGUILayout.EndVertical(); //

            #endregion

            EditorGUILayout.EndVertical();
        }

        // Apply changed parameters variables
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Searching through component's owner to find head or neck bone
    /// </summary>
    private void FindHeadBone(FEyesAnimator eyesAnimator)
    {
        // First let's check if it's humanoid character, then we can get head bone transform from it
        Animator animator = eyesAnimator.GetComponent<Animator>();
        Transform animatorHeadBone = null;
        if (animator)
        {
            if (animator.isHuman)
            {
                animatorHeadBone = animator.GetBoneTransform(HumanBodyBones.Head);
            }
        }


        Transform headBone = null;
        Transform probablyWrongTransform = null;

        foreach (Transform t in eyesAnimator.GetComponentsInChildren<Transform>())
        {
            if (t == eyesAnimator.transform) continue;

            if (t.name.ToLower().Contains("head"))
            {
                if (t.GetComponent<SkinnedMeshRenderer>())
                {
                    if (t.parent == eyesAnimator.transform) continue; // If it's just mesh object from first depths
                    probablyWrongTransform = t;
                    continue;
                }

                headBone = t;
                break;
            }
        }

        if (!headBone)
            foreach (Transform t in eyesAnimator.GetComponentsInChildren<Transform>())
            {
                if (t.name.ToLower().Contains("neck"))
                {
                    headBone = t;
                    break;
                }
            }

        if (headBone == null && animatorHeadBone != null)
            headBone = animatorHeadBone;
        else
        if (headBone != null && animatorHeadBone != null)
        {
            if (animatorHeadBone.name.ToLower().Contains("head")) headBone = animatorHeadBone;
            else
                if (!headBone.name.ToLower().Contains("head")) headBone = animatorHeadBone;
        }

        if (headBone)
        {
            eyesAnimator.HeadReference = headBone;
            FindingEyes(eyesAnimator);
        }
        else
        {
            if (probablyWrongTransform) eyesAnimator.HeadReference = probablyWrongTransform;
            Debug.LogWarning("Found " + probablyWrongTransform + " but it's probably wrong transform");
        }
    }

    protected virtual void FindingEyes(FEyesAnimator eyesAnimator)
    {
        if (eyesAnimator.HeadReference == null) return;

        // Trying to find eye bones inside head bone
        Transform[] children = eyesAnimator.HeadReference.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            string lowerName = children[i].name.ToLower();
            if (lowerName.Contains("eye"))
            {
                if (lowerName.Contains("brow") || lowerName.Contains("lid") || lowerName.Contains("las")) continue;

                if (lowerName.Contains("left")) { if (!eyesAnimator.Eyes.Contains(children[i])) eyesAnimator.Eyes.Add(children[i]); continue; }
                else
                    if (lowerName.Contains("l")) { if (!eyesAnimator.Eyes.Contains(children[i])) eyesAnimator.Eyes.Add(children[i]); continue; }

                if (lowerName.Contains("right")) { if (!eyesAnimator.Eyes.Contains(children[i])) eyesAnimator.Eyes.Add(children[i]); continue; }
                else
                    if (lowerName.Contains("r")) { if (!eyesAnimator.Eyes.Contains(children[i])) eyesAnimator.Eyes.Add(children[i]); continue; }
            }
        }
    }

    protected virtual void DrawPre()
    {

    }
}
