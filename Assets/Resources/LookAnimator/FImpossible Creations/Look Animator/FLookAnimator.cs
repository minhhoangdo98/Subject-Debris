using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FLook
{

    /// <summary>
    /// FM: Class which controlls behaviour of head bone and pre bones (define them)
    /// limit it's rotations, creating target transform to look at.
    /// It can turn on affect whole spine option (looking without weapon) or turn off look rotation (for example when aiming - head should be animated like in animation and spine etc. and rest of bones should be affected by other controller for precise aiming animations)
    /// You can use moment target to create handy events for looking at targets.
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Look Animator/FLook Animator")]
    public class FLookAnimator : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public string EditorIconPath { get { return "Look Animator/Look Animator Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        // Just define quick enums
        public enum EFAxisFixOrder { Parental, FromBased, FullManual, ZYX }
        public enum EFHeadLookState { Null, Following, OutOfMaxRotation, ClampedAngle, OutOfMaxDistance }


        #region VARIABLES Inspector

        // MAIN ANIMATION VARIABLES
        [Tooltip("Object which will be main target of look by component, it will always return to look at it when using moment targets etc.")]
        public Transform ObjectToFollow;
        //private Transform preObjectToFollow;

        [Tooltip("Head bone - head of look chain")]
        public Transform LeadBone;

        [Tooltip("Base root transform - object which moves / rotates")]
        public Transform BaseTransform;

        [Range(0.1f, 2.5f)]
        [Tooltip("Speed of look rotation animation change")]
        public float RotationSpeed = 1f;
        private bool instantRotation = false;

        [Range(0.0f, 1f)]
        [Tooltip("This variable is making rotation animation become very smooth, but it can provide errors in some extreme values - Will be upgraded in future versions to prevent of happening this errors")]
        public float UltraSmoother = 0f;

        [Tooltip("For now it's toggle, but later it might be default option - Preventing from head rotating around when using high value of ultra smoother and target object going crazy around character")]
        public bool Prevent180Error = false;

        public int BackBonesCount = 0;
        [HideInInspector] public Transform[] BackBonesTransforms;
        private List<CustomBackBone> customBackBones = new List<CustomBackBone>(); // support for custom bones

        [Tooltip("Making script start after first frame so initialization will not catch TPose initial bones rotations, which can cause some wrong offsets for rotations")]
        public bool StartAfterTPose = true;
        private int helpTPose = 1;

        [Tooltip("When you want use curve for more custom falloff or define it by simple slider")]
        public bool CurveSpread = false;

        [Tooltip("Configurable rotation weight placed over back bones - when you will use for example spine bones, here you can define how much will they rotate towards target in reference to other animated bones")]
        public AnimationCurve BackBonesFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        [Tooltip("Faloff value of how weight of animation should be spread over bones")]
        [Range(0f, 1f)]
        public float FaloffValue = 0.7f;

        [Tooltip("Max distance to target object to lost interest in it (0 = infinite range) when you have moment target after exceeding distance moment target will be forgotten")]
        public float MaximumDistance = 0f;

        [Tooltip("Minimum angle needed to trigger head follow movement")]
        [Range(0f, 90)]
        public float MinHeadLookAngle = 0;

        [Header("Limits for rotation | Horizontal: X Vertical: Y")]

        [FPD_MinMaxSlider(-180, 180)]
        public Vector2 XRotationLimits = new Vector2(-60f, 60f);

        [Range(0, 45)]
        [Tooltip("Making clamp ranges elastic, so when it starts to reach clamp value it slows like muscles needs more effort")]
        public float XElasticRange = 20f;

        [Tooltip("When head want go back to default state of looking, it will blend with default animation instead of changing values of rotation variables to go back")]
        public bool LimitHolder = true;

        [FPD_MinMaxSlider(-90, 90)]
        public Vector2 YRotationLimits = new Vector2(-50f, 50f);

        [Range(0, 45)]
        [Tooltip("Making clamp ranges elastic, so when it starts to reach clamp value it slows like muscles needs more effort")]
        public float YElasticRange = 15f;

        [Header("Look forward if this angle is exceeded", order = 1)]

        [Range(25f, 180f)]
        [Tooltip("If target is too much after transform's back we smooth rotating head back to default animation's rotation")]
        public float MaxRotationDiffrence = 140f;

        [Range(25f, 70f)]
        [Tooltip("If head have to rotate more than this value it's animation speed for rotating increases, slight touch on detailing animation")]
        public float DeltaAccelerationRange = 50f;

        [Range(40f, 180f)]
        [Tooltip("Slowing down rotation to target when it moves rapidly and head have big angle to rotate toward it")]
        public float BigAngleSmoother = 60f;

        [Range(0.1f, 2f)]
        [Tooltip("If your character moves head too fast when loosing / changing target, here you can adjust it")]
        public float ChangeTargetSpeed = 1f;

        // Bones flipped angles fixing variables
        private Vector3 LookDirection = Vector3.forward;
        private Vector3 HeadUpDirection = Vector3.up;

        [Tooltip("Defines model specific bones orientation in order to fix Quaternion.LookRotation axis usage")]
        public Vector3 ManualFromAxis = Vector3.forward;
        public Vector3 ManualToAxis = Vector3.forward;

        public Vector3 FromAuto;// { get; private set; }
        public Vector3 OffsetAuto;// { get; private set; }

        public Vector3 ReferenceLookForward;// { get; private set; }
        public Vector3 ReferenceUp;// { get; private set; }
        public Vector3 DynamicReferenceUp;// { get; private set; }


        [Tooltip("With crazy flipped axes from models done in different modelling softwares, sometimes you have to change axes order for Quaternion.LookRotation to work correctly")]
        public EFAxisFixOrder FixingPreset = EFAxisFixOrder.Parental;

        [Tooltip("Additional degrees of rotations for head look - for simple correction, sometimes you have just to rotate head in y axis by 90 degrees")]
        public Vector3 RotationOffset = new Vector3(0f, 0f, 0f);
        [Tooltip("Additional degrees of rotations for backones - for example when you have wolf and his neck is going up in comparison to keyfarmed animation")]
        public Vector3 BackBonesAddOffset = new Vector3(0f, 0f, 0f);

        [Tooltip("[ADVANCED] Axes multiplier for custom fixing flipped armature rotations")]
        public Vector3 RotCorrectionMultiplier = new Vector3(1f, 1f, 1f);

        //[Tooltip("Temporary solution to proble with 360 degrees rotation to nearest angle when lerping rotation which can occur in extremal situations, in future versions it will be done in more elegant way")]
        //public bool Prevent360Rotation = false;

        [Tooltip("View debug rays in scene window")]
        public bool DebugRays = false;

        // ADDITIONAL CONTROLL VARIABLES
        [Range(0f, 1f)]
        [Tooltip("Blend from look animator to keyframed animation")]
        public float BlendToOriginal = 0f;


        [Tooltip("If head look seems to be calculated like it is not looking from center of head but far from bottom or over it")]
        public Vector3 LookReferenceOffset;

        [Tooltip("Freezes reference start look position in x and z axes to avoid re-reaching max rotation limits when hips etc. are rotating in animation clip")]
        public bool AnchorReferencePoint = true;
        [Tooltip("In some cases you'll want to refresh anchor position during gameplay to make it more fitting to character's animation poses")]
        public bool RefreshAnchor = true;

        [Range(0f, 1f)]
        [Tooltip("Leading bone (most cases main head bone) should be rotated more hard to target, here you can controll it")]
        public float LeadBoneWeight = 1f;


        [Tooltip("Don't set hard rotations for bones, use animation rotation and add rotation offset to bones so animation's rotations are animated correctly (useful when using attack animations for example)")]
        public bool AnimateWithSource = true;

        [Tooltip("When using above action, we need to keep remembered rotations of animation clip from first frame, with monitoring we will remember root rotations from each new animation played")]
        public bool MonitorAnimator = false;

        [Header("If you don't want arms to be rotated when spine", order = 1)]
        [Header("bone is rotated by script (drag & drop here)", order = 3)]
        public List<Transform> CompensationBones;

        [Range(0f, 1f)]
        public float CompensationWeight = 0.5f;
        public bool CompensatePositions = false;
        public bool CompensatePositionsNotAnimated = false;

        [Range(0f, 3f)]
        [Tooltip("When you want create strange effects. This variable will overrotate animation intensity")]
        public float WeightsMultiplier = 1f;

        public int bonesNotAnimated = 0;

        // v1.0.7 - Bird Mode
        [Tooltip("Enabling laggy movement for head and delaying position")]
        public bool BirdMode = false;
        [Range(0f, 1f)]
        [Tooltip("Bird mode laggy movement for neck")]
        public float LagMovement = 0.85f;
        [Range(5f, 30f)]
        public float LaggySpeed = 14f;
        [Range(0.25f, 2f)]
        public float LagProgressSpeed = 1.15f;
        [Range(0.5f, 3f)]
        public float LagFrequency = 1.1f;
        [Range(0f, 1f)]
        [Tooltip("Bird mode keeping previous position until distance is reached")]
        public float DelayPosition = 0f;
        [Tooltip("How far distance to go back should have head to move (remind movement of pigeons to yourself)")]
        public float DelayMaxDistance = 0.25f;
        [Tooltip("How quick head and neck should go back to right position after reaching distance")]
        [Range(5f, 50f)]
        public float DelayGoSpeed = 15f;

        #endregion


        #region Main Variables

        /// <summary> Variable to define what's going on with look calculations </summary>
        public EFHeadLookState LookState { get; protected set; }

        public float[] RotationWeights { get; private set; }

        /// <summary> Just saved last position of look start position, can be helpful in extending component for custom purposes </summary>
        public Vector3 LastLookStartPosition { get; private set; }

        /// <summary> Generated helper transform to look at other than main focus object </summary>
        public Transform MomentLookTransform { get; private set; }

        [Tooltip("Update with waiting for fixed update clock")]
        public bool AnimatePhysics = false;
        private bool animatePhysicsWorking = false;
        private bool triggerAnimatePhysics = false;


        /// <summary>  Point from which will be calculated LookRotation to target look object  </summary>
        protected Transform lookStartReferenceTransform;

        /// <summary> Weight of animating bones in local - it can go to 0 when max ranges are exceeded </summary>
        private float localAnimationWeight = 0;

        /// <summary> Rotating bones faster when deltaY to target is bigger </summary>
        private float deltaLerpAccelerator = 1f;
        private Vector2 lastDeltaVector = Vector2.zero;
        private float holdGoBack = 0f;

        /// <summary> Making rotation animation speed look more smooth when look target is changed </summary>
        private float changeTargetSmootherWeight = 0f;
        private float changeTargetSmootherBones = 0f;

        // Remembered variables which allowing to smooth head rotations and translations
        private Quaternion headLerpRot; // Lead bone smoothed rotation
        private Quaternion headLerpRotUltra;
        private Quaternion[] lerpRotations;
        private Quaternion[] lerpRotationsUltra;

        /// <summary> When head bone isn't animated in animation se we have to refresh them every frame manually </summary>
        private Quaternion[] defautLocalRots;

        private GameObject generatedMomentTarget;

        /// <summary> Variable to detect when moment look target disappears</summary>
        private bool wasMomentLookTransform = false;

        /// If we using bones compensation, we must remember rotations dictated by animator
        private Quaternion[] compensationRotations = new Quaternion[0];
        private Vector3[] compensationPositions = new Vector3[0];
        private Vector3[] bonesTruePositions;

        /// <summary> Look-back frame variable to launch smoothing animation if we lost target </summary>
        private bool targetExists;

        /// <summary> Flag to launch smoothness variable when head is turning back to defaults when we exceed max delta angle </summary>
        private bool smoothingOutOfMaxRange = false;

        /// <summary> Variable used in calculating look animation, remembered only to use for avoid one small animation bug </summary>
        private Quaternion targetLookRotation;
        /// <summary> targetLookRotation without minLookAngle - can be used for eyes </summary>
        public Quaternion targetRotationMin { get; private set; }
        private Vector2 preLookRotation;

        private Quaternion[] newBonesRotations;

        /// <summary> Transform for helping positioning anchored position and support model rescalling </summary>
        protected Transform anchorHelper;

        private Animator animator;
        private int lastClipHash;
        protected bool initialized = false;

        #endregion


        #region Additional features and animation enchancement variables


        // Variables for using rotations as offsets function
        // We remembering rotations of bones from first frames on animation, then rotating bones with offsets with reference to this rotations
        // when we offseting animations with further frames of animation, very helpful for attack animations etc. when we also using backbones with spine etc.
        private Quaternion[] poseReferenceRotations;
        private Quaternion[] lerpedReferenceRotations;
        private Quaternion mainReferenceRotation;
        private Quaternion lerpedMainReferenceRotation;
        private Quaternion[] staticRotations;
        private Quaternion[] hardTargetRoatations;

        // v1.0.7 - Bird Mode
        private float lagTimer;
        private float lagProgress;
        private float lerpLagProgress;
        private Vector3[] initLocalPositions;
        private Vector3[] delayStartPositions;
        private Vector3[] delayCurrentPositions;
        private Vector3[] delayLerpPositions;
        private Quaternion[] lagStartRotations;
        private Quaternion[] lagTargetRotations;
        private Quaternion[] lagTargetRotationsLerp;
        private Quaternion[] preLagTargetRotations;

        #endregion


        #region > Handy methods for programmers <


        /// <summary>
        /// Tell controller to stop or start looking without changing target follow object.
        /// </summary>
        /// <param name="enableLooking"> if look == null looking will be toggled </param>
        /// <param name="transitionTime"> Transition time in seconds </param>
        public void SwitchLooking(bool? enableLooking = null, float transitionTime = 0.2f, System.Action callback = null)
        {
            bool enableAnimation = true;

            if (enableLooking == null)
            {
                if (BlendToOriginal < 0.5f) enableAnimation = false;
            }
            else if (enableLooking == false) enableAnimation = false;

            // Applying current pose to lerped variables to avoid one-frame stutter
            if (enableAnimation)
            {
                localAnimationWeight = 0.0f;
                SmoothChangeTarget(0.3f, 1f);

                newBonesRotations = new Quaternion[lerpRotations.Length + 1];
                for (int i = 0; i < lerpRotations.Length; i++)
                {
                    lerpRotations[i] = GetParentBone(i).rotation;
                    newBonesRotations[i] = lerpRotations[i];
                }

                headLerpRot = LeadBone.rotation;

                for (int i = 0; i < lerpRotationsUltra.Length; i++)
                {
                    lerpRotationsUltra[i] = GetParentBone(i).rotation;
                }

                headLerpRot = LeadBone.rotation;

                newBonesRotations[newBonesRotations.Length - 1] = headLerpRot;
            }

            StopAllCoroutines();
            StartCoroutine(SwitchLookingTransition(transitionTime, enableAnimation, callback));
        }


        /// <summary>
        /// Setting new target to follow by head bones
        /// </summary>
        public void SetLookTarget(Transform transform)
        {
            ObjectToFollow = transform;
            MomentLookTransform = null;
        }


        /// <summary>
        /// Creating custom head follow target object (position or pinned to transform) to look at
        /// </summary>
        /// <param name="parent"> Transform to which should be attached moment look target, you can leave it null if don't want attach to anything </param>
        /// <param name="position"> Target position to be followed, you can leave it null if just want to follow parent transform </param>
        /// <param name="destroyTimer"> Time in seconds, how long head should look at this moment target then return to main target, you can leave it null if you don't want to destroy this object - then you have to call ForceDestroyMomentTarget() to go back looking at main target </param>
        /// <param name="worldPosition"> If "position" variable is defining world position, set it to true </param>
        public GameObject SetMomentLookTarget(Transform parent = null, Vector3? position = null, float? destroyTimer = 3f, bool worldPosition = false)
        {
            GameObject target;

            if (destroyTimer == null)
            {
                if (!generatedMomentTarget)
                    generatedMomentTarget = new GameObject(transform.gameObject.name + "-MomentLookTarget");
                else
                    generatedMomentTarget.name = transform.gameObject.name + "-MomentLookTarget";

                target = generatedMomentTarget;
            }
            else
            {
                target = new GameObject(transform.gameObject.name + "-MomentLookTarget");
            }

            // Handling different states of provided variables
            if (parent != null)
            {
                // Creating follower at position and then attaching to target transform
                target.transform.SetParent(parent);

                if (position != null)
                {
                    if (worldPosition)
                        target.transform.position = (Vector3)position;
                    else
                        target.transform.localPosition = (Vector3)position;
                }
                else
                    target.transform.localPosition = Vector3.zero;
            }
            else // If parent isn't assigned
            {
                if (position != null)
                    target.transform.position = (Vector3)position;
            }

            MomentLookTransform = target.transform;
            wasMomentLookTransform = true;
            SmoothChangeTarget(0.1f, 0.1f, true);

            if (destroyTimer != null)
            {
                // Destroying generated object after 
                Destroy(target, (float)destroyTimer);
            }

            return target;
        }


        /// <summary>
        /// Assigning moment look transform with custom transform
        /// </summary>
        public void SetMomentLookTransform(Transform transform, float timeToLeft = 0f)
        {
            MomentLookTransform = transform;
            wasMomentLookTransform = true;
            SmoothChangeTarget(0.1f, 0.1f, true);

            if (timeToLeft > 0f)
            {
                StartCoroutine(CResetMomentLookTransform(null, timeToLeft));
            }
        }


        /// <summary>
        /// Destroying moment look transform manually
        /// </summary>
        public void ForceDestroyMomentTarget()
        {
            if (generatedMomentTarget)
            {
                Destroy(generatedMomentTarget);
            }
            else
                if (MomentLookTransform) MomentLookTransform = null;
        }


        /// <summary>
        /// Setting moment smoothness when look target is changed for more realistic speed of rotating head
        /// </summary>
        /// <param name="value"> 0f to 1f less = slower rotation </param>
        public void SmoothChangeTarget(float value, float bonesSmoother = 0.8f, bool overrideIfSlower = false)
        {
            if (BirdMode)
            {
                changeTargetSmootherWeight = 0.8f;
                changeTargetSmootherBones = 0.9f;
                return;
            }

            if (!overrideIfSlower)
            {
                changeTargetSmootherWeight = value * ChangeTargetSpeed;
                changeTargetSmootherBones = bonesSmoother * ChangeTargetSpeed;
            }
            else
            {
                float targetW = changeTargetSmootherWeight;
                if (changeTargetSmootherWeight > value) targetW = value;

                float targetB = changeTargetSmootherBones;
                if (changeTargetSmootherBones > bonesSmoother) targetB = bonesSmoother;

                changeTargetSmootherWeight = targetW * ChangeTargetSpeed;
                changeTargetSmootherBones = targetB * ChangeTargetSpeed;
            }
        }



        /// <summary>
        /// Trying to find other's transform's head position using different methods
        /// </summary>
        public Vector3 TryFindHeadPositionInTarget(Transform other)
        {
            // First, let's see if other object have look animator attached to it
            FLookAnimator look = other.GetComponent<FLookAnimator>();

            if (look)
            {
                if (look.LeadBone)
                {
                    return look.GetLookStartPosition();
                }
            }

            // Let's see if other object is using humanoid animator
            Animator animator = other.GetComponentInChildren<Animator>();
            if (animator)
            {
                if (animator.isHuman)
                {
                    if (animator.GetBoneTransform(HumanBodyBones.LeftEye)) return animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
                    else
                    if (animator.GetBoneTransform(HumanBodyBones.Head)) return animator.GetBoneTransform(HumanBodyBones.Head).position;
                }

            }

            // Trying to predict head position using other object's bounds
            Renderer rend = other.GetComponentInChildren<Renderer>();
            if (!rend) if (other.childCount > 0) rend = other.GetChild(0).GetComponentInChildren<Renderer>();

            if (rend)
            {
                Vector3 boundsHead = other.position;
                boundsHead += other.TransformVector(Vector3.up * (rend.bounds.max.y * 0.9f));
                boundsHead += other.TransformVector(Vector3.forward * (rend.bounds.max.z * 0.75f));
                return boundsHead;
            }

            return other.position;
        }

        /// <summary>
        /// Calculating start reference point of look for algorithm
        /// </summary>
        public Vector3 GetLookStartPosition()
        {
            Vector3 lookStartPosition;

            if (!AnchorReferencePoint)
            {
                if (lookStartReferenceTransform == null) if (BackBonesTransforms.Length > 0) lookStartReferenceTransform = BackBonesTransforms[0]; else lookStartReferenceTransform = LeadBone;
                lookStartPosition = lookStartReferenceTransform.position + lookStartReferenceTransform.TransformVector(LookReferenceOffset);
            }
            else
            {
                lookStartPosition = anchorHelper.position + BaseTransform.TransformVector(LookReferenceOffset);
            }

            LastLookStartPosition = lookStartPosition;

            return LastLookStartPosition;
        }


        /// <summary>
        /// Returning transform oj object on which character is looking now (moment look target or main target)
        /// </summary>
        public Transform GetCurrentTarget()
        {
            if (MomentLookTransform) return MomentLookTransform;
            return ObjectToFollow;
        }


        #region Private calculations

        /// <summary>
        /// Coroutine to turn whole look animation on or off
        /// </summary>
        private IEnumerator SwitchLookingTransition(float transitionTime, bool enableAnimation, System.Action callback = null)
        {
            float time = 0f;
            float startBlend = BlendToOriginal;

            while (time < transitionTime)
            {
                time += Time.deltaTime;
                float progress = time / transitionTime;

                if (enableAnimation)
                    BlendToOriginal = Mathf.Lerp(startBlend, 0f, progress);
                else
                    BlendToOriginal = Mathf.Lerp(startBlend, 1f, progress);

                yield return null;
            }

            if (callback != null) callback.Invoke();
        }

        #endregion

        #endregion


        #region Initialization() Methods


        private void Reset()
        {
            FindBaseTransform();
        }


        void Awake()
        {
            if (LeadBone == null)
            {
                Debug.LogError(gameObject.name + " don't have assigned lead bone!");
                return;
            }

            // Remembering main offset in position from transform to look reference transform
            anchorHelper = new GameObject(name + "-LookAnimator-AnchorHelper").transform;
            anchorHelper.SetParent(BaseTransform, true);
            anchorHelper.position = LeadBone.position;
        }


        void Init()
        {
            if (initialized) return;

            if (LeadBone == null)
            {
                Debug.LogError(gameObject.name + " don't have assigned lead bone!");
                return;
            }

            // Assign initial variables
            LookState = EFHeadLookState.Null;

            // Do initial calculations
            CalculateBonesRotationsWeights();

            // Assign needed references, you can inherit class to play with this one
            lookStartReferenceTransform = LeadBone.transform;
            headLerpRot = LeadBone.rotation;

            // Configuring back bones
            GetAdditionalBones();

            Quaternion preRot = BaseTransform.rotation;
            BaseTransform.rotation = Quaternion.identity;
            BaseTransform.rotation = preRot;

            // Remembering initial transforms for not animated bones option
            defautLocalRots = new Quaternion[BackBonesTransforms.Length + 1];
            defautLocalRots[0] = LeadBone.localRotation;

            for (int i = 1; i < defautLocalRots.Length; i++)
                defautLocalRots[i] = BackBonesTransforms[i - 1].localRotation;

            animator = GetComponentInChildren<Animator>();

            // Static rotations
            staticRotations = new Quaternion[BackBonesCount + 1];
            mainReferenceRotation = BaseTransform.rotation;

            staticRotations[0] = LeadBone.localRotation;
            if (BackBonesCount > 0)
                for (int i = 0; i < staticRotations.Length - 1; i++)
                    staticRotations[i + 1] = GetParentBone(i).localRotation;

            ComputeBonesRotationsFixVariables();

            if (CompensationBones == null) CompensationBones = new List<Transform>();

            // Remembering main offset in position from transform to look reference transform
            //anchorHelper = new GameObject(name + "-LookAnimator-AnchorHelper").transform;
            //anchorHelper.SetParent(BaseTransform, true);
            //anchorHelper.position = LeadBone.position;

            // v1.0.7 - Bird Mode
            lagTimer = 0f;
            lagProgress = 1f;
            lerpLagProgress = 1f;
            initLocalPositions = new Vector3[BackBonesCount + 1];
            delayStartPositions = new Vector3[BackBonesCount + 1];
            delayLerpPositions = new Vector3[BackBonesCount + 1];
            delayCurrentPositions = new Vector3[BackBonesCount + 1];
            lagStartRotations = new Quaternion[BackBonesCount + 1];
            lagTargetRotations = new Quaternion[BackBonesCount + 1];
            lagTargetRotationsLerp = new Quaternion[BackBonesCount + 1];
            preLagTargetRotations = new Quaternion[BackBonesCount + 1];

            initLocalPositions[0] = LeadBone.localPosition;
            delayStartPositions[0] = LeadBone.position;
            delayLerpPositions[0] = delayStartPositions[0];
            delayCurrentPositions[0] = delayLerpPositions[0];
            lagTargetRotations[0] = LeadBone.rotation;
            lagTargetRotationsLerp[0] = LeadBone.rotation;
            preLagTargetRotations[0] = LeadBone.rotation;

            if (BackBonesCount > 0)
                for (int i = 0; i < staticRotations.Length - 1; i++)
                {
                    initLocalPositions[i + 1] = GetParentBone(i).localPosition;
                    delayStartPositions[i + 1] = GetParentBone(i).position;
                    delayLerpPositions[i + 1] = delayStartPositions[i + 1];
                    delayCurrentPositions[i + 1] = delayLerpPositions[i + 1];
                    lagStartRotations[i + 1] = GetParentBone(i).rotation;
                    lagTargetRotations[i + 1] = GetParentBone(i).rotation;
                    lagTargetRotationsLerp[i + 1] = GetParentBone(i).rotation;
                    preLagTargetRotations[i + 1] = GetParentBone(i).rotation;
                }

            if (AnimateWithSource)
            {
                for (int i = 0; i < lagStartRotations.Length - 1; i++)
                {
                    lagStartRotations[i] = Quaternion.identity; //lerpRotations[i];
                    lagTargetRotationsLerp[i] = lagStartRotations[i];
                    lagTargetRotations[i] = lagStartRotations[i];
                    preLagTargetRotations[i] = lagStartRotations[i];
                }

                lagStartRotations[newBonesRotations.Length - 1] = Quaternion.identity;//headLerpRot;
                lagTargetRotationsLerp[newBonesRotations.Length - 1] = lagStartRotations[newBonesRotations.Length - 1];
                lagTargetRotations[newBonesRotations.Length - 1] = lagStartRotations[newBonesRotations.Length - 1];
                preLagTargetRotations[newBonesRotations.Length - 1] = lagStartRotations[newBonesRotations.Length - 1];
            }

            ResetBonesLerps();

            preLookRotation = CalculateLimitationAndStuff(Quaternion.LookRotation(transform.forward).eulerAngles, false);

            initialized = true;
        }


        /// <summary>
        /// Initializing head look controller
        /// </summary>
        protected virtual void Start()
        {
            if (!StartAfterTPose) Init();
        }

        /// <summary>
        /// Computing variables making Quaternion.Look universal for different skeletonal setups
        /// </summary>
        private void ComputeBonesRotationsFixVariables()
        {
            if (BaseTransform != null)
            {
                Quaternion preRot = BaseTransform.rotation;
                BaseTransform.rotation = Quaternion.identity;

                FromAuto = LeadBone.rotation * -Vector3.forward;

                float angl = Quaternion.Angle(Quaternion.identity, LeadBone.rotation);
                Quaternion rotateAxis = (LeadBone.rotation * Quaternion.Inverse(Quaternion.FromToRotation(FromAuto, LookDirection)));

                OffsetAuto = Quaternion.AngleAxis(angl, rotateAxis.eulerAngles.normalized).eulerAngles;

                BaseTransform.rotation = preRot;

                ReferenceLookForward = Quaternion.Inverse(LeadBone.parent.rotation) * BaseTransform.rotation * LookDirection.normalized;
                ReferenceUp = Quaternion.Inverse(LeadBone.parent.rotation) * BaseTransform.rotation * HeadUpDirection.normalized;
            }
            else
            {
                Debug.LogWarning("Base Transform isn't defined, so we can't use auto correction!");
            }
        }


        #endregion


        #region Update() Controll


        private void Update()
        {
            if (!initialized)
            {
                if (StartAfterTPose)
                {
                    helpTPose--;
                    if (helpTPose < -1) Init();
                }

                return;
            }
            else
            {
                if (RefreshAnchor)
                {
                    helpTPose--;
                    if (helpTPose < -6)
                    {
                        Quaternion preRot = BaseTransform.rotation;
                        BaseTransform.rotation = Quaternion.identity;
                        anchorHelper.position = LeadBone.position;
                        RefreshAnchor = false;
                        BaseTransform.rotation = preRot;
                    }
                }
                else helpTPose = -1;
            }
        }


        /// <summary>
        /// Support for 'animate physics' option inside unity's Animator
        /// </summary>
        private IEnumerator AnimatePhysicsClock()
        {
            animatePhysicsWorking = true;

            while (true)
            {
                yield return new WaitForFixedUpdate();
                triggerAnimatePhysics = true;
            }
        }


        /// <summary>
        /// Main bone rotations calculations for head look
        /// </summary>
        public virtual void LateUpdate()
        {
            if (!initialized) return;

            if (Time.deltaTime <= 0) return;

            if (AnimatePhysics)
            {
                if (!animatePhysicsWorking) StartCoroutine(AnimatePhysicsClock());
                if (!triggerAnimatePhysics) return; else triggerAnimatePhysics = false;
            }

            if (RotationSpeed >= 2.5f && UltraSmoother <= 0f) instantRotation = true; else instantRotation = false;

            // Remembering first frame bones rotations for offset mode
            if (poseReferenceRotations == null)
            {
                RememberCurrentBonesRotations(true);
            }

            if (localAnimationWeight < 0.01f)
            {
                ResetBonesLerps();

                // If we blend animation to 0 we don't do anything
                if (BlendToOriginal >= 1f) return;
            }

            // Optional options calculations
            BasicCalculations();

            if (!GetCurrentTarget())
            {
                SmoothChangeTarget(0.2f, 0.3f, true);
            }

            // Handling max distance feature
            if (MaximumDistance > 0f)
            {
                if (MomentLookTransform)
                {
                    float dist = Vector3.Distance(LastLookStartPosition, MomentLookTransform.position);
                    if (dist > MaximumDistance) MomentLookTransform = null;
                }
                else
                {
                    Transform target = GetCurrentTarget();
                    if (target)
                        if (LookState != EFHeadLookState.OutOfMaxDistance)
                        {
                            float dist = Vector3.Distance(LastLookStartPosition, target.position);
                            if (dist > MaximumDistance)
                            {
                                if (MomentLookTransform) { ForceDestroyMomentTarget(); return; }
                                SmoothChangeTarget(0.3f, 0.3f);
                                LookState = EFHeadLookState.OutOfMaxDistance;
                            }
                        }
                        else
                        {
                            float dist = Vector3.Distance(LastLookStartPosition, target.position);
                            if (dist <= MaximumDistance)
                            {
                                LookState = EFHeadLookState.Null;
                            }
                        }
                }
            }

            if (LookState != EFHeadLookState.OutOfMaxDistance)
            {
                // When you change target to null by using just variable
                if (!targetExists && !GetCurrentTarget() && LookState != EFHeadLookState.Null)
                {
                    SmoothChangeTarget(0.1f, 0.1f);
                }

                // Checking if we lost follow target
                if (targetExists && !GetCurrentTarget())
                {
                    targetExists = false;
                    LookState = EFHeadLookState.Null;

                    SmoothChangeTarget(0.35f, changeTargetSmootherBones);
                }

                // If any target not existing, we returning to default animation
                // Update() method taking care of resetting this component's animation blend to 0
                if (!GetCurrentTarget())
                {
                    if (LookState != EFHeadLookState.Null)
                    {
                        SmoothChangeTarget(0.75f, 0.2f, true);
                        LookState = EFHeadLookState.Null;
                    }
                }
                else
                {
                    if (LookState == EFHeadLookState.Null)
                    {
                        SmoothChangeTarget(0.4f, 0.3f, true);
                    }
                }
            }

            // Changing switch target smoothness speed factor
            changeTargetSmootherWeight = Mathf.Min(1f, changeTargetSmootherWeight + Time.deltaTime * 0.6f);
            changeTargetSmootherBones = Mathf.Min(1f, changeTargetSmootherBones + Time.deltaTime * 0.6f);
            EFHeadLookState previousState = LookState;

            // Main look rotation calculations
            CalculateLookAnimation();

            // Changing lerp value for smoothing animations - this lerp value is used in CalculateLookAnimation() method
            if (LookState == EFHeadLookState.ClampedAngle || LookState == EFHeadLookState.Following)
            {
                if (previousState == EFHeadLookState.OutOfMaxRotation) SmoothChangeTarget(0.1f, 0.3f, true);

                float targetWeight = 1.05f * (1f - BlendToOriginal);
                if (!instantRotation)
                    localAnimationWeight = Mathf.Lerp(localAnimationWeight, targetWeight, Time.deltaTime * 25f * RotationSpeed * changeTargetSmootherWeight * deltaLerpAccelerator);
                else
                    localAnimationWeight = targetWeight;

            }
            else // Smoothly rotating head back to default bone rotation
            {
                if (!instantRotation)
                    localAnimationWeight = Mathf.Max(0f, Mathf.Lerp(localAnimationWeight, -0.02f, Time.deltaTime * 15f * RotationSpeed * changeTargetSmootherWeight));
                else
                    localAnimationWeight = 0.0f;
            }


            // Applying final computed rotations and others 
            ChangeBonesRotations();
            //preObjectToFollow = ObjectToFollow;

            if (BirdMode) CalculateBirdMode();
        }


        #endregion


        #region Main Look Algorithm

        /// <summary>
        /// Optional and basic calculations before main animation computing code
        /// </summary>
        private void BasicCalculations()
        {
            if (CompensationBones.Count > 0)
            {
                if (compensationPositions.Length != CompensationBones.Count)
                {
                    compensationRotations = new Quaternion[CompensationBones.Count];
                    compensationPositions = new Vector3[CompensationBones.Count];
                }

                for (int i = 0; i < CompensationBones.Count; i++)
                {
                    if (CompensationBones[i] != null)
                    {
                        compensationRotations[i] = CompensationBones[i].rotation;
                        compensationPositions[i] = CompensationBones[i].position;
                    }
                }
            }

            /// Monitoring animator for capturing bones rotation states
            if (AnimateWithSource)
            {
                if (animator)
                    if (MonitorAnimator)
                    {
                        if (!animator.IsInTransition(0))
                        {
                            int currentClipHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

                            // When animation clip changed
                            if (currentClipHash != lastClipHash)
                            {
                                RememberCurrentBonesRotations();
                            }

                            lastClipHash = currentClipHash;
                        }

                        for (int i = 0; i < lerpedReferenceRotations.Length; i++)
                            lerpedReferenceRotations[i] = Quaternion.Slerp(lerpedReferenceRotations[i], poseReferenceRotations[i], Time.deltaTime * 7f);

                        lerpedMainReferenceRotation = Quaternion.Slerp(lerpedMainReferenceRotation, mainReferenceRotation, Time.deltaTime * 7f);
                    }
            }


            /// If some bones aren't animated from animator (very small cases)
            if (bonesNotAnimated > 0)
            {
                LeadBone.localRotation = defautLocalRots[0];

                for (int i = 1; i < defautLocalRots.Length; i++)
                    BackBonesTransforms[i - 1].localRotation = defautLocalRots[i];
            }
        }

        /// <summary>
        /// Applying calculated bones rotations
        /// </summary>
        private void ChangeBonesRotations()
        {
            for (int i = 0; i < BackBonesTransforms.Length; i++) GetParentBone(i).rotation = newBonesRotations[i];
            LeadBone.rotation = newBonesRotations[newBonesRotations.Length - 1];

            #region Optional options calculations finishing operations

            // Applying compensation weights
            if (compensationRotations.Length > 0f)
            {
                float posBlend = 0f;
                if (CompensatePositions)
                {
                    posBlend = CompensationWeight;
                }

                for (int i = 0; i < CompensationBones.Count; i++)
                {
                    if (CompensationBones[i] != null)
                    {
                        CompensationBones[i].rotation = Quaternion.Slerp(CompensationBones[i].rotation, compensationRotations[i], CompensationWeight);
                        CompensationBones[i].position = Vector3.Lerp(CompensationBones[i].position, compensationPositions[i], posBlend);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Calculating target look position for different conditions
        /// </summary>
        /// <returns> Target look position, returns null if there is nowhere to look at </returns>
        private Vector3? GetTargetPosition()
        {
            Vector3? targetLookPosition = null;

            // If we have assigned timed look transform
            if (MomentLookTransform)
            {
                targetLookPosition = MomentLookTransform.position;
            }

            // Setting main follow position
            if (!MomentLookTransform)
            {
                if (wasMomentLookTransform)
                {
                    SmoothChangeTarget(0.1f, 0.1f, true);
                    wasMomentLookTransform = false;
                }

                if (ObjectToFollow) targetLookPosition = ObjectToFollow.position;
            }

            return targetLookPosition;
        }

        /// <summary>
        /// Calculating look rotation to target object from sight reference position with clamping and other operations and setting lookState etc.
        /// </summary>
        private Vector3 CalculateTargetLookRotation(Vector3 lookPos)
        {
            Vector3 lookRotation = Vector3.zero;
            Vector3 targetLookPosition = lookPos;

            // From what point we will look at target position - offseting regerence transform's position with lookReferenceOffset in order to lookSourceTransform's orientation
            Vector3 lookStartPosition = GetLookStartPosition();

            if (FixingPreset != EFAxisFixOrder.Parental)
            {
                lookRotation = CalculateLimitationAndStuff(Quaternion.LookRotation(targetLookPosition - lookStartPosition).eulerAngles, false);
            }
            else
            {
                lookRotation = LookRotationParental(lookStartPosition, targetLookPosition).eulerAngles;
            }


            return lookRotation;
        }

        /// <summary>
        /// Main head look rotation calculations using above methods GetTargetPosition() and CalculateTargetLookRotation()
        /// </summary>
        private void CalculateLookAnimation()
        {
            Vector3? lookPos = GetTargetPosition();
            Vector3 targetLookPosition = Vector3.zero;

            // Will be calculated in CalculateTargetLookRotation()
            Vector3 lookRotation = Vector3.zero;
            Quaternion backbonesTargetLookRotationOffset = Quaternion.identity;

            // We will use this variable during calculations for clamping how much head can rotate (for example to not flip head in 180 degrees, but limit rotation to configured angle)

            if (holdGoBack <= 0f)
            {
                // If we have target position to follow
                if (lookPos != null)
                {
                    targetLookPosition = (Vector3)lookPos;
                    lookRotation = CalculateTargetLookRotation(targetLookPosition);
                }
                //else lookState = EFHeadLookState.Null;
            }
            else
            {
                holdGoBack -= Time.deltaTime;
                if (lookPos != null) lookRotation = CalculateTargetLookRotation(LeadBone.transform.position + BaseTransform.forward * 5f);
            }

            // When we should rotate bones to calculated look rotation - wee need to do them even we go with localAnimationBlend to 0, without it there would be small bug with head going back from last calculated rotation
            // Correcting target rotation if it's needed

            // Remembering previous look rotation to avoid returning from out of max rotation range small animation bug
            //if (lookState != EFHeadLookState.OutOfMaxRotation)
            if (LookState == EFHeadLookState.ClampedAngle || LookState == EFHeadLookState.Following)
            {
                lookRotation = ConvertFlippedAxes(lookRotation);
                targetLookRotation = Quaternion.Euler(lookRotation);
                backbonesTargetLookRotationOffset = Quaternion.Euler(BackBonesAddOffset);
            }

            // Transitioning target bones rotations to new values and assigning them to bones final pose
            if (!AnimateWithSource)
            {
                Quaternion weightedRotation;

                if (newBonesRotations == null || newBonesRotations.Length != BackBonesTransforms.Length + 1)
                    newBonesRotations = new Quaternion[BackBonesTransforms.Length + 1];

                if (UltraSmoother < 1f)
                {
                    // Setting target rotations for bones
                    for (int i = 0; i < BackBonesTransforms.Length; i++)
                    {
                        Quaternion currentBoneRotation = GetParentBone(i).rotation;
                        weightedRotation = LinearInterpolateRotation(Quaternion.identity, Quaternion.Inverse(currentBoneRotation) * targetLookRotation * backbonesTargetLookRotationOffset, RotationWeights[i + 1] * WeightsMultiplier);
                        hardTargetRoatations[i] = currentBoneRotation * weightedRotation;

                        // Applying rotation with weight value
                        if (!instantRotation)
                            lerpRotations[i] = LinearInterpolateRotation(lerpRotations[i], weightedRotation, Time.deltaTime * 20f * RotationSpeed * changeTargetSmootherBones);
                        else
                            lerpRotations[i] = weightedRotation;

                        newBonesRotations[i] = LinearInterpolateRotation(currentBoneRotation, currentBoneRotation * lerpRotations[i], localAnimationWeight);

                        if (BirdMode) newBonesRotations[i] = LinearInterpolateRotation(currentBoneRotation, Quaternion.Slerp(newBonesRotations[i], lagTargetRotationsLerp[i], LagMovement), localAnimationWeight);
                    }

                    // Setting leading head bone rotation without weighting for precision to be sure that head is truly looking at target
                    weightedRotation = LinearInterpolateRotation(Quaternion.identity, Quaternion.Inverse(LeadBone.rotation) * targetLookRotation, LeadBoneWeight);
                    hardTargetRoatations[hardTargetRoatations.Length - 1] = LeadBone.rotation * weightedRotation;

                    float lerpM = 1f;
                    //if (holdGoBack > 0f) lerpM = 0.8f;

                    if (Prevent180Error)
                    {
                        if (FromAuto.z > 0.9f)
                        {
                            Vector3 from = (headLerpRot * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;
                            Vector3 to = ((LeadBone.rotation * weightedRotation) * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;

                            if (!instantRotation)
                            {
                                float value = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                                headLerpRot = Quaternion.Euler(Mathf.LerpAngle(from.x, to.x, value), Mathf.Lerp(from.y, to.y, value), Mathf.LerpAngle(from.z, to.z, value)) * BaseTransform.rotation;
                            }
                            else
                                headLerpRot = Quaternion.Euler(to) * BaseTransform.rotation;
                        }
                        else
                        {
                            Vector3 off = new Vector3(0f, BaseTransform.eulerAngles.y, 0f);
                            off.y = off.y - (90) - off.y * 2;

                            Vector3 s = headLerpRot.eulerAngles + off;
                            Vector3 e = (LeadBone.rotation * weightedRotation).eulerAngles + off;

                            if (!instantRotation)
                            {
                                float lerp = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                                Vector3 lim = new Vector3(Mathf.LerpAngle(s.x, e.x, lerp), Mathf.Lerp(AnglePositive(s.y), AnglePositive(e.y), lerp), Mathf.LerpAngle(s.z, e.z, lerp));
                                headLerpRot = Quaternion.Euler(lim - off);
                            }
                            else
                                headLerpRot = Quaternion.Euler(e - off);
                        }
                    }
                    else
                    {
                        // Setting leading head bone rotation without weighting for precision to be sure that head is truly looking at target
                        if (!instantRotation)
                            headLerpRot = LinearInterpolateRotation(headLerpRot, LeadBone.rotation * weightedRotation, Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones * lerpM);
                        else
                            headLerpRot = LeadBone.rotation * weightedRotation;
                    }

                    newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, headLerpRot, localAnimationWeight);

                    if (BirdMode) newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, Quaternion.Slerp(newBonesRotations[newBonesRotations.Length - 1], lagTargetRotationsLerp[newBonesRotations.Length - 1], LagMovement), localAnimationWeight);
                }

                if (UltraSmoother > 0f)
                {
                    // Setting target rotations for bones
                    for (int i = 0; i < BackBonesTransforms.Length; i++)
                    {
                        Quaternion currentBoneRotation = GetParentBone(i).rotation;
                        weightedRotation = LinearInterpolateRotation(currentBoneRotation, targetLookRotation * backbonesTargetLookRotationOffset, RotationWeights[i + 1] * WeightsMultiplier);
                        hardTargetRoatations[i] = currentBoneRotation * weightedRotation;

                        // Applying rotation with weight value
                        lerpRotationsUltra[i] = LinearInterpolateRotation(lerpRotationsUltra[i], weightedRotation, Time.deltaTime * 20f * RotationSpeed * changeTargetSmootherBones);

                        if (UltraSmoother < 1f)
                            newBonesRotations[i] = LinearInterpolateRotation(newBonesRotations[i], LinearInterpolateRotation(currentBoneRotation, lerpRotationsUltra[i], localAnimationWeight), UltraSmoother); // Getting target rotation
                        else
                        {
                            newBonesRotations[i] = LinearInterpolateRotation(newBonesRotations[i], lerpRotationsUltra[i], UltraSmoother);
                        }
                    }

                    weightedRotation = LinearInterpolateRotation(LeadBone.rotation, targetLookRotation, LeadBoneWeight);
                    hardTargetRoatations[hardTargetRoatations.Length - 1] = LeadBone.rotation * weightedRotation;

                    float lerpM = 1f;
                    //if (holdGoBack > 0f) lerpM = 0.8f;

                    if (Prevent180Error)
                    {
                        if (FromAuto.z > 0.9f)
                        {
                            Vector3 from = (headLerpRotUltra * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;
                            Vector3 to = (weightedRotation * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;
                            float value = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                            headLerpRotUltra = Quaternion.Euler(Mathf.LerpAngle(from.x, to.x, value), Mathf.Lerp(from.y, to.y, value), Mathf.LerpAngle(from.z, to.z, value)) * BaseTransform.rotation;
                        }
                        else
                        {
                            float lerp = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                            Vector3 off = new Vector3(0f, BaseTransform.eulerAngles.y, 0f);
                            off.y = off.y - (90) - off.y * 2;

                            Vector3 s = headLerpRotUltra.eulerAngles + off;
                            Vector3 e = weightedRotation.eulerAngles + off;
                            Vector3 lim = new Vector3(Mathf.LerpAngle(s.x, e.x, lerp), Mathf.Lerp(AnglePositive(s.y), AnglePositive(e.y), lerp), Mathf.LerpAngle(s.z, e.z, lerp));
                            headLerpRotUltra = Quaternion.Euler(lim - off);
                        }
                    }
                    else
                    {
                        // Setting leading head bone rotation without weighting for precision to be sure that head is truly looking at target
                        headLerpRotUltra = Quaternion.Lerp(headLerpRotUltra, weightedRotation, Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones * lerpM);
                    }


                    if (UltraSmoother < 1f)
                    {
                        Quaternion ultraHeadLerpedRotation = LinearInterpolateRotation(LeadBone.rotation, headLerpRotUltra, localAnimationWeight);
                        newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(newBonesRotations[newBonesRotations.Length - 1], ultraHeadLerpedRotation, UltraSmoother * 0.5f);
                    }
                    else
                    {
                        newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, headLerpRotUltra, localAnimationWeight);
                    }
                }
            }
            else // When we won't replace bones rotations but just offset them to right state - it will work with model keyframed animation
            {
                // Variables used to rotate backbones and leading bone
                Quaternion diffRotation;
                Quaternion weightedRotation;
                Quaternion referenceRotation;
                Quaternion newOffsetedRotation;

                // Remembering rotations and assigning later to prevent unwanted hierarchy rotation changes
                if (newBonesRotations.Length != BackBonesTransforms.Length + 1)
                {
                    newBonesRotations = new Quaternion[BackBonesTransforms.Length + 1];
                }

                // Variable used to transform remembered rotations little like localRotations but ignoring all hierarchy chain-rotation changes
                Quaternion diffOnMain = BaseTransform.rotation * Quaternion.Inverse(lerpedMainReferenceRotation);
                if (UltraSmoother < 1f)
                {
                    // Calculating rotations with weights etc.
                    for (int i = BackBonesTransforms.Length - 1; i >= 0; i--)
                    {
                        Quaternion currentBoneRotation = GetParentBone(i).rotation;
                        referenceRotation = diffOnMain * lerpedReferenceRotations[i + 1]; // Transponing reference pose world location with main transform rotation

                        // Getting weighted look rotation for bone
                        weightedRotation = LinearInterpolateRotation(Quaternion.identity, targetLookRotation * backbonesTargetLookRotationOffset * Quaternion.Inverse(referenceRotation), RotationWeights[i + 1] * WeightsMultiplier);
                        hardTargetRoatations[i] = weightedRotation;

                        if (!instantRotation)
                            lerpRotations[i] = LinearInterpolateRotation(lerpRotations[i], weightedRotation, Time.deltaTime * 20f * RotationSpeed * changeTargetSmootherBones);
                        else
                            lerpRotations[i] = weightedRotation;

                        newBonesRotations[i] = LinearInterpolateRotation(currentBoneRotation, lerpRotations[i] * currentBoneRotation, localAnimationWeight); // Getting target rotation

                        if (BirdMode) newBonesRotations[i] = LinearInterpolateRotation(currentBoneRotation, Quaternion.Slerp(newBonesRotations[i], lagTargetRotationsLerp[i] * currentBoneRotation, LagMovement), localAnimationWeight);
                    }


                    float lerpM = 1f;
                    //if (holdGoBack > 0f) lerpM = 0.8f;

                    // Almost the same thing like in upper for(;;) loop
                    referenceRotation = diffOnMain * lerpedReferenceRotations[0]; // Transponing reference pose world location with main transform rotation
                    weightedRotation = LinearInterpolateRotation(Quaternion.identity, targetLookRotation * Quaternion.Inverse(referenceRotation), LeadBoneWeight);
                    hardTargetRoatations[hardTargetRoatations.Length - 1] = weightedRotation;

                    if (Prevent180Error)
                    {
                        if (FromAuto.z > 0.9f)
                        {
                            Vector3 from = headLerpRot.eulerAngles;
                            Vector3 to = weightedRotation.eulerAngles;
                            //float value = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                            //Vector3 lim = new Vector3(Mathf.LerpAngle(from.x, to.x, value), Mathf.Lerp(FLogicMethods.WrapAngle(from.y), FLogicMethods.WrapAngle(to.y), value), Mathf.LerpAngle(from.z, to.z, value));
                            //headLerpRot = Quaternion.Euler(lim);

                            if (!instantRotation)
                            {
                                float value = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                                Vector3 lim = new Vector3(Mathf.LerpAngle(from.x, to.x, value), Mathf.Lerp(FLogicMethods.WrapAngle(from.y), FLogicMethods.WrapAngle(to.y), value), Mathf.LerpAngle(from.z, to.z, value));
                                headLerpRot = Quaternion.Euler(lim);
                            }
                            else
                                headLerpRot = Quaternion.Euler(to);
                        }
                        else
                        {
                            Vector3 off = new Vector3(0f, 180f, 0f);
                            Vector3 s = headLerpRot.eulerAngles + off;

                            if (!instantRotation)
                            {
                                float lerp = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                                Vector3 e = weightedRotation.eulerAngles + off;
                                Vector3 lim = new Vector3(Mathf.LerpAngle(s.x, e.x, lerp), Mathf.Lerp(AnglePositive(s.y), AnglePositive(e.y), lerp), Mathf.LerpAngle(s.z, e.z, lerp));
                                headLerpRot = Quaternion.Euler(lim - off);
                            }
                            else
                                headLerpRot = Quaternion.Euler(s - off);
                        }
                    }
                    else
                    {
                        if (!instantRotation)
                            headLerpRot = LinearInterpolateRotation(headLerpRot, weightedRotation, Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones * lerpM);
                        else
                            headLerpRot = weightedRotation;
                    }

                    newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, headLerpRot * LeadBone.rotation, localAnimationWeight);

                    if (BirdMode) newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, Quaternion.Slerp(newBonesRotations[newBonesRotations.Length - 1], lagTargetRotationsLerp[newBonesRotations.Length - 1] * LeadBone.rotation, LagMovement), localAnimationWeight);
                }



                if (UltraSmoother > 0f)
                {
                    // Calculating rotations with weights etc.
                    for (int i = BackBonesTransforms.Length - 1; i >= 0; i--)
                    {
                        Quaternion currentBoneRotation = GetParentBone(i).rotation;
                        referenceRotation = diffOnMain * lerpedReferenceRotations[i + 1]; // Transponing reference pose world location with main transform rotation

                        // Getting weighted look rotation for bone
                        weightedRotation = LinearInterpolateRotation(referenceRotation, targetLookRotation * backbonesTargetLookRotationOffset, RotationWeights[i + 1] * WeightsMultiplier);
                        diffRotation = weightedRotation * Quaternion.Inverse(referenceRotation); // getting diff rotation to reference rotation (after setting localRotation in upper code)
                        newOffsetedRotation = diffRotation * currentBoneRotation; // Only this order of multiply works
                        hardTargetRoatations[i] = newOffsetedRotation;

                        // Smooth transitioning target rotation
                        lerpRotationsUltra[i] = LinearInterpolateRotation(lerpRotationsUltra[i], newOffsetedRotation, Time.deltaTime * 20f * RotationSpeed * changeTargetSmootherBones);

                        if (UltraSmoother < 1f)
                            newBonesRotations[i] = LinearInterpolateRotation(newBonesRotations[i], LinearInterpolateRotation(currentBoneRotation, lerpRotationsUltra[i], localAnimationWeight), UltraSmoother); // Getting target rotation
                        else
                        {
                            newBonesRotations[i] = LinearInterpolateRotation(currentBoneRotation, lerpRotationsUltra[i], UltraSmoother); // Getting target rotation
                        }
                    }

                    // Almost the same thing like in upper for(;;) loop
                    referenceRotation = diffOnMain * lerpedReferenceRotations[0]; // Transponing reference pose world location with main transform rotation
                    weightedRotation = LinearInterpolateRotation(referenceRotation, targetLookRotation, LeadBoneWeight);
                    diffRotation = weightedRotation * Quaternion.Inverse(referenceRotation);

                    newOffsetedRotation = diffRotation * LeadBone.rotation;
                    hardTargetRoatations[hardTargetRoatations.Length - 1] = newOffsetedRotation;

                    float lerpM = 1f;
                    //if (holdGoBack > 0f) lerpM = 0.8f;

                    if (Prevent180Error)
                    {
                        if (FromAuto.z > 0.9f)
                        {
                            Vector3 from = (headLerpRotUltra * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;
                            Vector3 to = (newOffsetedRotation * Quaternion.Inverse(BaseTransform.rotation)).eulerAngles;
                            float value = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                            headLerpRotUltra = Quaternion.Euler(Mathf.LerpAngle(from.x, to.x, value), Mathf.Lerp(from.y, to.y, value), Mathf.LerpAngle(from.z, to.z, value)) * BaseTransform.rotation;
                        }
                        else
                        {
                            float lerp = Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones;
                            Vector3 off = new Vector3(0f, BaseTransform.eulerAngles.y, 0f);
                            off.y = off.y - (90) - off.y * 2;

                            Vector3 s = headLerpRotUltra.eulerAngles + off;
                            Vector3 e = newOffsetedRotation.eulerAngles + off;
                            Vector3 lim = new Vector3(Mathf.LerpAngle(s.x, e.x, lerp), Mathf.Lerp(AnglePositive(s.y), AnglePositive(e.y), lerp), Mathf.LerpAngle(s.z, e.z, lerp));
                            headLerpRotUltra = Quaternion.Euler(lim - off);
                        }
                    }
                    else
                        headLerpRotUltra = LinearInterpolateRotation(headLerpRotUltra, newOffsetedRotation, Time.deltaTime * 10f * RotationSpeed * changeTargetSmootherBones * lerpM);


                    if (UltraSmoother < 1f)
                    {
                        Quaternion ultraHeadLerpedRotation = LinearInterpolateRotation(LeadBone.rotation, headLerpRotUltra, localAnimationWeight);
                        newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(newBonesRotations[newBonesRotations.Length - 1], ultraHeadLerpedRotation, UltraSmoother * 0.5f);
                    }
                    else
                    {
                        newBonesRotations[newBonesRotations.Length - 1] = LinearInterpolateRotation(LeadBone.rotation, headLerpRotUltra, localAnimationWeight);
                    }
                }
            }
        }


        // Making y position of look target position a little offseted, when we look at something we don't look at it totally directly, we also using eyes direction to see things fully - this thing makes animation little more realistic
        private Quaternion LookRotationParental(Vector3 startLookPosition, Vector3 targetPosition)
        {
            if (!AnimateWithSource)
            {
                LeadBone.localRotation = staticRotations[0];
                for (int i = 0; i < BackBonesTransforms.Length; i++)
                {
                    BackBonesTransforms[i].localRotation = staticRotations[i + 1];
                }
            }

            // Target look rotation equivalent for LeadBone's parent
            Vector3 lookDirectionParent = Quaternion.Inverse(LeadBone.parent.rotation) * (targetPosition - startLookPosition).normalized;

            // Getting angle offset in y axis - horizontal rotation
            float yAngle = AngleAroundAxis(ReferenceLookForward, lookDirectionParent, ReferenceUp);

            Vector3 targetRight = Vector3.Cross(ReferenceUp, lookDirectionParent);
            Vector3 horizontalPlaneTarget = lookDirectionParent - Vector3.Project(lookDirectionParent, ReferenceUp);

            float xAngle = AngleAroundAxis(horizontalPlaneTarget, lookDirectionParent, targetRight);

            Vector3 limited = CalculateLimitationAndStuff(new Vector2(xAngle, yAngle), true);
            xAngle = limited.x;
            yAngle = limited.y;

            Vector3 referenceRightDir = Vector3.Cross(ReferenceUp, ReferenceLookForward);

            // With calculated angles we can get rotation by rotating around desired axes
            lookDirectionParent = Quaternion.AngleAxis(yAngle, ReferenceUp) * Quaternion.AngleAxis(xAngle, referenceRightDir) * ReferenceLookForward;

            // Making look and up direction perpendicular
            Vector3 upDirGoal = ReferenceUp;
            Vector3.OrthoNormalize(ref lookDirectionParent, ref upDirGoal);

            // Look and up directions in lead's parent space
            Vector3 lookDir = lookDirectionParent;
            DynamicReferenceUp = upDirGoal;
            Vector3.OrthoNormalize(ref lookDir, ref DynamicReferenceUp);

            // Finally getting look rotation
            Quaternion lookRot = LeadBone.parent.rotation * Quaternion.LookRotation(lookDir, DynamicReferenceUp);
            lookRot *= Quaternion.Inverse(LeadBone.parent.rotation * Quaternion.LookRotation(ReferenceLookForward, ReferenceUp));

            lookRot = LeadBone.parent.rotation * Quaternion.LookRotation(lookDir, DynamicReferenceUp);
            lookRot *= Quaternion.Inverse(LeadBone.parent.rotation * Quaternion.LookRotation(ReferenceLookForward, ReferenceUp));

            targetRotationMin = lookRot * LeadBone.rotation;

            if (MinHeadLookAngle > 0)
            {
                float angle = Quaternion.Angle(targetLookRotation, targetRotationMin);

                if (angle < MinHeadLookAngle) return targetLookRotation;

                if (!BirdMode)
                    if (angle != 0) targetRotationMin = Quaternion.Lerp(targetLookRotation, targetRotationMin, (angle - MinHeadLookAngle) / angle);
            }

            return targetRotationMin;
        }


        #region Bird Mode Stuff


        /// <summary>
        /// V 1.0.7 - Bird Mode
        /// Handling eye lag simulation
        /// </summary>
        private void CalculateBirdMode()
        {
            UltraSmoother = 0f;

            float angle = Quaternion.Angle(lagTargetRotationsLerp[newBonesRotations.Length - 1], hardTargetRoatations[hardTargetRoatations.Length - 1]);
            float rusher;

            if (angle < 60f)
            {
                rusher = Mathf.Lerp(1f, 1.5f, Mathf.InverseLerp(30f, 50f, angle));
            }
            else
            {
                rusher = Mathf.Lerp(1.5f, 3f, Mathf.InverseLerp(50f, 110f, angle));
            }

            lagTimer -= Time.deltaTime * rusher;


            lerpLagProgress = Mathf.Lerp(lerpLagProgress, lagProgress, Time.deltaTime * LaggySpeed);

            // Calculating look rotation lag animation
            if (lagProgress > 0)
            {
                if (lagTimer < 0f) lagProgress -= Random.Range(0.4f, 0.8f) * Time.deltaTime * 50f * LagProgressSpeed;
            }
            else
            {
                if (lagProgress <= 0) if (lagTimer < 0f) ResetBirdTargeting();
            }

            if (lagTimer < 0f) lagTimer = Random.Range(0.15f, 0.35f) / LagFrequency;


            for (int i = 0; i < lagTargetRotationsLerp.Length - 1; i++)
                lagTargetRotationsLerp[i] = Quaternion.Slerp(lagTargetRotationsLerp[i], Quaternion.Slerp(lagStartRotations[i], lagTargetRotations[i], 1f - lagProgress), Time.deltaTime * LaggySpeed);

            lagTargetRotationsLerp[newBonesRotations.Length - 1] = Quaternion.Slerp(lagTargetRotationsLerp[newBonesRotations.Length - 1], Quaternion.Slerp(lagStartRotations[newBonesRotations.Length - 1], lagTargetRotations[newBonesRotations.Length - 1], 1f - lagProgress), Time.deltaTime * LaggySpeed);



            // Head neck positions holding animation
            if (DelayPosition > 0f)
            {
                // Resetting default positions in animation frame
                LeadBone.localPosition = initLocalPositions[0];
                for (int i = 1; i < initLocalPositions.Length; i++) BackBonesTransforms[i - 1].transform.localPosition = initLocalPositions[i];

                // Detecting movement offset to trigger new neck movement position
                float xzDist = Vector3.Distance(Vector3.Scale(delayStartPositions[0], new Vector3(1, 0, 1)), Vector3.Scale(LeadBone.position, new Vector3(1, 0, 1)));
                float yDist = Mathf.Abs(delayStartPositions[0].y - LeadBone.position.y);

                if (xzDist > DelayMaxDistance || yDist > DelayMaxDistance / 1.65f)
                {
                    delayStartPositions[0] = LeadBone.position;
                    for (int i = 1; i < initLocalPositions.Length; i++) delayStartPositions[i] = BackBonesTransforms[i - 1].position;
                }

                for (int i = 0; i < delayLerpPositions.Length; i++)
                {
                    delayLerpPositions[i] = Vector3.Lerp(delayLerpPositions[i], delayStartPositions[i], Time.deltaTime * DelayGoSpeed);
                }

                if (bonesTruePositions == null || bonesTruePositions.Length != BackBonesTransforms.Length + 1)
                    bonesTruePositions = new Vector3[BackBonesTransforms.Length + 1];

                for (int i = 1; i < bonesTruePositions.Length; i++) bonesTruePositions[i - 1] = BackBonesTransforms[i - 1].position;
                bonesTruePositions[0] = LeadBone.position;

                for (int i = 0; i < BackBonesTransforms.Length; i++)
                {
                    BackBonesTransforms[i].position = Vector3.Lerp(bonesTruePositions[i], delayLerpPositions[i + 1], RotationWeights[i] * DelayPosition * localAnimationWeight);
                    delayCurrentPositions[i + 1] = BackBonesTransforms[i].position;
                }

                LeadBone.position = Vector3.Lerp(bonesTruePositions[0], delayLerpPositions[0], DelayPosition * localAnimationWeight);
                delayCurrentPositions[0] = LeadBone.position;
            }
        }


        private void ResetBirdTargeting()
        {
            lagProgress = 1f;
            lagTimer = -0.1f;
            lerpLagProgress = 1f;

            if (!AnimateWithSource)
            {
                for (int i = 0; i < lagStartRotations.Length - 1; i++)
                    lagStartRotations[i] = lagTargetRotations[i];

                lagStartRotations[newBonesRotations.Length - 1] = lagTargetRotations[newBonesRotations.Length - 1];
            }
            else
            {
                for (int i = 0; i < lagStartRotations.Length - 1; i++)
                    lagStartRotations[i] = lagTargetRotations[i];

                lagStartRotations[newBonesRotations.Length - 1] = lagTargetRotations[newBonesRotations.Length - 1];
            }

            for (int i = 0; i < lagTargetRotations.Length - 1; i++) lagTargetRotations[i] = hardTargetRoatations[i];
            lagTargetRotations[newBonesRotations.Length - 1] = hardTargetRoatations[newBonesRotations.Length - 1];
        }


        #endregion


        /// <summary>
        /// Methods is calculating if character look goes out of range, clamping rotations, elastic clamp etc.
        /// </summary>
        private Vector2 CalculateLimitationAndStuff(Vector3 angles, bool parental)
        {
            if (LookState == EFHeadLookState.OutOfMaxDistance) return Vector2.zero;

            Vector3 lookRotation = angles;
            Vector3 characterRotation = BaseTransform.rotation.eulerAngles;

            if (FixingPreset == EFAxisFixOrder.Parental) characterRotation = Vector3.zero;

            // Vector with degrees differences to all axes
            Vector2 deltaVector = new Vector3(Mathf.DeltaAngle(lookRotation.x, characterRotation.x), Mathf.DeltaAngle(lookRotation.y, characterRotation.y));

            // If we look in big angle we set variables to rotate head to the front in later calculations
            float deltaYAbs = Mathf.Abs(deltaVector.y);

            float maxRotOff = MaxRotationDiffrence;
            if (Mathf.Abs(XRotationLimits.x) > MaxRotationDiffrence) maxRotOff = Mathf.Abs(XRotationLimits.x);
            if (Mathf.Abs(XRotationLimits.y) > MaxRotationDiffrence) maxRotOff = Mathf.Abs(XRotationLimits.y);

            bool change = false;

            if (Prevent180Error)
            {
                if (Mathf.Sign(lastDeltaVector.y) != Mathf.Sign(deltaVector.y))
                {
                    // When we reach 180 degrees change, make model straighten to default for a moment
                    if (Mathf.Abs(lastDeltaVector.y) > Mathf.Max(90, MaxRotationDiffrence))
                    {
                        LookState = EFHeadLookState.OutOfMaxRotation;

                        if (!LimitHolder)
                        {
                            targetLookRotation = LeadBone.rotation;
                            holdGoBack = 0.135f;
                            change = true;
                        }
                    }
                }
            }

            if (!change)
                if (deltaYAbs > maxRotOff)
                {
                    LookState = EFHeadLookState.OutOfMaxRotation;

                    if (!LimitHolder) targetLookRotation = LeadBone.rotation;
                }
                else
                {
                    if (LookState == EFHeadLookState.OutOfMaxRotation) SmoothChangeTarget(0.5f, 0.3f, true);

                    LookState = EFHeadLookState.Following;
                    smoothingOutOfMaxRange = false; // Setting flag back (see var summary)
                }

            // Additional option, accelerating head rotation speed in certain difference point - adding small touch for animation to look more realistic
            if (deltaYAbs > DeltaAccelerationRange)
            {
                deltaLerpAccelerator = Mathf.Lerp(1f, 1.5f, Mathf.InverseLerp(DeltaAccelerationRange, maxRotOff, deltaYAbs));
            }


            // Limiting rotation when looking around
            if (LookState == EFHeadLookState.Following)
            {
                Vector2 diffVector = lastDeltaVector - deltaVector;
                if (Mathf.Abs(diffVector.y) > BigAngleSmoother)
                {
                    float smootherVal = Mathf.Lerp(0.55f, 0.3f, Mathf.InverseLerp(BigAngleSmoother, 180f, diffVector.y)) * 0.8f;
                    SmoothChangeTarget(smootherVal * 0.9f, smootherVal, true);
                }

                // Limit when looking up or down
                if (deltaVector.x > YRotationLimits.y)
                {
                    float elasticRange = 0f;
                    if (YElasticRange > 0f)
                    {
                        elasticRange = Mathf.Abs(deltaVector.x - YRotationLimits.y);
                        elasticRange = FEasing.EaseOutCubic(0f, YElasticRange, elasticRange / (180f - YRotationLimits.y));
                    }

                    lookRotation.x = characterRotation.x - YRotationLimits.y - elasticRange;
                    LookState = EFHeadLookState.ClampedAngle;
                }
                else if (deltaVector.x < YRotationLimits.x)
                {
                    float elasticRange = 0f;
                    if (YElasticRange > 0f)
                    {
                        elasticRange = Mathf.Abs(deltaVector.x - YRotationLimits.x);
                        elasticRange = FEasing.EaseOutCubic(0f, YElasticRange, elasticRange / (180f - YRotationLimits.x));
                    }

                    lookRotation.x = characterRotation.x - YRotationLimits.x + elasticRange;
                    LookState = EFHeadLookState.ClampedAngle;
                }


                // Limit when looking left or right
                if (deltaVector.y > -XRotationLimits.x)
                {
                    float elasticRange = 0f;
                    if (XElasticRange > 0f)
                    {
                        elasticRange = Mathf.Abs(deltaVector.y + XRotationLimits.x);
                        elasticRange = FEasing.EaseOutCubic(0f, XElasticRange, elasticRange / (180f + XRotationLimits.x));
                    }

                    lookRotation.y = characterRotation.y - XRotationLimits.y - elasticRange;
                    LookState = EFHeadLookState.ClampedAngle;
                }
                else if (deltaVector.y < -XRotationLimits.y)
                {
                    float elasticRange = 0f;
                    if (XElasticRange > 0f)
                    {
                        elasticRange = Mathf.Abs(deltaVector.y + XRotationLimits.y);
                        elasticRange = FEasing.EaseOutCubic(0f, XElasticRange, elasticRange / (180f + XRotationLimits.y));
                    }

                    lookRotation.y = characterRotation.y + XRotationLimits.y + elasticRange;
                    LookState = EFHeadLookState.ClampedAngle;
                }
            }
            else // (only option for OutOfMaxRotation) If we look in big angle we rotate head to default bone rotation with smoothness
            {
                // Setting smoothness variable for milder change head rotation
                if (!smoothingOutOfMaxRange)
                {
                    smoothingOutOfMaxRange = true;
                    SmoothChangeTarget(0.1f, 0.1f, true);
                }
            }

            lastDeltaVector = deltaVector;

            if (!parental)
            {

                if (MinHeadLookAngle > 0)
                {
                    Quaternion pre = Quaternion.Euler(preLookRotation);
                    Quaternion tgt = Quaternion.Euler(lookRotation);

                    float angle = Quaternion.Angle(pre, tgt);

                    if (angle < MinHeadLookAngle) return preLookRotation;

                    // Calculating target look to be on the edge of 'minRange'
                    if (BirdMode)
                    {
                        preLookRotation = lookRotation;
                    }
                    else
                        if (angle != 0)
                    {
                        lookRotation = Quaternion.Lerp(pre, tgt, (angle - MinHeadLookAngle) / angle).eulerAngles;
                        preLookRotation = lookRotation;
                    }
                    else
                        preLookRotation = lookRotation;
                }
            }

            return lookRotation;
        }

        #endregion


        #region Utilities for look algorithm


        /// <summary>
        /// Saving current clip animation bones rotations for rotations as offsets option
        /// </summary>
        public void RememberCurrentBonesRotations(bool resetLerpRotations = false)
        {
            if (resetLerpRotations)
            {
                if (poseReferenceRotations == null || poseReferenceRotations.Length != BackBonesCount + 1) poseReferenceRotations = new Quaternion[BackBonesCount + 1];
                if (lerpedReferenceRotations == null || lerpedReferenceRotations.Length != poseReferenceRotations.Length) lerpedReferenceRotations = new Quaternion[poseReferenceRotations.Length];
            }

            if (poseReferenceRotations == null || poseReferenceRotations.Length != BackBonesCount + 1) poseReferenceRotations = new Quaternion[BackBonesCount + 1];
            if (lerpedReferenceRotations == null || lerpedReferenceRotations.Length != poseReferenceRotations.Length) lerpedReferenceRotations = new Quaternion[poseReferenceRotations.Length];

            mainReferenceRotation = BaseTransform.rotation;

            poseReferenceRotations[0] = LeadBone.rotation;

            if (BackBonesCount > 0)
                for (int i = 0; i < poseReferenceRotations.Length - 1; i++)
                    poseReferenceRotations[i + 1] = GetParentBone(i).rotation;

            if (resetLerpRotations)
            {
                for (int i = 0; i < lerpedReferenceRotations.Length; i++) lerpedReferenceRotations[i] = poseReferenceRotations[i];
                lerpedMainReferenceRotation = mainReferenceRotation;
            }
        }


        private IEnumerator CResetMomentLookTransform(Transform transform, float time)
        {
            yield return new WaitForSeconds(time);
            MomentLookTransform = transform;
        }

        /// <summary>
        /// Correcting bones look at rotations for Quaternion.LookRotation() to work correctly - different modelling softwares doing sometimes crazy things with this orientations
        /// </summary>
        private Vector3 ConvertFlippedAxes(Vector3 rotations)
        {
            rotations += RotationOffset;

            if (FixingPreset == EFAxisFixOrder.Parental)
            {
                // Will just return rotations
            }
            else if (FixingPreset == EFAxisFixOrder.FromBased)
            {
                rotations += OffsetAuto;
                rotations = (Quaternion.Euler(rotations) * Quaternion.FromToRotation(FromAuto, LookDirection)).eulerAngles;
            }
            else if (FixingPreset == EFAxisFixOrder.FullManual)
            {
                rotations.x *= RotCorrectionMultiplier.x;
                rotations.y *= RotCorrectionMultiplier.y;
                rotations.z *= RotCorrectionMultiplier.z;

                return (Quaternion.Euler(rotations) * Quaternion.FromToRotation(ManualFromAxis, ManualToAxis)).eulerAngles;

            }
            else if (FixingPreset == EFAxisFixOrder.ZYX)
            {
                return Quaternion.Euler(rotations.z, rotations.y - 90f, -rotations.x - 90f).eulerAngles;
            }

            return rotations;
        }


        /// <summary>
        /// Calculate angle between two directions around defined axis
        /// </summary>
        public static float AngleAroundAxis(Vector3 firstDirection, Vector3 secondDirection, Vector3 axis)
        {
            // Projecting to orthogonal target axis plane
            firstDirection = firstDirection - Vector3.Project(firstDirection, axis);
            secondDirection = secondDirection - Vector3.Project(secondDirection, axis);

            float angle = Vector3.Angle(firstDirection, secondDirection);

            return angle * (Vector3.Dot(axis, Vector3.Cross(firstDirection, secondDirection)) < 0 ? -1 : 1);
        }


        /// <summary> 
        /// Getting bone from avaiable references
        /// </summary>
        private Transform GetParentBone(int index)
        {
            if (BackBonesTransforms.Length == 0) return null;

            if (index < 0) index = 0;
            if (index > BackBonesTransforms.Length - 1) index = BackBonesTransforms.Length - 1;

            return BackBonesTransforms[index];
        }


        /// <summary>
        /// Getting parent transforms for back bones
        /// </summary>
        private void GetAdditionalBones(bool resetRotation = false)
        {
            if (LeadBone == null)
            {
                if (gameObject.activeInHierarchy)
                    if (Application.isPlaying) Debug.LogError("Assign lead bone first! (" + transform.name + ")");

                return;
            }

            // Getting back bones
            for (int i = 0; i < BackBonesTransforms.Length; i++)
            {
                if (i == 0)
                {
                    BackBonesTransforms[i] = LeadBone.parent;
                }
                else
                {
                    if (BackBonesTransforms[i - 1].parent)
                    {
                        BackBonesTransforms[i] = BackBonesTransforms[i - 1].parent;
                    }
                    else
                    {
                        Debug.LogError("Error during getting parents of LookTarget! (no parent?)");
                        return;
                    }
                }
            }

            // Supporting custom backbones
            for (int i = customBackBones.Count - 1; i >= 0; i--)
            {
                if (customBackBones[i].index < BackBonesTransforms.Length)
                {
                    BackBonesTransforms[customBackBones[i].index] = customBackBones[i].transform;
                }
                else
                    customBackBones.RemoveAt(i);
            }

            if (!resetRotation)
            {
                ResetBonesLerps(true);
                return;
            }

            ResetBonesLerps();
        }


        /// <summary>
        /// Resetting bones smoothing control variables
        /// </summary>
        private void ResetBonesLerps(bool onlyIfNull = false)
        {
            bool reset = true;
            if (onlyIfNull) if (lerpRotations != null) reset = false;

            if (reset)
            {
                if (lerpRotations == null || lerpRotations.Length != BackBonesTransforms.Length)
                    lerpRotations = new Quaternion[BackBonesTransforms.Length];

                if (AnimateWithSource)
                {
                    headLerpRot = Quaternion.identity;

                    for (int i = 0; i < lerpRotations.Length; i++)
                        lerpRotations[i] = Quaternion.identity;
                }
                else
                {
                    headLerpRot = LeadBone.rotation;

                    //for (int i = 0; i < lerpRotations.Length; i++)
                    //    lerpRotations[i] = BackBonesTransforms[i].rotation;

                    for (int i = 0; i < lerpRotations.Length; i++)
                        lerpRotations[i] = Quaternion.identity;
                }

                if (hardTargetRoatations == null || hardTargetRoatations.Length != BackBonesTransforms.Length + 1)
                {
                    hardTargetRoatations = new Quaternion[BackBonesTransforms.Length + 1];
                    newBonesRotations = new Quaternion[BackBonesTransforms.Length + 1];
                }

                for (int i = 0; i < newBonesRotations.Length - 1; i++)
                {
                    newBonesRotations[i] = BackBonesTransforms[i].rotation;
                    hardTargetRoatations[i] = BackBonesTransforms[i].rotation;
                }

                newBonesRotations[newBonesRotations.Length - 1] = LeadBone.rotation;
                hardTargetRoatations[newBonesRotations.Length - 1] = LeadBone.rotation;

            }

            reset = true;
            if (onlyIfNull) if (lerpRotationsUltra != null) reset = false;

            if (reset)
            {
                if (lerpRotationsUltra == null || lerpRotationsUltra.Length != BackBonesTransforms.Length)
                    lerpRotationsUltra = new Quaternion[BackBonesTransforms.Length];

                for (int i = 0; i < lerpRotationsUltra.Length; i++)
                    lerpRotationsUltra[i] = BackBonesTransforms[i].rotation;//Quaternion.identity;

                headLerpRotUltra = LeadBone.rotation;
            }
        }


        /// <summary>
        /// Calculating rotation weights for bones with or without curve
        /// </summary>
        private void CalculateBonesRotationsWeights()
        {
            if (BackBonesCount == 0)
            {
                RotationWeights = new float[0];
                return;
            }

            float sum = 0f;
            RotationWeights = new float[BackBonesTransforms.Length + 1];

            if (BackBonesTransforms.Length != 0)
            {
                // When we don't have curve to calculate weight of rotations for bones we calculate them by algorithm
                if (BackBonesFalloff.length < 2 || !CurveSpread)
                {
                    float totalValue = 1f;
                    float distributionFactor = 0.75f;

                    float varLeft = totalValue;

                    float[] targetWeights = new float[RotationWeights.Length];

                    targetWeights[0] = totalValue * distributionFactor * 0.65f;
                    varLeft -= targetWeights[0];

                    for (int i = 1; i < targetWeights.Length - 1; i++)
                    {
                        float ammountToShare = varLeft / (1f + (1f - distributionFactor)) * distributionFactor;
                        targetWeights[i] = ammountToShare;
                        varLeft -= ammountToShare;
                    }

                    targetWeights[targetWeights.Length - 1] = varLeft;
                    varLeft = 0;

                    for (int i = 0; i < targetWeights.Length; i++)
                    {
                        sum += targetWeights[i];
                    }

                    float equalVal = 1f / (float)(BackBonesTransforms.Length);
                    for (int i = 0; i < RotationWeights.Length; i++)
                    {
                        RotationWeights[i] = Mathf.LerpUnclamped(targetWeights[i], equalVal, FaloffValue * 1.25f);
                    }
                }
                else // When we have curve we use it to calculate values
                {
                    sum = 0f;
                    float curveDiv = 1f;
                    float prog = 1f / (RotationWeights.Length);

                    for (int i = 0; i < RotationWeights.Length; i++)
                    {
                        RotationWeights[i] = BackBonesFalloff.Evaluate(prog * i) / curveDiv;
                        sum += RotationWeights[i];
                    }

                    for (int i = 0; i < RotationWeights.Length; i++)
                    {
                        RotationWeights[i] /= sum;
                    }
                }
            }
            else
            {
                RotationWeights = new float[1];
                RotationWeights[0] = 1f;
            }
        }




        /// <summary>
        /// Searching through component's owner to find head or neck bone
        /// </summary>
        public void FindBaseTransform()
        {
            BaseTransform = transform;

            if (!GetComponentInChildren<Animator>())
            {
                if (!GetComponentInChildren<Animation>())
                    Debug.LogWarning(gameObject.name + " don't have animator, is it root transform for your character?");
            }
        }


        /// <summary>
        /// Method adding custom back-bone to backbones list and preventing from auto-replacing by algorithm
        /// </summary>
        public void AddCustomBackbone(Transform t, int index)
        {
            // Removing already reserved index
            for (int i = 0; i < customBackBones.Count; i++)
                if (customBackBones[i].index == index)
                {
                    customBackBones.RemoveAt(i);
                    if (t == null) return; else break;
                }

            // Adding backbone to list
            if (t != null) customBackBones.Add(new CustomBackBone(t, index));
        }


        /// <summary> Simple class to hold two variables to support custom backbones </summary>
        private class CustomBackBone
        {
            public Transform transform;
            public int index;

            public CustomBackBone(Transform t, int i)
            {
                transform = t;
                index = i;
            }
        }


        #endregion


        #region Simple structures operations

        private Vector3 WrapVector(Vector3 v)
        {
            return new Vector3(FLogicMethods.WrapAngle(v.x), FLogicMethods.WrapAngle(v.y), FLogicMethods.WrapAngle(v.z));
        }

        public float AnglePositive(float angle)
        {
            angle %= 360;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// Smooth lerping bone rotation
        /// </summary>
        Quaternion LinearInterpolateRotation(Quaternion start, Quaternion end, float value)
        {
            return Quaternion.Slerp(start, end, value);
        }

        #endregion


        #region Gizmos Drawing and other editor Stuff

        /// <summary>
        /// Auto get bones and do some calculations every change in inspector for editor's preview
        /// </summary>
        private void OnValidate()
        {
            if (BackBonesTransforms == null)
                BackBonesTransforms = new Transform[BackBonesCount];

            if (BackBonesCount != BackBonesTransforms.Length)
                BackBonesTransforms = new Transform[BackBonesCount];

            if (Application.isPlaying)
            {
                // Partial support for modifying backbones count in playmode
                GetAdditionalBones();
            }

            CalculateBonesRotationsWeights();
        }


        /// <summary>
        /// Remove all objects generated by component
        /// </summary>
        private void OnDestroy()
        {
            if (Application.isPlaying)
                if (anchorHelper) Destroy(anchorHelper.gameObject);
        }


        public void UpdateForCustomInspector()
        {
            //if (!Application.isPlaying) return;
            OnValidate();

            GetAdditionalBones(true);
            //CalculateBonesRotationsWeights();
        }

#if UNITY_EDITOR
        // Set it to false if you don't want any gizmo
        public static bool drawMainGizmo = true;
        public bool drawGizmos = true;

        private void OnDrawGizmos()
        {
            if (DebugRays)
            {
                if (LeadBone)
                {
                    lookStartReferenceTransform = LeadBone;

                    Vector3 lookStartPosition;
                    if (!AnchorReferencePoint)
                    {
                        if (lookStartReferenceTransform == null) if (BackBonesTransforms.Length > 0) lookStartReferenceTransform = BackBonesTransforms[0]; else lookStartReferenceTransform = LeadBone;
                        lookStartPosition = lookStartReferenceTransform.position + lookStartReferenceTransform.TransformVector(LookReferenceOffset);
                    }
                    else
                    {
                        if (anchorHelper)
                            lookStartPosition = anchorHelper.position + BaseTransform.TransformVector(LookReferenceOffset);
                        else
                            lookStartPosition = LeadBone.position + BaseTransform.TransformVector(LookReferenceOffset);
                    }

                    if (GetCurrentTarget()) Debug.DrawLine(lookStartPosition, GetCurrentTarget().position, new Color(0f, 0.8f, 0.4f, 0.7f));

                    if (BaseTransform)
                    {
                        Debug.DrawLine(LeadBone.position + LookReferenceOffset, BaseTransform.position, new Color(0.9f, 0.25f, 0.25f, 0.7f));
                        Debug.DrawLine(BaseTransform.position + BaseTransform.up, BaseTransform.position + BaseTransform.up + BaseTransform.forward, new Color(0.15f, 0.15f, 0.95f, 0.8f));
                    }
                }
            }

            if (!drawMainGizmo) return;

            Gizmos.DrawIcon(transform.position, "FIMSpace/FLook/FLookMainGizmo.png", true);

            if (!drawGizmos) return;

            if (LeadBone != null)
            {
                if (LeadBone != transform) Gizmos.DrawIcon(LeadBone.position, "FIMSpace/FLook/FLookFinalSegment.png");

                Vector3 previousCheckPos = LeadBone ? LeadBone.position : Vector3.zero;
                for (int i = 0; i < BackBonesTransforms.Length; i++)
                {
                    if (BackBonesTransforms[i] != null)
                    {
                        if (Vector3.Distance(BackBonesTransforms[i].position, previousCheckPos) < 0.175f)
                            continue;
                        else
                            previousCheckPos = BackBonesTransforms[i].position;

                        Gizmos.DrawIcon(BackBonesTransforms[i].position, "FIMSpace/FLook/FLookSegment.png");
                    }
                }

                if (CompensationBones != null)
                    for (int i = 0; i < CompensationBones.Count; i++)
                    {
                        if (CompensationBones[i] != null)
                        {
                            bool tooNear = false;

                            for (int b = 0; b < BackBonesTransforms.Length; b++)
                            {
                                if (BackBonesTransforms[b]) if (Vector3.Distance(BackBonesTransforms[b].position, CompensationBones[i].position) < 0.5f)
                                    {
                                        tooNear = true;
                                        break;
                                    }
                            }

                            if (!tooNear)
                                Gizmos.DrawIcon(CompensationBones[i].position, "FIMSpace/FLook/FLookCompensation.png");
                        }
                    }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            if (MaximumDistance <= 0f) return;

            Gizmos.color = new Color(0f, 0.2f, 0.85f, 0.35f);

            if (LastLookStartPosition != Vector3.zero)
                Gizmos.DrawWireSphere(LastLookStartPosition, MaximumDistance);
            else
                Gizmos.DrawWireSphere(transform.position, MaximumDistance);

        }
#endif
        #endregion

    }
}