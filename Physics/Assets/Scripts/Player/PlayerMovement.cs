using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 7.0f;            //  Speed of the player movement
    public float jumpForce = 10.0f;             //  Force of the player jumping
    public float stepDown = 30.0f;              //  Force applied to the player when falling without jumping
    public float pushPower = 2.0f;
    [Tooltip("Fall velocity when the player is on a slope")]
    public float fallSlopeVelocity = 8f;      //  Slope fall velocity affected too by the gravity
    [Tooltip("Force in the Y-axis when the player in on a slope")]
    public float fallSlopeForce = 10.0f;        //  Force that affects in the fall of the player in the slopes
    
    [HideInInspector]
    public bool isMoving = false;

    //  Movement variables
    Vector3 m_PlayerMovement;   //  Movement Vector
    Vector3 m_Movement;         //  Input movement Vector
    float m_Speed;              //  Speed of the player, this changes if the player run
    float m_Gravity = 20.0f;    //  Gravity
    float m_FallVelocity;       //  Variable that saves and applies gravity to the player
    bool m_IsJump;              //  Control if the player has jumped
    //  Slope variables
    bool m_OnSlope = false;     //  If the player is on a slope
    Vector3 m_HitNormal;        //  Get the normal of a plane
    //  Camera Variables
    Vector3 m_CameraForward;    //  Vectors to control camera orientation
    Vector3 m_CameraSideward;       

    float massRigibody;         //  Mass of the external objects moved by the player

    Animator m_Animator;            //  Animator component 
    CharacterController controller; //  Character Controller componet

    //--
    float m_JumpForceApply;
    PlayerPowers powers;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        powers = GetComponent<PlayerPowers>();

        m_Speed = playerSpeed;

        m_JumpForceApply = jumpForce;
    }

    void Update()
    {
        //  Get the input axis (w,a,s,d)
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        //  Set the movement vector with the input
        m_Movement.Set(horizontalMove, 0f, verticalMove);
        m_Movement = Vector3.ClampMagnitude(m_Movement, 1f);

        //  Approximates and equalizes the input value if it is zero
        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        //  Determine whether or not the player is movement
        bool isMove = hasHorizontalInput || hasVerticalInput;

        CameraDirection();
        //  Set the player movement vector multiply the movement vector and the currently speed of the player
        m_PlayerMovement = (m_Movement.x * m_CameraSideward + m_Movement.z * m_CameraForward) * m_Speed;

        if (!powers.isCharging)
        {
            //  Rotates the player with the camera orientation
            controller.transform.LookAt(controller.transform.position + m_PlayerMovement);
        }

        //  Apply the gravity
        Gravity();
        Jump();

        //  If the player jumped, the extra fall force does not apply
        if (controller.isGrounded && !m_IsJump)
        {
            //  Apply more force when the player fall
            m_PlayerMovement = m_PlayerMovement + Vector3.down * stepDown;
        }

        //  If the player is not charging a power
        if (!powers.isCharging)
        {
            //  Move the player using the character controller
            controller.Move((m_PlayerMovement) * Time.deltaTime);
            //Debug.Log(controller.velocity.magnitude);
            m_IsJump = !controller.isGrounded;

            if (isMove)
            {
                //  If the player is movement, then do an action
                Action();
                isMoving = true;
            }
            else
            {
                //  When the player is not moving or the input vector does not have any value, the player play the idle animation in zero
                m_Animator.SetFloat("Speed", 0);
                isMoving = false;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //  Take the normal of the plane where the player is walking
        m_HitNormal = hit.normal;

        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        massRigibody = body.mass;

        // Apply the push
        body.velocity = pushDir * pushPower / massRigibody;
    }

    void Action()
    {
        /*
         *  Determine if the player is walking, running, etc
         *  Play the animations for each action
         */
        if (Input.GetButton("Sprint"))
        {
            //  Multiply the initial speed to be able to run, if the speed change the blendtree of the animation have to change as well
            m_Speed = playerSpeed * 1.5f;
        }
        else
        {
            m_Speed = playerSpeed;
        }
        //  The animator controller take the speed
        m_Animator.SetFloat("Speed", controller.velocity.magnitude);
    }    

    void CameraDirection()
    {
        /*
         *  Get the camera orientation to make the player move with the camera orientation
         *  Forward and sideward
         * */
        m_CameraForward = Camera.main.transform.forward;
        m_CameraSideward = Camera.main.transform.right;

        m_CameraForward.y = 0;
        m_CameraSideward.y = 0;

        m_CameraForward = m_CameraForward.normalized;
        m_CameraSideward = m_CameraSideward.normalized;
    }

    void Gravity()
    {
        /*
         *  Gravity applies to the player
         *  Take the gravity variable and it is apply when the player is
         *  on the ground or when the player is in air
         * */
        if (controller.isGrounded)
        {
            m_FallVelocity = -m_Gravity * Time.deltaTime;
            m_PlayerMovement.y = m_FallVelocity;
        }
        else
        {
            m_FallVelocity -= m_Gravity * Time.deltaTime;
            m_PlayerMovement.y = m_FallVelocity;
            m_Animator.SetFloat("AirSpeed", controller.velocity.y);
        }
        m_Animator.SetBool("IsGrounded", controller.isGrounded);
        FallSlope();
    }

    void Jump()
    {
        /*
         *  When the player press the space bar the jump force is apply to the player
         * */
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            m_IsJump = true;
            //  Set the fall velocity when the player jump with the jump force to be able to the player goes up
            m_FallVelocity = m_JumpForceApply;
            m_PlayerMovement.y = m_FallVelocity;

            //  Reset the power charging
            powers.isCharging = false;
        }
    }

    void FallSlope()
    {
        /*
         *  When the player is in a slope the player goes down,
         *  calculating the angle between the Y-axis and the normal slope and 
         *  comparing it with the slope limit of the character's controller to know 
         *  if the player has to fall down the slope that he cannot climb
         *  and compare if the slope is a platform with a 90 degrees plane to nor allow
         *  the player to fall down from that slope
         * */
        m_OnSlope = Vector3.Angle(m_HitNormal, Vector3.up) > controller.slopeLimit && Vector3.Angle(m_HitNormal, Vector3.up) < 89;

        if (m_OnSlope)
        {
            /*
             * Set the player position applying a the fall velocity to every X-Z axis calculating the inclination of the slope
             * And apply a force on the Y-axis to avoid bouncing
             * */
            m_PlayerMovement.x += ((1f - m_HitNormal.y) * m_HitNormal.x) * fallSlopeVelocity;
            m_PlayerMovement.z += ((1f - m_HitNormal.y) * m_HitNormal.z) * fallSlopeVelocity;
            m_PlayerMovement.y -= fallSlopeForce;
        }
    }

    public void JumpPower(float newJumpForce)
    {
        m_JumpForceApply = newJumpForce;
    }

    private void OnAnimatorMove()
    {

    }
}
