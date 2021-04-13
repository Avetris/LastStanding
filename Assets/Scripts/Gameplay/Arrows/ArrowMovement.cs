using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class ArrowMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody arrowRigidbody = null;

    // register collision
    [SyncVar(hook=nameof(OnCollisionChanged))]
    bool collisionOccurred;

    // Reference to audioclip when target is hit
    public AudioClip targetHit;

    // Use this for initialization

    private void OnCollisionChanged(bool lastState, bool newState)
    {
        AvoidMoving();
    }

    public void Shoot(Vector3 origin, Vector3 target, float time)
    {
        Vector3 initialVelocity = CalculateInitialVelocity(target, origin, time);

        arrowRigidbody.AddForce(initialVelocity, ForceMode.VelocityChange);
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

    [ServerCallback]
    void Update()
    {
        //this part of update is only executed, if a rigidbody is present
        // the rigidbody is added when the arrow is shot (released from the bowstring)
        if (arrowRigidbody != null && !collisionOccurred)
        {
            // do we fly actually?
            if (arrowRigidbody.velocity != Vector3.zero)
            {
                // get the actual velocity
                Vector3 vel = arrowRigidbody.velocity;
                // calc the rotation from x and y velocity via a simple atan2
                float angleZ = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                float angleY = Mathf.Atan2(vel.z, vel.x) * Mathf.Rad2Deg;
                // rotate the arrow according to the trajectory
                transform.eulerAngles = new Vector3(0, -angleY, angleZ);
            }
        }
    }
    
    [ServerCallback]
    public void OnCollisionEnter(Collision other)
    {
        ApplyCollision(other.gameObject.tag, other.gameObject.name, other);
    }

    private void ApplyCollision(string tag, string name, Collision other)
    {
        if (tag == "Arrow") { return; }
        if (collisionOccurred) { return; }
        AvoidMoving();
        // and a collision occurred
        collisionOccurred = true;

        if (name == "Character")
        {           
            HittedPlayer(other.gameObject.GetComponentInParent<PlayerCollisionHandler>(), 
                         other.GetContact(0).point);
            other.gameObject.GetComponentInParent<PlayerInfo>().Kill();
        }
    }

    [ClientRpc]
    private void HittedPlayer(PlayerCollisionHandler playerCollisionHandler, Vector3 collisionPoint)
    {
        Transform hittedTransform = playerCollisionHandler.GetHittedBone(collisionPoint);
        transform.SetParent(hittedTransform, true);
    }

    private void AvoidMoving()
    {
        arrowRigidbody.useGravity = false;
        arrowRigidbody.velocity = Vector3.zero;
        // disable the rigidbody
        // rigidbody.isKinematic = true;
        arrowRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
