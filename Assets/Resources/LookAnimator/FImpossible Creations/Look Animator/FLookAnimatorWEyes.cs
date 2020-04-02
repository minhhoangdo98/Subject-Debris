using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FM: Class which derives from look animator and adding simple eyes' follow animation
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Look Animator/FLook Animator With Eyes")]
    public class FLookAnimatorWEyes : FLookAnimator
    {
        [Tooltip("Target on which eyes will look, set to null if target should be the same as for head target")]
        public Transform EyesTarget;
        [Space(4f)]

        [Tooltip("Eyes transforms / bones (origin should be in center of the sphere")]
        public Transform LeftEye;
        public bool InvertLeftEye = false;
        [Tooltip("Eyes transforms / bones (origin should be in center of the sphere")]
        public Transform RightEye;
        public bool InvertRightEye = false;
        [Tooltip("Look clamping reference rotation transform, mostly parent of eye objects")]
        public Transform HeadReference;
        public Vector3 EyesOffsetRotation;

        [Tooltip("How fast eyes should follow target")]
        [Range(0f, 1f)]
        public float EyesSpeed = 0.5f;

        [Range(0f, 1f)]
        public float EyesBlend = 1f;

        [Tooltip("In what angle eyes should go back to deafult position")]
        [Range(0.0f, 180f)]
        public Vector2 EyesXRange = new Vector2(-60f, 60f);
        public Vector2 EyesYRange = new Vector2(-50f, 50f);

        [Tooltip("If your eyes don't have baked keyframes in animation this value should be enabled, otherwise eyes would go crazy")]
        public bool EyesNoKeyframes = true;

        private float EyesOutOfRangeBlend = 1f;

        // If you are using also look animator, you can simply uncomment this and one LateUpdate() line for this feature
        //public FLookAnimator UseLookAnimatorTarget = null;

        private Transform[] eyes;
        private Vector3[] eyeForwards;
        private Quaternion[] eyesInitLocalRotations;
        private Quaternion[] eyesLerpRotations;
        private Vector3 headForward;


        protected override void Start()
        {
            base.Start();

            eyes = new Transform[0];

            if (LeftEye != null || RightEye != null)
            {
                if (LeftEye != null && RightEye != null) eyes = new Transform[2] { LeftEye, RightEye }; else if (LeftEye != null) eyes = new Transform[1] { LeftEye }; else eyes = new Transform[1] { RightEye };
            }

            eyeForwards = new Vector3[eyes.Length];
            eyesInitLocalRotations = new Quaternion[eyes.Length];
            eyesLerpRotations = new Quaternion[eyes.Length];

            for (int i = 0; i < eyeForwards.Length; i++)
            {
                Vector3 rootPos = eyes[i].position + Vector3.Scale(transform.forward, eyes[i].transform.lossyScale);
                Vector3 targetPos = eyes[i].position;

                eyeForwards[i] = (eyes[i].InverseTransformPoint(rootPos) - eyes[i].InverseTransformPoint(targetPos)).normalized;
                eyesInitLocalRotations[i] = eyes[i].localRotation;
                eyesLerpRotations[i] = eyes[i].rotation;
            }

            headForward = Quaternion.FromToRotation(HeadReference.InverseTransformDirection(transform.forward), Vector3.forward) * Vector3.forward;
        }


        private void OnValidate()
        {
            if (!HeadReference)
                if (LeftEye || RightEye)
                {
                    if (LeftEye) HeadReference = LeftEye.parent; else HeadReference = RightEye.parent;
                }
        }


        float blend;
        public override void LateUpdate()
        {
            if (!initialized) return;

            base.LateUpdate();

            if (EyesNoKeyframes)
                for (int i = 0; i < eyeForwards.Length; i++)
                {
                    eyes[i].localRotation = eyesInitLocalRotations[i];
                }

            Transform eyeTarget = EyesTarget;
            if (eyeTarget == null)
            {
                if (MomentLookTransform != null) eyeTarget = MomentLookTransform; else eyeTarget = ObjectToFollow;
            }

            bool fade = false;
            if (eyeTarget == null) fade = true;
            else
            {
                if (EyesTarget == null)
                    if (LookState != FLookAnimator.EFHeadLookState.ClampedAngle && LookState != EFHeadLookState.Following) fade = true;
            }

            if (fade)
                EyesOutOfRangeBlend = Mathf.Max(0f, EyesOutOfRangeBlend - Time.deltaTime);
            else
                EyesOutOfRangeBlend = Mathf.Min(1f, EyesOutOfRangeBlend + Time.deltaTime);


            blend = EyesBlend * EyesOutOfRangeBlend;
            if (blend <= 0f) return;


            if (eyeTarget != null)
            {
                Vector3 lookStartPosition;
                if (!AnchorReferencePoint)
                {
                    if (lookStartReferenceTransform == null) if (BackBonesTransforms.Length > 0) lookStartReferenceTransform = BackBonesTransforms[0]; else lookStartReferenceTransform = LeadBone;
                    lookStartPosition = lookStartReferenceTransform.position + lookStartReferenceTransform.TransformVector(LookReferenceOffset);
                }
                else
                    lookStartPosition = anchorHelper.position + BaseTransform.TransformVector(LookReferenceOffset);


                Quaternion lookRotationQuat = Quaternion.LookRotation(eyeTarget.position - lookStartPosition);
                Vector3 lookRotation = lookRotationQuat.eulerAngles;


                #region Limitating rotation

                Vector3 headRotation = (HeadReference.rotation * Quaternion.FromToRotation(headForward, Vector3.forward)).eulerAngles;// BaseTransform.rotation.eulerAngles;

                // Vector with degrees differences to all axes
                Vector2 deltaVector = new Vector3(Mathf.DeltaAngle(lookRotation.x, headRotation.x), Mathf.DeltaAngle(lookRotation.y, headRotation.y));

                // Limit when looking up or down
                if (deltaVector.x > EyesYRange.y)
                    lookRotation.x = headRotation.x - EyesYRange.y;
                else if (deltaVector.x < EyesYRange.x)
                    lookRotation.x = headRotation.x - EyesYRange.x;

                // Limit when looking left or right
                if (deltaVector.y > -EyesXRange.x)
                    lookRotation.y = headRotation.y - EyesXRange.y;
                else if (deltaVector.y < -EyesXRange.y)
                    lookRotation.y = headRotation.y + EyesXRange.y;

                #endregion


                for (int i = 0; i < eyes.Length; i++)
                {
                    Quaternion initRot = eyes[i].rotation;
                    Quaternion newEyeRot = Quaternion.Euler(lookRotation);

                    float mul = 1f;
                    if (eyes[i] == LeftEye) { if (InvertLeftEye) mul = -1f; } else if (eyes[i] == RightEye) if (InvertRightEye) mul = -1f;
                    newEyeRot *= Quaternion.FromToRotation(eyeForwards[i], Vector3.forward * mul);
                    newEyeRot *= eyesInitLocalRotations[i];

                    eyes[i].rotation = newEyeRot;
                    eyes[i].rotation *= Quaternion.Inverse(eyesInitLocalRotations[i]);
                    if (EyesOffsetRotation != Vector3.zero) eyes[i].rotation *= Quaternion.Euler(EyesOffsetRotation);
                    newEyeRot = eyes[i].rotation;

                    eyesLerpRotations[i] = Quaternion.Slerp(eyesLerpRotations[i], newEyeRot, Time.deltaTime * Mathf.Lerp(2f, 40f, EyesSpeed));

                    eyes[i].rotation = Quaternion.Slerp(initRot, eyesLerpRotations[i], blend);
                }
            }

        }
    }
}