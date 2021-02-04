using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWithObjects : MonoBehaviour
{
    public LayerMask obstaculesMask;        //  Mask of the obstacules in the scene

    CharacterController controller;         //  CharacterController of the player
    //  Variables
    Vector3 m_GroundPosition;               //  Current position on the ground
    Vector3 m_LastGroundPosition;           //  Last position on the ground
    string m_GroundName;
    string m_LastGroundName;

    Quaternion m_CurrentRotation;           //  Current rotation on the ground
    Quaternion m_LastRotation;              //  Last rotation on the ground

    Vector3 m_SpherCastPosition;            //  Position of the sphere cast (Higher than the origin of the player)

    private void Start()
    {
        //  Get the CharacterController componet 
        controller = GetComponent<CharacterController>();
        //  Assign the position of the sphere cast
        m_SpherCastPosition = new Vector3(0, transform.position.y + 1, 0);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            //  Raycast information
            RaycastHit hit;
            //  Make a Sphere cast to know what is under the player
            if(Physics.SphereCast(transform.position + m_SpherCastPosition, controller.height / 4, -transform.up, out hit, 5, obstaculesMask))
            {
                //  Create a new gameobject with the hit information of the cast
                GameObject ground = hit.collider.gameObject;
                //  Assign the name, transform and rotation of the hit object
                m_GroundName = ground.name;
                m_GroundPosition = ground.transform.position;
                m_CurrentRotation = ground.transform.rotation;

                //  Compare the current position and the last position of the ground
                if (m_GroundPosition != m_LastGroundPosition && m_GroundName == m_LastGroundName)
                {
                    //  Move the object base on the gound movement
                    transform.position += m_GroundPosition - m_LastGroundPosition;
                }

                if(m_CurrentRotation != m_LastRotation && m_GroundName == m_LastGroundName)
                {
                    //  Rotate the object base on the gound movement
                    var rotation = transform.rotation * (m_CurrentRotation.eulerAngles - m_LastRotation.eulerAngles);
                    transform.RotateAround(ground.transform.position, Vector3.up, rotation.y);
                }
                //  Assign the last position of the ground object
                m_LastGroundName = m_GroundName;
                m_LastGroundPosition = m_GroundPosition;
                m_LastRotation = m_CurrentRotation;
            }
        }
        else
        {
            //  Reset the default value of the variables
            m_LastGroundName = null;
            m_LastGroundPosition = Vector3.zero;
            m_LastRotation = Quaternion.identity;
        }
    }

    private void OnDrawGizmos()
    {
        controller = GetComponent<CharacterController>();
        Gizmos.DrawWireSphere(transform.position + m_SpherCastPosition, controller.height / 4);
    }
}
