using FIMSpace.FEditor;
using FIMSpace.FEyes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(FEyesAnimatorBlinking))]
public class FEyesAnimatorBlinking_Editor : FEyesAnimator_Editor
{
    private bool focusCloseRoations = false;
    private List<Quaternion> openRotations;

    public static bool drawBlinking = true;
    public static bool showEyelids = true;

    protected SerializedProperty sp_EyeLids;
    protected SerializedProperty sp_EyeLidsCloseRotations;
    protected SerializedProperty sp_SyncWithRandomPreset;
    protected SerializedProperty sp_BlinkFrequency;
    protected SerializedProperty sp_OpenCloseSpeed;
    protected SerializedProperty sp_IndividualBlinking;
    protected SerializedProperty sp_MinOpenValue;

    protected SerializedProperty sp_UpDownEyelidsFactor;
    protected SerializedProperty sp_UpEyelids;
    protected SerializedProperty sp_DownEyelids;


    protected override void OnEnable()
    {
        base.OnEnable();

        sp_EyeLids = serializedObject.FindProperty("EyeLids");
        sp_EyeLidsCloseRotations = serializedObject.FindProperty("EyeLidsCloseRotations");
        sp_SyncWithRandomPreset = serializedObject.FindProperty("SyncWithRandomPreset");
        sp_BlinkFrequency = serializedObject.FindProperty("BlinkFrequency");
        sp_OpenCloseSpeed = serializedObject.FindProperty("OpenCloseSpeed");
        sp_IndividualBlinking = serializedObject.FindProperty("IndividualBlinking");
        sp_MinOpenValue = serializedObject.FindProperty("MinOpenValue");

        sp_UpDownEyelidsFactor = serializedObject.FindProperty("AdditionalEyelidsMotion");
        sp_UpEyelids = serializedObject.FindProperty("UpEyelids");
        sp_DownEyelids = serializedObject.FindProperty("DownEyelids");
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Eyes Animator Blinking Inspector Operations");

        base.OnInspectorGUI();

        if (drawDefaultInspector) return;

        FEyesAnimatorBlinking eyesAnim = (FEyesAnimatorBlinking)target;

        Color preCol = GUI.color;

        EditorGUI.indentLevel--;
        EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground); //
        EditorGUI.indentLevel++;

        if (eyesAnim.EyeLids == null) eyesAnim.EyeLids = new List<Transform>();

        drawBlinking = EditorGUILayout.Foldout(drawBlinking, "Blinking Animation Options", true);

