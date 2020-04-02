using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FEyes
{
    /// <summary>
    /// FM: Class which is controlling eyes spheres to make them move to follow objects positions
    /// simulate random eyes movement, simulating eye movement lags etc.
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Eyes Animator/FEyes Animator")]
    public class FEyesAnimator : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public bool debugSwitch = false;
        public string EditorIconPath { get { return "Eyes Animator/Eyes Animator Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        [Tooltip("Target to look for eyes")]
        public Transform EyesTarget;

        [Tooltip("Head transform reference for look start position and also reference to limit range how much eyes can rotate")]
        public Transform HeadReference;

        [Tooltip("Sometimes head bone can be too low or too high and you can correct this with StartLookOffset (depends of head bone rotations and scale etc. position axes can behave unusual)")]
        public Vector3 StartLookOffset;
        //[Space(8f)]

        [Tooltip("Eyes transforms / bones (origin/pivot should be in center of the sphere")]
        public List<Transform> Eyes;

        [Tooltip("You can smoothly change it to 0 if you want to disable eyes animation")]
        [Range(0f, 1f)]
        public float EyesBlend = 1f;
        [Tooltip("How fast eyes should rotate to desired rotations")]
        [Range(0.0f, 2f)]
        public float EyesSpeed = 1f;
        protected float eyesSpeedValue = 1f; // For quicker access in derived classes for calculations etc.

        //[Space(4f)]
        [Tooltip("Compensating rotations for eyes to avoid squinting, sometimes you will want to have some of this")]
        [Range(0.0f, 1f)]
        public float SquintPreventer = 1f;

        //[Space(8f)]
        [Tooltip("Additional random movement for eyes giving more natural feel - you can crank it up for example when there is no target for eyes, or when character is talking with someone")]
        [Range(0f, 1f)]
        public float EyesRandomMovement = 0.3f;
        public Vector2 RandomMovementAxisScale = Vector2.one;

        public FERandomMovementType RandomMovementPreset = FERandomMovementType.Default;

        [Tooltip("How frequently should occur rotation change for random eyes movement")]
        [Range(0f, 3f)]
        public float RandomizingSpeed = 1f;

        [Tooltip("Option for monsters, each eye will have individual random rotation direction")]
        public bool EyesRandomMovementIndividual = false;


        //[Space(8f)]
        [Tooltip("When we rotate eyes in real life, they're reaching target with kinda jumpy movement, but for more toon effect you can left this value at 0")]
        [Range(0f, 1f)]
        public float EyesLagAmount = 0.65f;
        [Tooltip("Making lags a bit smaller and more frequent when setted to lower value")]
        [Range(0.1f, 1f)]
        public float LagStiffness = 1f;
        [Tooltip("Option for monsters, each eye will have individual random delay for movement")]
        public bool IndividualLags = false;



        //[Space(8f)]
        [Tooltip("In what angle eyes should go back to deafult position")]
        [Range(5f, 180f)]
        public float EyesMaxRange = 180f;
        [Tooltip("Maximum distance of target to look at, when exceed eyes will go back to default rotation. When max distance is equal 0, distance limit is infinite")]
        public float EyesMaxDistance = 0f;
        [Range(0.25f, 4f)]
        [Tooltip("Fading in/out eyes blend when max range is exceeded")]
        public float BlendTransitionSpeed = 1f;

        //[Space(8f)]
        public Vector2 EyesClampHorizontal = new Vector2(-35f, 35f);
        public Vector2 EyesClampVertical = new Vector2(-35f, 35f);

        //[Tooltip("If root is offsetted somehow (guide with arrow going from middle of model in red colour) you can fix it to face forward with this variable")]
        //public Vector3 RootFixer = Vector3.zero;
        public List<Vector3> CorrectionOffsets;

        private Vector3 targetLookPosition;

        private float conditionalBlend = 1f;
        //private float[] conditionalBlend;
        private Vector3 targetLookPositionOffset = Vector3.zero;

        // Rotation animation variables
        private Vector3[] eyeForwards;
        private Quaternion[] eyesInitLocalRotations;
        private Quaternion[] eyesLerpRotations;
        private Vector3 headForward;

        // Additional features
        private Vector3[] randomDirs;
        private float[] randomTimers;
        private float[] lagTimers;
        protected float[] lagProgresses;
        private Quaternion[] lagStartRotations;
        private float[] changeSmoothers;
        private bool changeFlag = true;

        /// <summary>
        /// Preparing all needed variables and references
        /// </summary>
        protected virtual void Start()
        {
            eyeForwards = new Vector3[Eyes.Count];
            eyesInitLocalRotations = new Quaternion[Eyes.Count];
            eyesLerpRotations = new Quaternion[Eyes.Count];
            randomDirs = new Vector3[Eyes.Count];
            lagTimers = new float[Eyes.Count];
            lagProgresses = new float[Eyes.Count];
            changeSmoothers = new float[Eyes.Count];
            lagStartRotations = new Quaternion[Eyes.Count];
            randomTimers = new float[Eyes.Count];

            for (int i = 0; i < eyeForwards.Length; i++)
            {
                //Vector3 rootPos = Eyes[i].position;
                //Vector3 targetPos = Eyes[i].position + Vector3.Scale(transform.forward, Eyes[i].transform.lossyScale);
                //eyeForwards[i] = (Eyes[i].InverseTransformPoint(targetPos) - Eyes[i].InverseTransformPoint(rootPos)).normalized;
                Vector3 rootPos = Eyes[i].position;
                Vector3 targetPos = Eyes[i].position + Vector3.Scale(transform.forward, Eyes[i].transform.lossyScale);
                eyeForwards[i] = (Eyes[i].InverseTransformPoint(targetPos) - Eyes[i].InverseTransformPoint(rootPos)).normalized;

                eyesInitLocalRotations[i] = Eyes[i].localRotation;
                eyesLerpRotations[i] = Eyes[i].rotation;
                lagStartRotations[i] = Eyes[i].rotation;

                randomTimers[i] = 0f;
                randomDirs[i] = Vector3.zero;
                lagTimers[i] = 0f;
                lagProgresses[i] = 1f;
                changeSmoothers[i] = 1f;
            }

            //headForward = Quaternion.FromToRotation(HeadReference.forward, transform.forward) * transform.forward;
            headForward = Quaternion.FromToRotation(HeadReference.InverseTransformDirection(transform.forward), Vector3.forward) * Vector3.forward;
            //headForward = (HeadReference.InverseTransformPoint(HeadReference.position) - (HeadReference.InverseTransformPoint(HeadReference.position + transform.forward))).normalized;
            //headForward = (HeadReference.InverseTransformPoint(HeadReference.position) - (HeadReference.InverseTransformPoint(HeadReference.position + transform.forward + transform.TransformVector(RootFixer)))).normalized;
        }


        /// <summary>
        /// Executing procedural animation
        /// </summary>
        protected virtual void LateUpdate()
        {
            for (int i = 0; i < Eyes.Count; i++)
            {
                Eyes[i].localRotation = eyesInitLocalRotations[i];
            }

            Vector3 headRotation = Vector3.zero;
            Quaternion lookRotationBaseClamped = Quaternion.identity;

            if (EyesTarget)
                targetLookPosition = EyesTarget.position + targetLookPositionOffset;
            else
                targetLookPosition = HeadReference.position + transform.forward * 5f;

            ComputeBaseRotation(ref headRotation, ref lookRotationBaseClamped);

            if (conditionalBlend <= 0f) return;
            if (EyesBlend <= 0f) return;

            // Calculations for each eye
            for (int i = 0; i < Eyes.Count; i++)
            {
                #region Additional features calculations

                int lagId = 0;
                int randomId = 0;

                if (i == 0)
                {
                    changeSmoothers[0] = Mathf.Lerp(changeSmoothers[0], 1f, Time.deltaTime * 1f);

                    CalculateLagTimer(0);
                    CalculateRandomTimer(0);
                }
                else
                {
                    if (EyesRandomMovementIndividual)
                    {
                        changeSmoothers[i] = Mathf.Lerp(changeSmoothers[i], 1f, Time.deltaTime * 1f);

                        ComputeBaseRotation(ref headRotation, ref lookRotationBaseClamped, i);

                        CalculateRandomTimer(i);
                        randomId = i;
                    }

                    if (IndividualLags)
                    {
                        lagId = i;
                        CalculateLagTimer(i);
                    }
                    else
                    {
                        CalculateLagTimerNonIndividualEvent(i);
                    }
                }

                #endregion

                Quaternion initRot = Eyes[i].rotation;

                #region Not squinted rotation

                Quaternion notSquintedRotation = lookRotationBaseClamped;

                notSquintedRotation *= Quaternion.FromToRotation(eyeForwards[i], Vector3.forward);
                notSquintedRotation *= eyesInitLocalRotations[i];

                Eyes[i].rotation = notSquintedRotation;
                Eyes[i].rotation *= Quaternion.Inverse(eyesInitLocalRotations[i]);
                notSquintedRotation = Eyes[i].rotation;

                #endregion

                Quaternion targetLookRotation = notSquintedRotation;

                #region Individual rotation

                Quaternion individualRotation = notSquintedRotation;

                if (SquintPreventer < 1f)
                {
                    Quaternion lookRotationQuatInd = Quaternion.LookRotation(targetLookPosition - Eyes[i].position);
                    Vector3 lookRotationInd = lookRotationQuatInd.eulerAngles;

                    if (randomDirs[randomId] != Vector3.zero) lookRotationInd += Vector3.LerpUnclamped(Vector3.zero, randomDirs[randomId], EyesRandomMovement);

                    // Additional features calculations before clamping
                    Vector2 deltaVectorInd = new Vector3(Mathf.DeltaAngle(lookRotationInd.x, headRotation.x), Mathf.DeltaAngle(lookRotationInd.y, headRotation.y));
                    ClampDetection(deltaVectorInd, ref lookRotationInd, headRotation);

                    // Getting clamped rotation
                    individualRotation = Quaternion.Euler(lookRotationInd);

                    individualRotation *= Quaternion.FromToRotation(eyeForwards[i], Vector3.forward);
                    individualRotation *= eyesInitLocalRotations[i];

                    Eyes[i].rotation = individualRotation;
                    Eyes[i].rotation *= Quaternion.Inverse(eyesInitLocalRotations[i]);
                    individualRotation = Eyes[i].rotation;

                    targetLookRotation = Quaternion.Slerp(individualRotation, notSquintedRotation, SquintPreventer);
                }

                #endregion


                if (CorrectionOffsets[i] != Vector3.zero) targetLookRotation *= Quaternion.Euler(CorrectionOffsets[i]);

                // Eye lag feature 
                if (EyesLagAmount > 0f) if (lagProgresses[lagId] > 0f) targetLookRotation = Quaternion.Slerp(targetLookRotation, lagStartRotations[i], lagProgresses[lagId] * EyesLagAmount);

                eyesSpeedValue = Mathf.LerpUnclamped(2f, 60f, EyesSpeed);
                eyesLerpRotations[i] = Quaternion.Slerp(eyesLerpRotations[i], targetLookRotation, Time.deltaTime * eyesSpeedValue * Mathf.Lerp(1f, changeSmoothers[randomId], EyesRandomMovement));

                Eyes[i].rotation = Quaternion.Slerp(initRot, eyesLerpRotations[i], EyesBlend * conditionalBlend);
            }

            changeFlag = false;
        }


        #region Public handy methods

        /// <summary>
        /// Setting target to look at for eyes with option to offset point of interest
        /// </summary>
        public void SetEyesTarget(Transform target, Vector3? offset = null)
        {
            EyesTarget = target;
            if (offset != null) targetLookPositionOffset = (Vector3)offset; else targetLookPositionOffset = Vector3.zero;
        }

        /// <summary>
        /// Changing blend value of component down to disabled state
        /// </summary>
        public void BlendOutEyesAnimation(float timeInSeconds = 0.5f)
        {
            StopAllCoroutines();
            StartCoroutine(BlendInOut(0f, timeInSeconds));
        }

        /// <summary>
        /// Changing blend value of component up to fully enabled state
        /// </summary>
        public void BlendInEyesAnimation(float timeInSeconds = 0.5f)
        {
            StopAllCoroutines();
            StartCoroutine(BlendInOut(1f, timeInSeconds));
        }

        #endregion


        private void ComputeBaseRotation(ref Vector3 headRotation, ref Quaternion lookRotationBaseClamped, int randomIndex = 0)
        {
            Quaternion lookRotationQuatBase;
            Vector3 lookRotationBase;
            Vector2 deltaVector;

            // Look position referencing from middle of head for unsquinted look rotation
            Vector3 lookStartPositionBase;
            lookStartPositionBase = transform.position;
            lookStartPositionBase.y = HeadReference.position.y;
            lookStartPositionBase += HeadReference.TransformVector(StartLookOffset);

            lookRotationQuatBase = Quaternion.LookRotation(targetLookPosition - lookStartPositionBase);
            lookRotationBase = lookRotationQuatBase.eulerAngles;

            if (randomDirs[randomIndex] != Vector3.zero) lookRotationBase += Vector3.Lerp(Vector3.zero, randomDirs[randomIndex], EyesRandomMovement);

            // Head rotation to offset clamp ranges in head rotates in animation clip of skeleton
            headRotation = (HeadReference.rotation * Quaternion.FromToRotation(headForward, Vector3.forward)).eulerAngles;

            // Vector with degrees differences to all needed axes
            deltaVector = new Vector3(Mathf.DeltaAngle(lookRotationBase.x, headRotation.x), Mathf.DeltaAngle(lookRotationBase.y, headRotation.y));

            // Clamping look rotation
            ClampDetection(deltaVector, ref lookRotationBase, headRotation);

            lookRotationBaseClamped = Quaternion.Euler(lookRotationBase);


            bool outOfRange = false;

            // Range blending out eyes animation
            if (Mathf.Abs(deltaVector.y) > EyesMaxRange)
            {
                outOfRange = true;
            }
            else
            {
                if (EyesMaxDistance > 0f)
                {
                    float distance = Vector3.Distance(lookStartPositionBase, targetLookPosition);
                    if (distance > EyesMaxDistance) outOfRange = true;
                }
            }

            if (outOfRange)
                conditionalBlend = Mathf.Max(0f, conditionalBlend - Time.deltaTime * 5f);
            else
                conditionalBlend = Mathf.Min(1f, conditionalBlend + Time.deltaTime * 5f);
        }


        /// <summary>
        /// Handling eye lag simulation
        /// </summary>
        private void CalculateLagTimer(int i)
        {
            lagTimers[i] -= Time.deltaTime / LagStiffness;

            if (lagProgresses[i] > 0)
            {
                if (lagTimers[i] < 0f)
                {
                    lagProgresses[i] -= Random.Range(0.4f, 0.85f) * Time.deltaTime * 50f * LagStiffness;
                }
            }
            else
            {
                if (lagProgresses[i] <= 0)
                    if (lagTimers[i] < 0f)
                    {
                        lagProgresses[i] = 1f;
                        lagStartRotations[i] = eyesLerpRotations[i];
                        changeFlag = true;
                    }
            }

            if (lagTimers[i] < 0f) lagTimers[i] = Random.Range(0.15f, 0.34f);
        }


        private void CalculateLagTimerNonIndividualEvent(int i)
        {
            if (changeFlag) lagStartRotations[i] = eyesLerpRotations[i];
        }


        protected virtual void ClampDetection(Vector3 deltaVector, ref Vector3 lookRotation, Vector3 rootOffset)
        {
            // Limit when looking left or right
            if (deltaVector.y > -EyesClampHorizontal.x)
                lookRotation.y = rootOffset.y - EyesClampHorizontal.y;
            else if (deltaVector.y < -EyesClampHorizontal.y)
                lookRotation.y = rootOffset.y + EyesClampHorizontal.y;

            // Limit when looking up or down
            if (deltaVector.x > EyesClampVertical.y)
                lookRotation.x = rootOffset.x - EyesClampVertical.y;
            else if (deltaVector.x < EyesClampVertical.x)
                lookRotation.x = rootOffset.x - EyesClampVertical.x;
        }


        /// <summary>
        /// Handling random eye movement simulation
        /// </summary>
        private void CalculateRandomTimer(int i)
        {
            randomTimers[i] -= Time.deltaTime * RandomizingSpeed;

            if (randomTimers[i] < 0f)
            {
                // If random rotation is directed away right now, we want go it back to center a bit
                if (randomDirs[i].magnitude > (EyesClampHorizontal.magnitude + EyesClampVertical.magnitude) / 8)
                {
                    float range = 5f;
                    randomDirs[i] = new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0f);
                    randomDirs[i] = Vector2.Scale(randomDirs[i], RandomMovementAxisScale);

                    switch (RandomMovementPreset)
                    {
                        case FERandomMovementType.Default:
                        case FERandomMovementType.Listening:
                        case FERandomMovementType.Calm:
                        case FERandomMovementType.Focused:
                            randomTimers[i] = Random.Range(0.9f, 2.4f);
                            break;
                        case FERandomMovementType.Nervous:
                            randomTimers[i] = Random.Range(0.2f, 0.6f);
                            break;
                        case FERandomMovementType.AccessingImaginedVisual:
                        case FERandomMovementType.AccessingImaginedAuditory:
                        case FERandomMovementType.AccessingFeelings:
                        case FERandomMovementType.AccessingVisualMemory:
                        case FERandomMovementType.AccessingAuditoryMemory:
                        case FERandomMovementType.AccessingInternalSelfTalk:
                            randomTimers[i] = Random.Range(0.6f, 0.9f);
                            break;
                    }
                }
                else
                {
                    switch (RandomMovementPreset)
                    {
                        case FERandomMovementType.Default:
                            randomTimers[i] = Random.Range(0.4f, 1.24f);
                            randomDirs[i] = new Vector3(Random.Range(-28, 28), Random.Range(-28, 28), 0f);
                            break;
                        case FERandomMovementType.Listening:
                            randomTimers[i] = Random.Range(0.4f, 1.24f);
                            randomDirs[i] = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0f);
                            break;
                        case FERandomMovementType.Calm:
                            randomTimers[i] = Random.Range(1.11f, 2.2f);
                            randomDirs[i] = new Vector3(Random.Range(-25, 25), Random.Range(-25, 25), 0f);
                            break;
                        case FERandomMovementType.Focused:
                            randomTimers[i] = Random.Range(1.3f, 3.2f);
                            randomDirs[i] = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0f);
                            break;
                        case FERandomMovementType.Nervous:
                            randomTimers[i] = Random.Range(0.165f, 0.34f);
                            randomDirs[i] = new Vector3(Random.Range(-24, 24), Random.Range(-24, 24), 0f);
                            break;

                        case FERandomMovementType.AccessingImaginedVisual:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(-50, -35), Random.Range(40, 60), 0f);
                            break;
                        case FERandomMovementType.AccessingImaginedAuditory:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(-3, 3), Random.Range(40, 60), 0f);
                            break;
                        case FERandomMovementType.AccessingFeelings:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(30, 40), Random.Range(40, 60), 0f);
                            break;

                        case FERandomMovementType.AccessingVisualMemory:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(-60, -40), Random.Range(-60, -40), 0f);
                            break;
                        case FERandomMovementType.AccessingAuditoryMemory:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(-3, 3), Random.Range(-60, -40), 0f);
                            break;
                        case FERandomMovementType.AccessingInternalSelfTalk:
                            randomTimers[i] = Random.Range(0.45f, 1.7f);
                            randomDirs[i] = new Vector3(Random.Range(40, 60), Random.Range(-60, -40), 0f);
                            break;
                    }

                    randomDirs[i] = Vector2.Scale(randomDirs[i], RandomMovementAxisScale);
                }

                // Smoothing a little speed for eye when new rotation is choosed
                float mul = Mathf.Lerp(0.4f, 1.3f, EyesLagAmount);
                changeSmoothers[i] = Random.Range(0.5f, 0.85f) * mul;
            }
        }


        #region Helper Methods


        private IEnumerator BlendInOut(float blendTo, float time)
        {
            float elapsed = 0f;
            float startVal = EyesBlend;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                EyesBlend = Mathf.Lerp(startVal, blendTo, elapsed / time);

                yield return null;
            }

            EyesBlend = blendTo;
            yield break;
        }


        protected virtual void OnValidate()
        {
            UpdateLists();
        }


        public virtual void UpdateLists()
        {
            if (Eyes == null) Eyes = new List<Transform>();
            if (CorrectionOffsets == null) CorrectionOffsets = new List<Vector3>();

            if (Eyes.Count != CorrectionOffsets.Count)
            {
                if (CorrectionOffsets.Count > Eyes.Count)
                    for (int i = 0; i < CorrectionOffsets.Count - Eyes.Count; i++) CorrectionOffsets.RemoveAt(CorrectionOffsets.Count - 1);
                else
                    for (int i = 0; i < Eyes.Count - CorrectionOffsets.Count; i++) CorrectionOffsets.Add(Vector3.zero);
            }
        }


        protected virtual void OnDrawGizmosSelected()
        {
            if (EyesTarget != null && HeadReference != null)
            {
                Vector3 lookStartPositionBase;
                lookStartPositionBase = transform.position;
                lookStartPositionBase.y = HeadReference.position.y;
                lookStartPositionBase += HeadReference.TransformVector(StartLookOffset);

                Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.7f);
                Gizmos.DrawLine(lookStartPositionBase, (EyesTarget.position + targetLookPositionOffset));
            }


            if (HeadReference != null)
            {
                ComputeReferences();
                Vector3 f;

                Gizmos.color = new Color(0.3f, 0.3f, 1f, 0.7f);
                for (int i = 0; i < Eyes.Count; i++)
                {
                    if (Eyes[i] == null) continue;
                    Quaternion eyeForwarded = Eyes[i].rotation * Quaternion.Inverse(Quaternion.FromToRotation(eyeForwards[i], Vector3.forward));
                    f = eyeForwarded * Vector3.forward;

                    Gizmos.DrawRay(Eyes[i].position, f);
                    Gizmos.DrawLine(Eyes[i].position + f, Eyes[i].position + f * 0.6f + eyeForwarded * Vector3.right * 0.3f);
                    Gizmos.DrawLine(Eyes[i].position + f, Eyes[i].position + f * 0.6f + eyeForwarded * Vector3.left * 0.3f);
                }

                Vector3 middle = Vector3.Lerp(transform.position, HeadReference.position, 0.5f);
                Gizmos.DrawSphere(middle, 0.1f);
                Quaternion headForwarded = HeadReference.rotation * Quaternion.FromToRotation(headForward, Vector3.forward);
                f = headForwarded * Vector3.forward;

                Gizmos.DrawRay(middle, f);
                Gizmos.DrawLine(middle + f, middle + f * 0.65f + headForwarded * Vector3.right * 0.3f);
                Gizmos.DrawLine(middle + f, middle + f * 0.65f + headForwarded * Vector3.left * 0.3f);
            }
        }


        protected void ComputeReferences()
        {
            if (Application.isPlaying) return;

            eyeForwards = new Vector3[Eyes.Count];

            for (int i = 0; i < eyeForwards.Length; i++)
            {
                if (Eyes[i] == null) { eyeForwards[i] = Vector3.zero; continue; }
                Vector3 rootPos = Eyes[i].position;
                Vector3 targetPos = Eyes[i].position + Vector3.Scale(transform.forward, Eyes[i].transform.lossyScale);
                eyeForwards[i] = (Eyes[i].InverseTransformPoint(targetPos) - Eyes[i].InverseTransformPoint(rootPos)).normalized;
            }

            //headForward = Quaternion.FromToRotation(HeadReference.TransformDirection(Vector3.forward), transform.forward) * transform.forward;
            headForward = Quaternion.FromToRotation(HeadReference.InverseTransformDirection(transform.forward), Vector3.forward) * Vector3.forward;
        }

        #endregion


        public enum FERandomMovementType
        {
            ///<summary> Random but not too quick movement for eyes </summary>
            Default = 0,
            ///<summary> Small calm movements </summary>
            Listening = 1,
            ///<summary> Rare long random moves for eyes </summary>
            Calm = 2,
            ///<summary> Rare medium random moves for eyes </summary>
            Focused = 3,
            ///<summary> Quick and short random movement for eyes </summary>
            Nervous = 4,
            ///<summary> (Right Up) When someone is imagining something visual, he can be lying right now </summary>
            AccessingImaginedVisual = 5,
            ///<summary> (Right) When someone is imagining something related to audio, he can be lying right now </summary>
            AccessingImaginedAuditory = 6,
            ///<summary> (Right Down) When someone is recalling / imagining emotion </summary>
            AccessingFeelings = 7,
            ///<summary> (Left Up) When someone is remembering image or scene </summary>
            AccessingVisualMemory = 8,
            ///<summary> (Left) When someone is remembering something heard before </summary>
            AccessingAuditoryMemory = 9,
            ///<summary> (Left Down) When someone is talking to himself inside </summary>
            AccessingInternalSelfTalk = 10
        }
    }
}