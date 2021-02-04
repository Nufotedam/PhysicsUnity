using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Transform hipsBone;          //  Get the Hips bone to be able to apply physics when the enemy is dead
    public SkinnedMeshRenderer mannequin;

    Rigidbody[] m_Rigidbodies;          //  Get the all rigibodies of the bone hierachy (Ragdoll)
    Animator m_Animator;                //  Animator component
    
    void Awake()
    {
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        //  At the beginning desactivate the ragdoll to avoid conflict with the animator and physics of the gameobject
        DeactivateRagdoll();
        //ActivateRagdoll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateRagdoll();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeactivateRagdoll();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ApplyForce(Vector3.up * 20);
        }
    }

    public void ActivateRagdoll()
    {
        /*
         *  For each object in the bone scheme, the kinematic property is disabled 
         *  and the animator component is disabled to be able to get the physics properties of the ragdoll.
         * */
        m_Animator.enabled = false;
        //  Activate that the mesh also renders when it is off screen
        mannequin.updateWhenOffscreen = true;
        foreach (var rigibody in m_Rigidbodies)
        {
            rigibody.isKinematic = false;
        }
    }

    public void DeactivateRagdoll()
    {
        /*
         *  For each object in the bone scheme, the kinematic property is enabled 
         *  and the animator component is enabled.
         * */
        m_Animator.enabled = true;
        foreach (var rigibody in m_Rigidbodies)
        {
            rigibody.isKinematic = true;
        }
    }

    public void ApplyForce(Vector3 force)
    {
        //  Apply force to the ragdoll when the enemy is dead
        var rigibody = hipsBone.GetComponent<Rigidbody>();
        rigibody.AddForce(force, ForceMode.VelocityChange);
    }
}
