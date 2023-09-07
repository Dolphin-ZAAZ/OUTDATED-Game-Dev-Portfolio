using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float bounceForce = 250000;
    bool hasBounced;
    [SerializeField] float bouncedCountdown = 0f;
    float bouncedTimer = 0.1f;

    public AudioSource carSound;
    public AudioSource gameplayMusic;
    public AudioSource actionSounds;
    public AudioSource actionSounds2;
    public AudioSource winSource;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip deathSound;
    public AudioClip winSound;
    public bool jumpSoundPlayed;
    public bool dashSoundPlayed;
    public bool deathSoundPlayed;
    public bool winSoundPlayed;

    public float force;
    public float sideForce;
    public float forwardForce;
    public float initBoostForce;
    public float upwardForce;
    public float initUpForce;
    public float velocity;
    public float tiltSpeed;
    public float inAirFuel;
    public float inAirFuelDefault;
    public float jumpForce;
    public float dipForce;
    public float dashForce;

    public bool hover = false;
    public bool boost = false;
    public bool inAirActive = false;
    public bool usingFuel = false;
    public bool gravity = true;

    public bool isGrounded;
    public bool hasJumped;
    public bool hasDipped;

    public float dashTimer;
    public float dashCountdown = 5f;

    float rot = 0;

    bool rotStop;
    public float rotSpeed = 0.1f;

    UI_Controller uiController;

    TextMeshProUGUI inAir;
    TextMeshProUGUI dash;
    // Start is called before the first frame update
    void Start()
    {
        hasBounced = false;
        inAir = GameObject.Find("InAirFuel").GetComponent<TextMeshProUGUI>();
        dash = GameObject.Find("DashTimer").GetComponent<TextMeshProUGUI>();
        deathSoundPlayed = true;
        winSoundPlayed = true;
        rb = GetComponent<Rigidbody>();
        rotStop = false;
        Cursor.lockState = CursorLockMode.Locked;
        hasJumped = false;
        uiController = GameObject.Find("Canvas").GetComponent<UI_Controller>();
    }

    void Update()
    {
        bouncedTimer -= Time.deltaTime; 
        if (bouncedTimer <= 0)
        {
            hasBounced = false;
        }
        else
        {
            hasBounced = true;
        }
        if (!deathSoundPlayed)
        {
            actionSounds.clip = deathSound;
            actionSounds.Play();
            deathSoundPlayed = true;
        }
        if (!winSoundPlayed)
        {
            gameplayMusic.Stop();
            winSource.Play();
            winSoundPlayed = true;
        }
        if (Input.GetButtonDown("Reset") && Debug.isDebugBuild)
        {
            ResetOrietnation();
        }
        if (!uiController.isPaused)
        {
            if (!inAirActive)
            {
                inAir.text = "";
            }
            else
            {
                inAir.text = "In Air Fuel: " + (int)(inAirFuel * 100f);
            }
            if (dashTimer > 0 && !inAirActive)
            {
                dash.text = "Dash: " + (int)(dashTimer + 1f) + "s";
            }
            else if (!inAirActive)
            {
                dash.text = "Dash: Ready";
            }
            else
            {
                dash.text = "";
            }
        }
    }
        
    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;
        gravity = rb.useGravity;
        if ((Input.GetAxis("Vertical") * 1.5f + 0.5f) == 0)
        {
            carSound.pitch = 0.01f;
        }
        else
        {
            carSound.pitch = Input.GetAxis("Vertical") * 1.5f + 0.5f;
        }
        if (isGrounded && inAirActive)
        {
            inAirFuel = inAirFuelDefault;
        }
        else if (isGrounded && !inAirActive)
        {
            inAirFuel = -1;
        }

        if (((Input.GetButton("UpwardBoost") && hover) || (Input.GetButton("ForwardBoost") && boost) || Input.GetButton("SideSlide") || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && !isGrounded)
        {
            usingFuel = true;
        }
        else
        {
            usingFuel = false;
        }

        if (!Input.GetButton("Tilt"))
        {
            if (isGrounded)
            {
                BasicMovement();
            }
            else if (inAirActive && inAirFuel > 0)
            {
                BasicMovement();
            }
        }
        else
        {
            RotationalMovement();
        }

        AlternateMovement();

        if (!isGrounded && usingFuel && gravity)
        {
            inAirFuel -= Time.fixedDeltaTime;
        }
    }
   
    private void ResetOrietnation()
    {
        Vector3 oldRot = rb.rotation.eulerAngles;
        rb.MoveRotation(Quaternion.Euler( 0, oldRot.y,0));
        rb.transform.Translate(0, 2,0, Space.World);
        rb.angularVelocity = Vector3.zero;
    }

    private void BasicMovement()
    {
        if (!boost || Input.GetAxis("ForwardBoost") == 0)
        {
            if (velocity < 15f)
            {
                rb.AddRelativeForce(0, 0, Input.GetAxis("Vertical") * force);
            }
        }

        rot = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.transform.Rotate(Vector3.up * rot * rotSpeed, Space.Self);
        }
    }
    private void AlternateMovement()
    {
        if(inAirActive && inAirFuel > 0)
        {
            rb.AddRelativeForce(Input.GetAxis("SideSlide") * sideForce, 0, 0);
            if (velocity < 25f && boost)
            {
                rb.AddRelativeForce(0, 0, Input.GetAxis("ForwardBoost") * forwardForce);
            }
            if (hover && rb.velocity.y < 15)
            {
                rb.AddRelativeForce(0, Input.GetAxis("UpwardBoost") * upwardForce, 0);
            }
        }

        if (Input.GetAxis("UpwardBoost") > 0 && !hasJumped && !hover)
        {
            rb.AddForce(0, Input.GetAxis("UpwardBoost") * jumpForce, 0, ForceMode.Impulse);
            actionSounds2.clip = jumpSound;
            actionSounds2.Play();
            hasJumped = true;
            hasDipped = false;
        }
        else if (Input.GetAxis("UpwardBoost") < 0 && !hover && !hasDipped)
        {
            rb.AddForce(0, Input.GetAxis("UpwardBoost") * dipForce, 0, ForceMode.Impulse);
            hasDipped = true;
        }
        if (Input.GetButton("ForwardBoost") && dashTimer < 0 && !boost)
        {
            rb.AddRelativeForce(0, 0, dashForce,ForceMode.Impulse);
            actionSounds.clip = dashSound;
            actionSounds.Play();
            dashTimer = dashCountdown;
        }
        dashTimer -= Time.deltaTime;
    }
    private void RotationalMovement()
    {
        rb.AddRelativeTorque(Input.GetAxis("Vertical") * tiltSpeed, 0, -Input.GetAxis("Horizontal") * tiltSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bounce Pad"))
        {
            if (!hasBounced)
            {
                rb.velocity = new Vector3(0, 0, 0);
                rb.angularVelocity = new Vector3(0, 0, 0);
                rb.AddForce(Vector3.Normalize(collision.GetContact(0).normal) * bounceForce);
                print(Vector3.Normalize(collision.GetContact(0).normal));
                bouncedTimer = bouncedCountdown;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            hasJumped = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            hasDipped = false;
        }
    }
}
