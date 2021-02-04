using System.Collections;
using UnityEngine;

public class PlarformMovement : MonoBehaviour
{
    public Vector3[] movement;              //  Array of the movement of the platform, without the current position
    public Vector3 rotation;                //  Vector of the rotation of the platform
    public float speedMovement = 10.0f;     //  Speed of the platform
    public float waitTime = 1.0f;           //  Time to wait for each waypoint

    public bool canMove = true;
    public bool canRotate = false;

    int currentDestination;                 //  Current destination of the platform
    Rigidbody m_Rigidbody;

    Vector3[] waypoints;                    //  All the waypoints of the platform movement

    private void Start()
    {
        //  Get the Rigibody component to move the platform
        m_Rigidbody = GetComponent<Rigidbody>();
        //  Declare the waypoint array plus the tranform of the object
        waypoints = new Vector3[movement.Length + 1];
        //  Make the waypoints based on the current position of the movement of the platform
        MakeWaypoints();
        //  The first destination is the second waypoint in the rray
        currentDestination = 1;
    }

    private void FixedUpdate()
    {
        Movement();
        Rotation();
    }

    private void Movement()
    {
        if (canMove)
        {
            //  Move the platform to the current destination
            m_Rigidbody.MovePosition(Vector3.MoveTowards(transform.position, waypoints[currentDestination], speedMovement * Time.deltaTime));
            NextDestination();
        }        
    }

    private void Rotation()
    {
        if (canRotate)
        {
            if (canMove)
            {
                //  Rotate around an axis over time
                Quaternion deltaRotation = Quaternion.Euler(rotation * Time.fixedDeltaTime);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            }
        }
    }

    private void NextDestination()
    {
        //  If the platform arrives to the destination, then update the next destination in the array
        if (Vector3.Distance(transform.position, waypoints[currentDestination]) <= 0)
        {
            StartCoroutine(WaitNextDestination());
            //  Assign the destination and reset when it finishes
            currentDestination = (currentDestination + 1) % waypoints.Length;
        }
    }

    private IEnumerator WaitNextDestination()
    {
        canMove = false;
        yield return new WaitForSeconds(waitTime);
        canMove = true;
    }

    private void MakeWaypoints()
    {
        //  The first waypoint is the platform inicial position
        waypoints[0] = transform.position;
        for (int i = 1; i < waypoints.Length; i++)
        {
            //  Make the different waypoints with the movement array
            waypoints[i] = waypoints[i - 1] + movement[i - 1];
        }
    }
}
