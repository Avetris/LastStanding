using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public sealed class PlayerRagdollController : MonoBehaviour
{
    PlayerAnimationController m_PlayerAnimationController;
    PlayerController m_PlayerController;
    PlayerInfo m_PlayerInfo;

    // parameters for control character moving while it is ragdolled
    private const float AirSpeed = 5f; // determines the max speed of the character while airborne

    // parameters needed to control ragdolling
    RagdollState m_State = RagdollState.Animated;
    float m_RagdollingEndTime;   //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
    const float RagdollToMecanimBlendTime = 0.5f;   //How long do we blend when transitioning from ragdolled to animated
    readonly List<RigidComponent> m_Rigids = new List<RigidComponent>();
    readonly List<TransformComponent> m_Transforms = new List<TransformComponent>();
    Transform m_HipsTransform;
    Rigidbody m_HipsTransformRigid;
    Vector3 m_StoredHipsPosition;
    Vector3 m_StoredHipsPositionPrivAnim;
    Vector3 m_StoredHipsPositionPrivBlend;

    public bool IsRagdolled
    {
        get
        {
            return
                m_State == RagdollState.Ragdolled ||
                m_State == RagdollState.WaitStablePosition;
        }
        set
        {
            if (value)
                RagdollIn();
            else
                RagdollOut();
        }
    }

    public void AddExtraMove(Vector3 move)
    {
        if (IsRagdolled)
        {
            Vector3 airMove = new Vector3(move.x * AirSpeed, 0f, move.z * AirSpeed);
            foreach (var rigid in m_Rigids)
                rigid.RigidBody.AddForce(airMove / 100f, ForceMode.VelocityChange);
        }
    }

    void Start()
    {
        m_PlayerAnimationController = GetComponent<PlayerAnimationController>();
        m_HipsTransform = m_PlayerAnimationController.GetBone(HumanBodyBones.Hips);
        m_HipsTransformRigid = m_HipsTransform.GetComponent<Rigidbody>();
        m_PlayerController = GetComponent<PlayerController>();
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_PlayerInfo.ClientOnStatusChange += HandleStatusChange;


        //Get all the rigid bodies that belong to the ragdoll
        Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigid in rigidBodies)
        {
            if (rigid.transform == transform)
                continue;

            RigidComponent rigidCompontnt = new RigidComponent(rigid);
            m_Rigids.Add(rigidCompontnt);
        }

        // disable ragdoll by default
        ActivateRagdollParts(false);

        //Find all the transforms in the character, assuming that this script is attached to the root
        //For each of the transforms, create a BodyPart instance and store the transform
        foreach (var t in GetComponentsInChildren<Transform>())
        {
            var trComp = new TransformComponent(t);
            m_Transforms.Add(trComp);
        }
    }

    private void OnDestroy()
    {
        m_PlayerInfo.ClientOnStatusChange -= HandleStatusChange;
    }

    private void HandleStatusChange()
    {
        if (m_PlayerInfo.hasAuthority)
        {
            IsRagdolled = !m_PlayerInfo.IsAlive() || m_PlayerInfo.HasFallenDown();
        }
    }

    void FixedUpdate()
    {
        if (m_State == RagdollState.WaitStablePosition)
        {
            GetUp();
        }

        if (m_State == RagdollState.Animated &&
            m_PlayerController.CharacterVelocity.y < -10f)
        {
            // kill and resuscitate will force character to enter in Ragdoll 
            RagdollIn();
            RagdollOut();
        }
    }

    void LateUpdate()
    {
        if (m_State != RagdollState.BlendToAnim)
            return;

        float ragdollBlendAmount = 1f - Mathf.InverseLerp(
            m_RagdollingEndTime,
            m_RagdollingEndTime + RagdollToMecanimBlendTime,
            Time.time);

        // In LateUpdate(), Mecanim has already updated the body pose according to the animations.
        // To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips
        // and slerp all the rotations towards the ones stored when ending the ragdolling

        if (m_StoredHipsPositionPrivBlend != m_HipsTransform.position)
        {
            m_StoredHipsPositionPrivAnim = m_HipsTransform.position;
        }
        m_StoredHipsPositionPrivBlend = Vector3.Lerp(m_StoredHipsPositionPrivAnim, m_StoredHipsPosition, ragdollBlendAmount);
        m_HipsTransform.position = m_StoredHipsPositionPrivBlend;

        foreach (TransformComponent trComp in m_Transforms)
        {
            //rotation is interpolated for all body parts
            if (trComp.PrivRotation != trComp.Transform.localRotation)
            {
                trComp.PrivRotation = Quaternion.Slerp(trComp.Transform.localRotation, trComp.StoredRotation, ragdollBlendAmount);
                trComp.Transform.localRotation = trComp.PrivRotation;
            }

            //position is interpolated for all body parts
            if (trComp.PrivPosition != trComp.Transform.localPosition)
            {
                trComp.PrivPosition = Vector3.Slerp(trComp.Transform.localPosition, trComp.StoredPosition, ragdollBlendAmount);
                trComp.Transform.localPosition = trComp.PrivPosition;
            }
        }

        //if the ragdoll blend amount has decreased to zero, move to animated state
        if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
        {
            m_State = RagdollState.Animated;
        }
    }

    /// <summary>
    /// Prevents jittering (as a result of applying joint limits) of bone and smoothly translate rigid from animated mode to ragdoll
    /// </summary>
    /// <param name="rigid"></param>
    /// <returns></returns>
    static IEnumerator FixTransformAndEnableJoint(RigidComponent joint)
    {
        if (joint.Joint == null || !joint.Joint.autoConfigureConnectedAnchor)
            yield break;

        SoftJointLimit highTwistLimit = new SoftJointLimit();
        SoftJointLimit lowTwistLimit = new SoftJointLimit();
        SoftJointLimit swing1Limit = new SoftJointLimit();
        SoftJointLimit swing2Limit = new SoftJointLimit();

        SoftJointLimit curHighTwistLimit = highTwistLimit = joint.Joint.highTwistLimit;
        SoftJointLimit curLowTwistLimit = lowTwistLimit = joint.Joint.lowTwistLimit;
        SoftJointLimit curSwing1Limit = swing1Limit = joint.Joint.swing1Limit;
        SoftJointLimit curSwing2Limit = swing2Limit = joint.Joint.swing2Limit;

        float aTime = 0.3f;
        Vector3 startConPosition = joint.Joint.connectedBody.transform.InverseTransformVector(joint.Joint.transform.position - joint.Joint.connectedBody.transform.position);

        joint.Joint.autoConfigureConnectedAnchor = false;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Vector3 newConPosition = Vector3.Lerp(startConPosition, joint.ConnectedAnchorDefault, t);
            joint.Joint.connectedAnchor = newConPosition;

            curHighTwistLimit.limit = Mathf.Lerp(177, highTwistLimit.limit, t);
            curLowTwistLimit.limit = Mathf.Lerp(-177, lowTwistLimit.limit, t);
            curSwing1Limit.limit = Mathf.Lerp(177, swing1Limit.limit, t);
            curSwing2Limit.limit = Mathf.Lerp(177, swing2Limit.limit, t);

            joint.Joint.highTwistLimit = curHighTwistLimit;
            joint.Joint.lowTwistLimit = curLowTwistLimit;
            joint.Joint.swing1Limit = curSwing1Limit;
            joint.Joint.swing2Limit = curSwing2Limit;


            yield return null;
        }
        joint.Joint.connectedAnchor = joint.ConnectedAnchorDefault;
        yield return new WaitForFixedUpdate();
        joint.Joint.autoConfigureConnectedAnchor = true;


        joint.Joint.highTwistLimit = highTwistLimit;
        joint.Joint.lowTwistLimit = lowTwistLimit;
        joint.Joint.swing1Limit = swing1Limit;
        joint.Joint.swing2Limit = swing2Limit;
    }

    private void RagdollIn()
    {
        //Transition from animated to ragdolled

        ActivateRagdollParts(true);     // allow the ragdoll RigidBodies to react to the environment
        m_PlayerAnimationController.Enable(false);      // disable animation
        m_State = RagdollState.Ragdolled;
        ApplyVelocity(m_PlayerController.CharacterVelocity);
    }

    /// <summary>
    /// Smoothly translate to animator's bone positions when character stops falling
    /// </summary>
    private void RagdollOut()
    {
        if (m_State == RagdollState.Ragdolled &&
            m_HipsTransformRigid.velocity.magnitude < 0.1f)
            m_State = RagdollState.WaitStablePosition;
    }

    private void GetUp()
    {
        //Transition from ragdolled to animated through the blendToAnim state
        m_RagdollingEndTime = Time.time;
        m_PlayerAnimationController.Enable(true);               //enable animation
        m_State = RagdollState.BlendToAnim;
        m_StoredHipsPositionPrivAnim = Vector3.zero;
        m_StoredHipsPositionPrivBlend = Vector3.zero;

        m_StoredHipsPosition = m_HipsTransform.position;

        // get distanse to floor
        Vector3 shiftPos = m_HipsTransform.position - transform.position;
        shiftPos.y = GetDistanceToFloor(shiftPos.y);

        // shift and rotate character node without children
        MoveNodeWithoutChildren(shiftPos);

        //Store the ragdolled position for blending
        foreach (TransformComponent trComp in m_Transforms)
        {
            trComp.StoredRotation = trComp.Transform.localRotation;
            trComp.PrivRotation = trComp.Transform.localRotation;

            trComp.StoredPosition = trComp.Transform.localPosition;
            trComp.PrivPosition = trComp.Transform.localPosition;
        }

        m_PlayerAnimationController.SetGetUp(CheckIfLieOnBack());
        ActivateRagdollParts(false);    // disable gravity on ragdollParts.
    }

    private float GetDistanceToFloor(float currentY)
    {
        RaycastHit[] hits = Physics.RaycastAll(new Ray(m_HipsTransform.position, Vector3.down));
        float distFromFloor = float.MinValue;

        foreach (RaycastHit hit in hits)
            if (!hit.transform.IsChildOf(transform))
                distFromFloor = Mathf.Max(distFromFloor, hit.point.y);

        if (Mathf.Abs(distFromFloor - float.MinValue) > Mathf.Epsilon)
            currentY = distFromFloor - transform.position.y;

        return currentY;
    }

    private void MoveNodeWithoutChildren(Vector3 shiftPos)
    {
        Vector3 ragdollDirection = GetRagdollDirection();

        // shift character node position without children
        m_HipsTransform.position -= shiftPos;
        transform.position += shiftPos;

        // rotate character node without children
        Vector3 forward = transform.forward;
        transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
        m_HipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * m_HipsTransform.rotation;
    }

    private bool CheckIfLieOnBack()
    {
        var left = m_PlayerAnimationController.GetBone(HumanBodyBones.LeftUpperLeg).position;
        var right = m_PlayerAnimationController.GetBone(HumanBodyBones.RightUpperLeg).position;
        var hipsPos = m_HipsTransform.position;

        left -= hipsPos;
        left.y = 0f;
        right -= hipsPos;
        right.y = 0f;

        var q = Quaternion.FromToRotation(left, Vector3.right);
        var t = q * right;

        return t.z < 0f;
    }

    private Vector3 GetRagdollDirection()
    {
        Vector3 ragdolledFeetPosition = m_PlayerAnimationController.GetBone(HumanBodyBones.Hips).position;
        Vector3 ragdolledHeadPosition = m_PlayerAnimationController.GetBone(HumanBodyBones.Head).position;
        Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
        ragdollDirection.y = 0;
        ragdollDirection = ragdollDirection.normalized;

        if (CheckIfLieOnBack())
            return ragdollDirection;
        else
            return -ragdollDirection;
    }

    /// <summary>
    /// Apply velocity 'predieVelocity' to to each rigid of character
    /// </summary>
    private void ApplyVelocity(Vector3 predieVelocity)
    {
        foreach (var rigid in m_Rigids)
            rigid.RigidBody.velocity = predieVelocity;
    }

    private void ActivateRagdollParts(bool activate)
    {
        m_PlayerController.CharacterEnable(!activate);

        //m_HipsTransform.GetComponentInChildren<Collider>()
        foreach (var rigid in m_Rigids)
        {
            Collider partColider = rigid.RigidBody.GetComponent<Collider>();

            // fix for RagdollHelper (bone collider - BoneHelper.cs)
            if (partColider == null)
            {
                const string colliderNodeSufix = "_ColliderRotator";
                string childName = rigid.RigidBody.name + colliderNodeSufix;
                Transform transform = rigid.RigidBody.transform.Find(childName);
                partColider = transform.GetComponent<Collider>();
            }

            partColider.isTrigger = !activate;

            if (activate)
            {
                rigid.RigidBody.isKinematic = false;
                StartCoroutine(FixTransformAndEnableJoint(rigid));
            }
            else
                rigid.RigidBody.isKinematic = true;
        }
    }

    //Declare a class that will hold useful information for each body part
    sealed class TransformComponent
    {
        public readonly Transform Transform;
        public Quaternion PrivRotation;
        public Quaternion StoredRotation;

        public Vector3 PrivPosition;
        public Vector3 StoredPosition;

        public TransformComponent(Transform t)
        {
            Transform = t;
        }
    }

    struct RigidComponent
    {
        public readonly Rigidbody RigidBody;
        public readonly CharacterJoint Joint;
        public readonly Vector3 ConnectedAnchorDefault;

        public RigidComponent(Rigidbody rigid)
        {
            RigidBody = rigid;
            Joint = rigid.GetComponent<CharacterJoint>();
            if (Joint != null)
                ConnectedAnchorDefault = Joint.connectedAnchor;
            else
                ConnectedAnchorDefault = Vector3.zero;
        }
    }

    //Possible states of the ragdoll
    enum RagdollState
    {
        /// <summary>
        /// Mecanim is fully in control
        /// </summary>
        Animated,
        /// <summary>
        /// Mecanim turned off, but when stable position will be found, the transition to Animated will heppend
        /// </summary>
        WaitStablePosition,
        /// <summary>
        /// Mecanim turned off, physics controls the ragdoll
        /// </summary>
        Ragdolled,
        /// <summary>
        /// Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
        /// </summary>
        BlendToAnim,
    }
}
