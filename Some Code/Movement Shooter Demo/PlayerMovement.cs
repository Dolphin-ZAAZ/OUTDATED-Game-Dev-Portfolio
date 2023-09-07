using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Camera")]
    [SerializeField] float mouseSensitivity = 1f;
    GameObject cam;

    [Header("Base Movement")]
    [SerializeField] Transform playerTransform;
    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 2.5f;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] float jumpHeight = 5f;
    Vector3 move;
    bool hasJumped;
    bool hasDoubleJumped;
    Vector3 velocity;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    [Header("Wall Check")]
    [SerializeField] float wallRadius = 1.25f;
    [SerializeField] LayerMask wallLayer;
    bool isOnWall;
    bool tilted;
    bool unTilted;
    Vector3 tiltAngle;
    Vector3 wallNormal;

    [Header("Wall Jump")]
    [SerializeField] float wallSpeed = 5f;
    [SerializeField] float wallDecay = 200f;
    [SerializeField] float wallCutOff = 2f;
    float currWallSpeed;
    bool wallJumped;
    
    [Header("Dash")]
    [SerializeField] float dashSpeed = 5f;
    [SerializeField] float dashDecay = 5f;
    [SerializeField] float dashCutOff = 5f;
    [SerializeField] float dashCountdown = 3.5f;
    float currDashSpeed;
    float dashTimer = 0f;
    bool dashed;
    bool dashInactive;
    Vector3 dashDirection;

    [Header("Crouch")]
    [SerializeField] float slideMultiplier = 5f;
    [SerializeField] float slideMax;
    [SerializeField] float slideDecay = 5f;
    [SerializeField] float inAirslideDecay = 10f;
    [SerializeField] float slideCutOff = 5f;
    float currSlideSpeed;
    bool hasSlide;
    bool slideSpeedSet;
    bool inSlide;
    bool floorTilted;
    bool launchSet;
    bool launchVelSet = false;
    bool noWallTilt = false;
    Vector3 slideDirection;
    Vector3 slideTilt;
    Vector3 floorNormal;

    [Header("Up or Down")]
    [SerializeField] float upwardsDecay;
    [SerializeField] float downwardsBuild;
    [Range(-1, 1)] int upOrDown;
    Vector3 dir;
    float dirYInverse = 0f;
    #endregion

    void Start()
    {
        //Camera assignment
        cam = GameObject.Find("MainCamera");
        cam.transform.position = this.transform.position + new Vector3(0,0.8f,0);
        cam.transform.parent = this.transform;

        //Sets default untilted state
        tilted = false;
        unTilted = false;

        //Sets launchSet
        launchSet = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CheckSlope(hit);
        //Tilt player
        CheckWall(hit);
    }

    

    void Update()
    {
        //Gravity
        velocity.y -= gravity * Time.deltaTime;

        //Dash cooldown
        dashTimer -= Time.deltaTime;

        //Check Spheres
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
        isOnWall = Physics.CheckSphere(groundCheck.position, wallRadius, wallLayer);

        if (isGrounded && !hasJumped)
        {
            velocity.y = -1;
        }

        //Horizontal mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        playerTransform.Rotate(Vector3.up * mouseX, Space.Self);



        Jump();

        Dash();

        WallJump();

        UntiltPlayer();



        //Movement input
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float verticle = Input.GetAxis("Vertical") * speed;

        //Movement vector
        move = transform.right * horizontal + transform.forward * verticle;
        velocity.x = move.x;
        velocity.z = move.z;
        controller.Move(velocity * Time.deltaTime);
    }
    private void CheckWall(ControllerColliderHit hit)
    {
        if (!isGrounded && hit.normal.y < 0.005f && isOnWall)
        {
            if (!tilted && !noWallTilt)
            {
                wallNormal = hit.normal;
                tiltAngle = new Vector3(hit.normal.z, hit.normal.y, -hit.normal.x) * 10;
                playerTransform.Rotate(tiltAngle, Space.World);
                tilted = true;
                unTilted = false;
            }
        }
    }

    private void CheckSlope(ControllerColliderHit hit)
    {
        if (isGrounded)
        {
            floorNormal = hit.normal;
            Vector3 temp = Vector3.Cross(hit.normal, controller.transform.forward);
            dir = Vector3.Cross(temp, hit.normal); // this does no work for 
            dirYInverse = 1 + (Mathf.Asin(dir.y) * Mathf.Rad2Deg) / 180;
            if (dir.y > 0 && floorNormal.y != 1f)
            {
                upOrDown = 1;
            }
            else if (floorNormal.y == 1f)
            {
                upOrDown = 0;
            }
            else
            {
                upOrDown = -1;
            }
        }
    }

    private void Jump()
    {
        //Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            hasDoubleJumped = false;
            hasJumped = true;
        }
        //Double jump
        else if (!isGrounded && Input.GetButtonDown("Jump") && !hasDoubleJumped)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            hasDoubleJumped = true;
            //Wall jump input
            if (isOnWall && !noWallTilt)
            {
                hasDoubleJumped = false;
                velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
                wallJumped = true;
            }
        }
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }
        if (isOnWall && velocity.y <= 0f && !noWallTilt)
        {
            velocity.y = 0;
            hasDoubleJumped = false;
        }
        if (velocity.y < 0 && hasJumped)
        {
            hasJumped = false;
        }
    }

    private void Slide()
    {
        //Slide and Crouch
        if (isGrounded && velocity.y <= 0f) //Execute when player becomes grounded
        {
            if (!slideSpeedSet && dir.y <= 0)
            {
                currSlideSpeed = -velocity.y;
                currSlideSpeed = Mathf.Clamp(currSlideSpeed, 0, slideMax);
                slideDirection = controller.transform.forward;
                slideSpeedSet = true;
            }
            if (floorNormal.y != 1f) //keep player stuck to a slope
            {
                velocity.y = -50f;
            }
            else
            {
                velocity.y = -1f;
            }
        }
        if (Input.GetButton("Crouch"))
        {
            if (!isGrounded && !hasSlide)
            {
                hasSlide = true;
            }
        }
        if (Input.GetButtonUp("Crouch"))
        {
            hasSlide = false;
        }
        Vector3 slide = (slideDirection * currSlideSpeed * slideMultiplier * Time.deltaTime);
        if (hasSlide /*Only initiate if crouch button is down and you're not grounded*/ && ((currSlideSpeed >= slideCutOff || launchSet /*Either cancel slide on ground when cutoff is reached or cancel once vel = 0 when in air (launchSet)*/))) 
        {
            noWallTilt = true; // don't tilt when hitting the wall while in slide
            if ((floorNormal.y != 1f || isGrounded) && !launchVelSet && !hasJumped) //stick to ground if touching the ground
            {
                velocity.y = -240f; 
            }
            if (!isGrounded) // once not touching the ground, launch into the air
            {
                playerTransform.eulerAngles = new Vector3(0, playerTransform.eulerAngles.y, 0); //resets rotation besides for mouse rotation
                floorTilted = false;
                if (!launchVelSet && !hasJumped)
                {
                    velocity.y = 1 / floorNormal.y * currSlideSpeed / slideMultiplier;
                    launchVelSet = true;
                }
                launchSet = true;
            }
            if (currSlideSpeed > 0f) // apply drag
            {
                if (!isGrounded) //Air drag
                {
                    currSlideSpeed -= inAirslideDecay * Time.deltaTime;
                }
                else //floor drag
                {
                    if (upOrDown > 0)
                    {
                        currSlideSpeed -= slideDecay * upwardsDecay * Time.deltaTime;
                    }
                    else if(upOrDown < 0)
                    {
                        currSlideSpeed -= 0;
                    }
                    else
                    {
                        currSlideSpeed -= slideDecay * Time.deltaTime;
                    }
                }
            }
            if (!floorTilted && !launchSet)//tilt player while in slide
            {
                slideTilt = new Vector3(-1, 0, 0) * 10;
                playerTransform.Rotate(slideTilt, Space.Self);
                floorTilted = true;
            }
            if (currSlideSpeed <= 0f)
            {
                launchSet = false;
                launchVelSet = false;
            }
            inSlide = true;
            controller.Move(slide);
        }
        else // execute when slide is finished
        {
            noWallTilt = false;
            if (floorTilted) // if you're tilted from slide, untilt
            {
                playerTransform.eulerAngles = new Vector3(0, playerTransform.eulerAngles.y, 0); //resets rotation besides for mouse rotation
                floorTilted = false;
                if (!Input.GetButtonDown("Jump"))
                {
                    velocity.y = -1f;
                }
            }
            if (velocity.y != -50f) //reset slide speed if not on a slope
            {
                slideSpeedSet = false;
            }
            currSlideSpeed = 1f;
            if (isGrounded)
            {
                inSlide = false;
            }
        }
    }

    private void Dash()
    {
        //Dash
        if (dashTimer <= 0)
        {
            dashInactive = false;
        }
        if (Input.GetButtonDown("Dash") && !dashInactive)
        {
            dashDirection = cam.transform.forward;
            dashed = true;
            dashInactive = true;
            currDashSpeed = dashSpeed;
            dashTimer = dashCountdown;
        }
        if (dashed && currDashSpeed >= dashCutOff)
        {
            controller.Move(dashDirection * currDashSpeed * Time.deltaTime);
            currDashSpeed -= dashDecay * Time.deltaTime;
            velocity.y = 0f;
        }
        else
        {
            dashed = false;
            currDashSpeed = dashSpeed;
        }
    }

    private void WallJump()
    {
        //Wall jump execution
        if (wallJumped && currWallSpeed >= wallCutOff && !noWallTilt)
        {
            controller.Move(wallNormal * currWallSpeed * Time.deltaTime);
            currWallSpeed -= wallDecay * Time.deltaTime;
        }
        else
        {
            wallJumped = false;
            currWallSpeed = wallSpeed;
        }
    }

    private void UntiltPlayer()
    {
        //Untilt player
        if (!isOnWall && !noWallTilt)
        {
            if (!unTilted && tilted)
            {
                unTilted = true;
                tilted = false;
                playerTransform.Rotate(-tiltAngle, Space.World);
            }
        }
    }
}
