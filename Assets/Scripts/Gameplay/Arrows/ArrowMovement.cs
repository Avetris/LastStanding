using UnityEngine;
using Mirror;
using Mirror.Experimental;

public class ArrowMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody m_ArrowRigidbody = null;

    // register collision
    [SyncVar(hook = nameof(OnCollisionChanged))]
    bool m_CollisionOccurred;

    [SyncVar(hook = nameof(OnParentChanged))]
    Transform m_ParentTransform = null;

    // Reference to audioclip when target is hit
    public AudioClip targetHit;

    // Use this for initialization

    private void OnCollisionChanged(bool lastState, bool newState)
    {
        AvoidMoving();
    }

    private void OnParentChanged(Transform oldParent, Transform newParent)
    {
        transform.SetParent(newParent, true);
    }

    public void Shoot(Vector3 origin, Vector3 target, float time)
    {
        Vector3 initialVelocity = CalculateInitialVelocity(target, origin, time);

        m_ArrowRigidbody.AddForce(initialVelocity, ForceMode.VelocityChange);
    }

    private Vector3 CalculateInitialVelocity(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    [ClientCallback]
    void Update()
    {
        //this part of update is only executed, if a rigidbody is present
        // the rigidbody is added when the arrow is shot (released from the bowstring)
        if (!m_CollisionOccurred && m_ArrowRigidbody != null)
        {
            // do we fly actually?
            if (m_ArrowRigidbody.velocity != Vector3.zero)
            {
                // get the actual velocity
                Vector3 vel = m_ArrowRigidbody.velocity;
                // calc the rotation from x and y velocity via a simple atan2
                float angleZ = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                float angleY = Mathf.Atan2(vel.z, vel.x) * Mathf.Rad2Deg;
                // rotate the arrow according to the trajectory
                transform.eulerAngles = new Vector3(0, -angleY, angleZ);
            }
        }
    }

    // [ClientCallback]
    public void OnCollisionEnter(Collision other)
    {
        ApplyCollision(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyCollision(other.gameObject);
    }

    private void ApplyCollision(GameObject other)
    {
        Debug.Log($"The tag is {other.tag} - name {other.name} - layer  {other.layer}");
        if ("Untagged".Equals(other.tag)) { return; }
        if (m_CollisionOccurred) { return; }

        if (LobbyRoomManager.instance.IsPaused() && "Player".Equals(other.tag)) { return; }

        Vector3 hitDirection = m_ArrowRigidbody.velocity;

        AvoidMoving();
        // and a collision occurred
        m_CollisionOccurred = true;

        if ("Player".Equals(other.tag))
        {
            other.GetComponentInParent<PlayerInfo>().Kill();
            other.GetComponentInParent<PlayerCollisionHandler>().CmdHitArrow(other.name,
                                                                             transform.position,
                                                                             transform.rotation,
                                                                             hitDirection);
            Destroy(gameObject);
        }
    }

    private void AvoidMoving()
    {
        m_ArrowRigidbody.useGravity = false;
        m_ArrowRigidbody.velocity = Vector3.zero;
        // disable the rigidbody
        // rigidbody.isKinematic = true;
        m_ArrowRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
