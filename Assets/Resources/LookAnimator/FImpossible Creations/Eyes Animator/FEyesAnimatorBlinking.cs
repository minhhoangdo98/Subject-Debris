using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FEyes
{
    /// <summary>
    /// FM: Deriving from eyes animator and adding few extra features for blinking
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Eyes Animator/FEyes Animator Blinking")]
    public class FEyesAnimatorBlinking : FEyesAnimator
    {
        [Tooltip("Eyelids game objects to animate their rotation")]
        public List<Transform> EyeLids;
        [Tooltip("Target rotations for eyelids - closed pose")]
        public List<Vector3> EyeLidsCloseRotations;

        [Tooltip("Syncing frequency of blinking with random movement preset choosed in 'Animation Settings' Tab")]
        public bool SyncWithRandomPreset = true;

        [Range(0.35f, 3f)]
        [Tooltip("How fast should occur eyelids blinking")]
        public float BlinkFrequency = 1f;
        [Range(0.25f, 2f)]
        [Tooltip("If you want eyelids movement to be slower or quicker")]
        public float OpenCloseSpeed = 1f;

        [Tooltip("If each eye should blink in individual random timers")]
        public bool IndividualBlinking = false;

        [Tooltip("If you want simply animate eyelids beeing a bit closed than opened wide for the character. For example you can easily simulate that character is tired.")]
        [Range(0f,1f)]
        public float MinOpenValue = 1f;

        // Version 1.0.5
        [Range(0f, 1.5f)]
        [Tooltip("When we look up in real life, our eyelids are opening a bit more, when looking down, upper lid is closing a bit and down lid is shifting a little - this option will simulate this behaviour if eyelids are setted properly (subtle effect)")]
        public float AdditionalEyelidsMotion = 0f;
        public List<Transform> UpEyelids;
        public List<Transform> DownEyelids;
        private List<int> upEyelidsIndexes;
        private List<int> downEyelidsIndexes;
        private List<float> upEyelidsFactor;
        private List<float> downEyelidsFactor;
        private Vector2 deltaV;

        private float[] blinkTimers;
        private float[] blinkPowers;
        private float[] keepCloseTimes;
        private float[] blinkProgress;

        private Quaternion[] initEyelidsLocalRotations;
        private float blinkScale = 1f;

        protected override void Start()
        {
            base.Start();

            blinkTimers = new float[EyeLids.Count];
            blinkPowers = new float[EyeLids.Count];
            keepCloseTimes = new float[EyeLids.Count];
            blinkProgress = new float[EyeLids.Count];
            initEyelidsLocalRotations = new Quaternion[EyeLids.Count];

            upEyelidsIndexes = new List<int>();
            downEyelidsIndexes = new List<int>();

            upEyelidsFactor = new List<float>();
            downEyelidsFactor = new List<float>();

            for (int i = 0; i < EyeLids.Count; i++)
            {
                initEyelidsLocalRotations[i] = EyeLids[i].localRotation;

                blinkTimers[i] = 1f;
                blinkPowers[i] = 1f;
                keepCloseTimes[i] = 0f;
                blinkProgress[i] = 0f;

                for (int j = 0; j < UpEyelids.Count; j++)
                {
                    if (UpEyelids[j] == EyeLids[i]) upEyelidsIndexes.Add(i);
                    upEyelidsFactor.Add(0f);
                }

                for (int j = 0; j < DownEyelids.Count; j++)
                {
                    if (DownEyelids[j] == EyeLids[i]) downEyelidsIndexes.Add(i);
                    downEyelidsFactor.Add(0f);
                }
            }
        }


        protected override void LateUpdate()
        {
            base.LateUpdate();

            blinkScale = Mathf.Lerp(0.65f, 1.5f, BlinkFrequency / 3f);

            if (!IndividualBlinking)
            {
                CalculateBlinking();

                for (int i = 0; i < EyeLids.Count; i++)
                {
                    AnimateBlinking(i, 0);
                }
            }
            else
            {
                for (int i = 0; i < EyeLids.Count; i++)
                {
                    CalculateBlinking(i);
                    AnimateBlinking(i, i);
                }
            }

            if (SyncWithRandomPreset)
            {
                if ((int)RandomMovementPreset < 2)
                    BlinkFrequency = 1f;
                else if ((int)RandomMovementPreset < 5)
                    BlinkFrequency = 0.65f;
                else if ((int)RandomMovementPreset == 5)
                    BlinkFrequency = 2.75f;
                else
                    BlinkFrequency = 1.45f;
            }
        }

        protected override void ClampDetection(Vector3 deltaVector, ref Vector3 lookRotation, Vector3 rootOffset)
        {
            base.ClampDetection(deltaVector, ref lookRotation, rootOffset);
            deltaV = deltaVector;
        }


        private void CalculateBlinking(int i = 0)
        {
            if (keepCloseTimes[i] <= 0f)
            {
                if (blinkProgress[i] <= 0f)
                {
                    blinkTimers[i] -= Time.deltaTime * BlinkFrequency;
                }
            }

            if (blinkTimers[i] <= 0f)
            {
                blinkTimers[i] = Random.Range(0.75f, 1.50f);
                blinkPowers[i] = Random.Range(3f * (blinkScale), 5f * blinkScale);
                keepCloseTimes[i] = Random.Range(0.125f, 0.25f);
            }
        }

        private void AnimateBlinking(int i, int varInd)
        {
            if (keepCloseTimes[varInd] > 0f)
            {
                blinkProgress[varInd] += Time.deltaTime * blinkPowers[varInd] * OpenCloseSpeed;

                if (blinkProgress[varInd] >= 1f)
                {
                    blinkProgress[varInd] = 1f;
                    keepCloseTimes[varInd] -= Time.deltaTime * blinkScale;
                }
            }
            else
            {
                blinkProgress[varInd] -= Time.deltaTime * blinkPowers[varInd] * 0.87f * OpenCloseSpeed;
            }

            // Changing opened eyes lids rotation when using UpDownEyelidsFactor
            Quaternion newBackRotation = initEyelidsLocalRotations[i];
            Quaternion closeRotation = Quaternion.Euler(EyeLidsCloseRotations[i]);

            if (MinOpenValue < 1f) newBackRotation = Quaternion.Slerp(closeRotation, initEyelidsLocalRotations[i], MinOpenValue);

            #region V1.0.5 Additional Eyelids Motion

            if (AdditionalEyelidsMotion > 0f)
            {
                bool upDowned = false;
                if (UpEyelids.Count > 0)
                {
                    int j = -1;
                    for (int k = 0; k < upEyelidsIndexes.Count; k++) if (upEyelidsIndexes[k] == i) j = k;
                    float targetFactor;

                    if (j >= 0)
                    {
                        int lagId = 0;
                        if (IndividualLags) lagId = i;

                        if (deltaV.x > 1)
                        {
                            float openMoreFactor = Mathf.Lerp(0f, -0.475f * AdditionalEyelidsMotion, Mathf.InverseLerp(1, EyesClampVertical.y, deltaV.x));

                            if (EyesLagAmount > 0f)
                            {
                                if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(openMoreFactor, upEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = openMoreFactor;
                            }
                            else
                                targetFactor = openMoreFactor;
                        }
                        else
                            if (deltaV.x < -1)
                        {
                            float closeFactor = Mathf.Lerp(0f, 0.4f * AdditionalEyelidsMotion, Mathf.InverseLerp(-1, EyesClampVertical.x, deltaV.x));

                            if (EyesLagAmount > 0f)
                            {
                                if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(closeFactor, upEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = closeFactor;
                            }
                            else targetFactor = closeFactor;
                        }
                        else
                        {
                            if (EyesLagAmount > 0f)
                            {
                                if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(0f, upEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = 0f;
                            }
                            else targetFactor = 0f;
                        }

                        upEyelidsFactor[j] = Mathf.Lerp(upEyelidsFactor[j], targetFactor, Time.deltaTime * eyesSpeedValue);

                        newBackRotation = Quaternion.SlerpUnclamped(newBackRotation, Quaternion.Euler(EyeLidsCloseRotations[i]), upEyelidsFactor[j]);

                        upDowned = true;
                    }
                }

                if (!upDowned)
                    if (DownEyelids.Count > 0)
                    {
                        int j = -1;
                        for (int k = 0; k < downEyelidsIndexes.Count; k++) if (downEyelidsIndexes[k] == i) j = k;
                        float targetFactor;

                        if (j >= 0)
                        {
                            int lagId = 0;
                            if (IndividualLags) lagId = i;

                            if (deltaV.x > 1)
                            {
                                float closeFactor = Mathf.Lerp(0f, 0.3f * AdditionalEyelidsMotion, Mathf.InverseLerp(1, EyesClampVertical.y, deltaV.x));

                                if (EyesLagAmount > 0f)
                                {
                                    if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(closeFactor, downEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = closeFactor;
                                }
                                else
                                    targetFactor = closeFactor;
                            }
                            else
                                if (deltaV.x < -1)
                            {
                                float openMoreFactor = Mathf.Lerp(-1.9f * AdditionalEyelidsMotion, 0f, Mathf.InverseLerp(EyesClampVertical.x, -1, deltaV.x));

                                if (EyesLagAmount > 0f)
                                {
                                    if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(openMoreFactor, downEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = openMoreFactor;
                                }
                                else
                                    targetFactor = openMoreFactor;
                            }
                            else
                            {
                                if (EyesLagAmount > 0f)
                                {
                                    if (lagProgresses[lagId] > 0f) targetFactor = Mathf.Lerp(0f, downEyelidsFactor[j], lagProgresses[lagId] * EyesLagAmount); else targetFactor = 0f;
                                }
                                else
                                    targetFactor = 0f;
                            }

                            downEyelidsFactor[j] = Mathf.Lerp(downEyelidsFactor[j], targetFactor, Time.deltaTime * eyesSpeedValue);

                            newBackRotation = Quaternion.SlerpUnclamped(newBackRotation, Quaternion.Euler(EyeLidsCloseRotations[i]), downEyelidsFactor[j]);
                        }
                    }
            }
            #endregion

            EyeLids[i].localRotation = Quaternion.Lerp(newBackRotation, closeRotation, blinkProgress[varInd]);

        }

        public override void UpdateLists()
        {
            base.UpdateLists();

            if (EyeLids == null) EyeLids = new List<Transform>();

            if (UpEyelids != null)
                for (int i = 0; i < UpEyelids.Count; i++)
                    if (!EyeLids.Contains(UpEyelids[i]))
                        EyeLids.Add(UpEyelids[i]);


            if (DownEyelids != null)
                for (int i = 0; i < DownEyelids.Count; i++)
                    if (!EyeLids.Contains(DownEyelids[i])) EyeLids.Add(DownEyelids[i]);


            if (EyeLidsCloseRotations == null) EyeLidsCloseRotations = new List<Vector3>();

            if (EyeLids.Count != EyeLidsCloseRotations.Count)
            {
                EyeLidsCloseRotations.Clear();

                for (int i = 0; i < EyeLids.Count; i++)
                {
                    if (EyeLids[i] == null) continue;
                    EyeLidsCloseRotations.Add(EyeLids[i].localRotation.eulerAngles);
                }

                //if (EyeLidsCloseRotations.Count > EyeLids.Count)
                //    for (int i = 0; i < EyeLidsCloseRotations.Count - EyeLids.Count; i++) EyeLidsCloseRotations.RemoveAt(EyeLidsCloseRotations.Count - 1);
                //else
                //    for (int i = 0; i < EyeLids.Count - EyeLidsCloseRotations.Count; i++) if (EyeLids[EyeLids.Count - 1] != null) EyeLidsCloseRotations.Add(EyeLids[EyeLids.Count - 1].localRotation.eulerAngles);
            }
        }
    }
}