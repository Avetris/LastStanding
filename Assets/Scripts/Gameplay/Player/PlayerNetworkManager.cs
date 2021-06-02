using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private Transform m_RootBone = null;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableNoTransformComponent();
        }
    }

    public void SetNetworkChildTransform()
    {
        foreach (NetworkTransformChild child in GetComponents<NetworkTransformChild>())
        {
            Destroy(child);
        }

        InitNetworkChildTransform(m_RootBone);
    }

    private void InitNetworkChildTransform(Transform parent)
    {
        foreach (Transform child in parent)
        {
            NetworkTransformChild networkTransform = gameObject.AddComponent<NetworkTransformChild>();
            networkTransform.target = child;
            networkTransform.clientAuthority = true;
            networkTransform.compressRotation = true;
            InitNetworkChildTransform(child);
        }
    }

    private void DisableNoTransformComponent()
    {
        ChangeComponentStatus(GetComponent<PlayerController>());
        ChangeComponentStatus(GetComponent<PlayerCharacterController>());
        ChangeComponentStatus(GetComponent<PlayerAnimationController>());
        ChangeComponentStatus(GetComponent<PlayerRagdollController>());
        // ChangeComponentStatus(GetComponent<NetworkAnimator>());
        ChangeComponentStatus(GetComponent<Animator>());
        ChangeRigidbody(m_RootBone);
    }

    private void ChangeRigidbody(Transform parent)
    {
        foreach (Collider col in parent.GetComponents<Collider>())
        {
            col.isTrigger = false;
        }
    }

    private void ChangeComponentStatus(Behaviour component)
    {
        Destroy(component);
        // component.enabled = false;
    }
}
