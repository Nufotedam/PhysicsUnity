using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float speed = 5.0f;      //  Speed of the player
    public float jumpForce = 10.0f; //  Force of the jump

    Rigidbody m_Rigidbody;
    Vector3 m_MovementInput;
    Vector3 m_PlayerMovement;

    private void Start()
    {
        //  Get the Rigibody component
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        m_MovementInput.Set(horizontalMovement, 0.0f, verticalMovement);

        Jump();

        m_PlayerMovement = m_MovementInput * speed;

        m_Rigidbody.MovePosition(transform.position + m_PlayerMovement * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            m_Rigidbody.AddForce(Vector3.up * jumpForce);
        }
    }
}
