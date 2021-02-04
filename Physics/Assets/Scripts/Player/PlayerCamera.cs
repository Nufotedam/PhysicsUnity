using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera movementCamera;
    public Transform cameraLookAt;                  //  Target which the camer will look at and follow
    public Transform cameraTarget;                  //  Target that moves with the player and the camera follows the position
    public AxisState xAxis;
    public AxisState yAxis;

    PlayerPowers powers;
    
    private void Start()
    {
        //  Lock and hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //  Get the powers component
        powers = GetComponent<PlayerPowers>();
    }

    private void FixedUpdate()
    {
        //  Every frame get the mouse x and y input
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);
        //  Follow the target object rotation
        cameraLookAt.transform.rotation = Quaternion.Euler(yAxis.Value, xAxis.Value, 0f);
    }

    private void Update()
    {
        //  Follow the position of the player with the target object
        cameraLookAt.position = Vector3.MoveTowards(cameraLookAt.position, cameraTarget.position, 10.0f);

        if (powers.isCharging)
        {
            //  Rotate the player with the camera
            float axisCamera = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, axisCamera, 0), 20 * Time.deltaTime);
        }
    }
}