        if (drawBlinking)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); ///

            GUILayout.BeginVertical(FEditor_Styles.Emerald); ////

            GUILayout.BeginHorizontal();
            showEyelids = EditorGUILayout.Foldout(showEyelids, "Eyelids Game Objects (" + eyesAnim.EyeLids.Count + ")", true);

            if (showEyelids)
            {
                if (!Application.isPlaying)
                {
                    if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = preCol;
                    if (GUILayout.Button(new GUIContent("Lock Inspector", "Locking Inspector Window to help Drag & Drop operations"), new GUILayoutOption[2] { GUILayout.Width(106), GUILayout.Height(16) })) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;

                    GUI.color = preCol;

                    if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                    {
                        eyesAnim.EyeLids.Add(null);
                        eyesAnim.UpdateLists();
                        EditorUtility.SetDirty(target);
                    }
                }
            }

            GUILayout.EndHorizontal();
            GUI.color = preCol;

            if (showEyelids)
            {
                GUI.color = new Color(0.5f, 1f, 0.5f, 0.9f);

                if (!Application.isPlaying)
                {
                    var drop = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                    GUI.Box(drop, "Drag & Drop your eyelids GameObjects here", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fixedHeight = 22 });
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
                                        if (!eyesAnim.EyeLids.Contains(draggedObject.transform)) eyesAnim.EyeLids.Add(draggedObject.transform);
                                        EditorUtility.SetDirty(target);
                                    }
                                }

                                eyesAnim.UpdateLists();
                            }

                            Event.current.Use();
                            break;
                    }
                }


                if (focusCloseRoations) GUI.color = new Color(0.14f, 0.9f, 0.05f, 0.9f); else GUI.color = preCol;

                if (GUILayout.Button(new GUIContent("Focus On Close Rotations", "Changing close rotations for eyes in editor mode, so you can easily adjust them"), EditorStyles.miniButton))
                {
                    focusCloseRoations = !focusCloseRoations;

                    if (focusCloseRoations)
                    {
                        if (openRotations == null) openRotations = new List<Quaternion>(); else openRotations.Clear();

                        for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                            if (eyesAnim.EyeLids[i] != null)
                            {
                                openRotations.Add(eyesAnim.EyeLids[i].localRotation);
                                eyesAnim.EyeLids[i].localRotation = Quaternion.Euler(eyesAnim.EyeLidsCloseRotations[i]);
                            }
                    }
                    else
                    {
                        for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                            if (eyesAnim.EyeLids[i] != null) if (i < openRotations.Count) eyesAnim.EyeLids[i].localRotation = openRotations[i];
                    }
                }

                GUI.color = preCol;

                for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    string name;
                    if (!eyesAnim.EyeLids[i])
                    {
                        name = "Assign Object";
                        GUI.color = new Color(0.9f, 0.4f, 0.4f, 0.9f);
                    }
                    else
                    {
                        name = eyesAnim.EyeLids[i].name;
                        if (name.Length > 16) name = eyesAnim.EyeLids[i].name.Substring(0, 12) + "...";
                    }

                    eyesAnim.EyeLids[i] = (Transform)EditorGUILayout.ObjectField("[" + i + "] " + name, eyesAnim.EyeLids[i], typeof(Transform), true);

                    if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                    {
                        if (!Application.isPlaying)
                        {
                            if (eyesAnim.UpEyelids.Contains(eyesAnim.EyeLids[i])) eyesAnim.UpEyelids.Remove(eyesAnim.EyeLids[i]);
                            else
                                if (eyesAnim.DownEyelids.Contains(eyesAnim.EyeLids[i])) eyesAnim.DownEyelids.Remove(eyesAnim.EyeLids[i]);

                            eyesAnim.EyeLids.RemoveAt(i);
                            eyesAnim.EyeLidsCloseRotations.RemoveAt(i);
                            eyesAnim.UpdateLists();
                            EditorUtility.SetDirty(target);
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.EndHorizontal();

                        if (i < eyesAnim.EyeLidsCloseRotations.Count)
                        {
                            Vector3 preEye = eyesAnim.EyeLidsCloseRotations[i];
                            eyesAnim.EyeLidsCloseRotations[i] = EditorGUILayout.Vector3Field("Close Rotation", eyesAnim.EyeLidsCloseRotations[i]);

                            if (eyesAnim.EyeLidsCloseRotations[i] != preEye)
                            {
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }

                    GUI.color = preCol;
                }

                GUILayout.Space(3);
            }

            EditorGUILayout.EndVertical(); ///

            GUILayout.Space(4f);
            EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.6f, 0.4f, 0.8f, 0.07f)));

            EditorGUILayout.PropertyField(sp_UpDownEyelidsFactor);

            if (eyesAnim.UpEyelids != null && eyesAnim.DownEyelids != null)
                if (eyesAnim.UpEyelids.Count == 0 && eyesAnim.DownEyelids.Count == 0)
                {
                    EditorGUILayout.HelpBox("Tell the component which GameObjects are upper and which are lower eyelids", MessageType.None);

                    if (eyesAnim.EyeLids.Count > 0)
                    {
                        if (GUILayout.Button("Try detect with 'EyeLids' array"))
                            for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                            {
                                if (eyesAnim.EyeLids[i] == null) continue;

                                if (eyesAnim.EyeLids[i].name.Contains("Up")) eyesAnim.UpEyelids.Add(eyesAnim.EyeLids[i]);

                                if (eyesAnim.EyeLids[i].name.Contains("Down")) eyesAnim.DownEyelids.Add(eyesAnim.EyeLids[i]);
                                else
                                if (eyesAnim.EyeLids[i].name.Contains("Lower")) eyesAnim.DownEyelids.Add(eyesAnim.EyeLids[i]);
                                EditorUtility.SetDirty(target);
                            }

                        if (eyesAnim.EyeLids.Count > 0) if (eyesAnim.AdditionalEyelidsMotion <= 0f)
                            {
                                eyesAnim.AdditionalEyelidsMotion = 1f;
                                EditorUtility.SetDirty(target);
                            }
                    }
                }

            EditorGUILayout.PropertyField(sp_UpEyelids, true);
            EditorGUILayout.PropertyField(sp_DownEyelids, true);

            EditorGUILayout.EndVertical();

            GUILayout.Space(4f);

            EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground); ////

            GUI.color = new Color(0.95f, 0.95f, 1f, 0.92f);

            EditorGUILayout.PropertyField(sp_SyncWithRandomPreset);
            if (!eyesAnim.SyncWithRandomPreset) EditorGUILayout.PropertyField(sp_BlinkFrequency);
            EditorGUILayout.PropertyField(sp_OpenCloseSpeed);
            EditorGUILayout.PropertyField(sp_IndividualBlinking);
            EditorGUILayout.PropertyField(sp_MinOpenValue);

            EditorGUILayout.EndVertical(); ///

            GUI.color = preCol;
            GUILayout.Space(3f);

            EditorGUILayout.EndVertical(); //

            GUILayout.Space(3f);
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (focusCloseRoations)
        {
            if (openRotations != null)
            {
                FEyesAnimatorBlinking eyesAnim = target as FEyesAnimatorBlinking;

                if (eyesAnim.EyeLids != null)
                    for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                        if (eyesAnim.EyeLids[i] != null) eyesAnim.EyeLids[i].localRotation = Quaternion.Euler(eyesAnim.EyeLidsCloseRotations[i]);
            }
        }
    }

    protected override void FindingEyes(FEyesAnimator eyesAnimator)
    {
        base.FindingEyes(eyesAnimator);

        FEyesAnimatorBlinking blinking = eyesAnimator as FEyesAnimatorBlinking;

        if (eyesAnimator.HeadReference)
        {
            if (eyesAnimator.HeadReference == null) return;

            if (blinking.EyeLids == null) blinking.EyeLids = new List<Transform>();
            if (blinking.DownEyelids == null) blinking.DownEyelids = new List<Transform>();
            if (blinking.UpEyelids == null) blinking.UpEyelids = new List<Transform>();

            // Trying to find eyelid bones inside eyes bones
            for (int e = 0; e < eyesAnimator.Eyes.Count; e++)
            {
                Transform[] children = eyesAnimator.Eyes[e].GetComponentsInChildren<Transform>();

                for (int i = 0; i < children.Length; i++)
                {
                    string lowerName = children[i].name.ToLower();
                    if (lowerName.Contains("lid"))
                    {
                        if (lowerName.Contains("low") || lowerName.Contains("down") || lowerName.Contains("bot"))
                        {
                            if (!blinking.DownEyelids.Contains(children[i])) blinking.DownEyelids.Add(children[i]);
                        }
                        else
                        if (lowerName.Contains("up") || lowerName.Contains("top"))
                        {
                            if (!blinking.UpEyelids.Contains(children[i])) blinking.UpEyelids.Add(children[i]);
                        }
                        else
                        {
                            if (!blinking.EyeLids.Contains(children[i])) blinking.EyeLids.Add(children[i]);
                        }
                    }
                }

                blinking.UpdateLists();
            }
        }
    }

    private void OnDisable()
    {
        if (focusCloseRoations)
        {
            if (openRotations != null)
            {
                FEyesAnimatorBlinking eyesAnim = target as FEyesAnimatorBlinking;

                for (int i = 0; i < eyesAnim.EyeLids.Count; i++)
                    if (eyesAnim.EyeLids[i] != null) eyesAnim.EyeLids[i].localRotation = openRotations[i];
            }
        }
    }
}
