using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPhysics : MonoBehaviour
{
    public float forceApply = 30.0f;        //  Force of the punch
    public float punchDamage = 50.0f;       //  Damage of the punch

    //  Variables
    bool isCollision;           //  Check if the object can react to collision
    Vector3 directionForce;

    private void OnCollisionEnter(Collision collision)
    {
        if (isCollision)
        {
            //  Apply the force to the rigibody ignoring the mass
            collision.rigidbody.AddForceAtPosition(directionForce * forceApply, collision.transform.position, ForceMode.VelocityChange);
            //  Reset the collision
            isCollision = false;

            //  Take damage to the object
            DestroyableObject destroyableObject = collision.collider.gameObject.GetComponent<DestroyableObject>();
            if (destroyableObject)
                destroyableObject.TakeDamage(punchDamage);
        }
    }

    public void CollisionForce(Vector3 direction)
    {
        //  Start the collision
        isCollision = true;
        directionForce = direction;
        Invoke("ResetPunch", 0.5f);
    }

    void ResetPunch()
    {
        //  Reset the collision if after a timer the punch did not collide with something
        isCollision = false;
    }
}
