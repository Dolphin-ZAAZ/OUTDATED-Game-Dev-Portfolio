using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] float velocity;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashCooldown = 8f;
    [SerializeField] float dashTimer;
    [SerializeField] float dashTime = 0.1f;
    [SerializeField] float dashTimeTimer = 0.1f;
    Vector2 prevMovement;

    Vector2 dashEnd;
    Vector2 dashStart;
    bool dashMoving;

    Text dashText;
    Image dashScalar;
    Image dashGrey;
    Image dashControlGrey;

    Rigidbody2D body;
    [SerializeField]Camera cam;
    [SerializeField] public float walkSpeed = 10;
    [SerializeField] public float sprintSpeed = 20;
    [SerializeField] float currentSpeed = 10;
    [SerializeField] float zoomInc = 0.5f;
    [SerializeField] float zoomMin = 5f;
    [SerializeField] float zoomMax = 25f;
    [SerializeField] float zoomStart = 15f;
    Vector2 movement;
    Vector2 lookPos;

    AudioSource runFootsteps;
    AudioSource sprintFootsteps;
    AudioSource dashSound;

    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        body = this.gameObject.GetComponent<Rigidbody2D>();
        runFootsteps = GameObject.Find("RunFootsteps").GetComponent<AudioSource>();
        sprintFootsteps = GameObject.Find("SprintFootsteps").GetComponent<AudioSource>();
        dashSound = GameObject.Find("DashSound").GetComponent<AudioSource>();

        dashText = GameObject.Find("DashText").GetComponent<Text>();
        dashGrey = GameObject.Find("DashGrey").GetComponent<Image>();
        dashScalar = GameObject.Find("DashScalar").GetComponent<Image>();
        dashControlGrey = GameObject.Find("DashControlGrey").GetComponent<Image>();

        cam.orthographicSize = zoomStart;

        runFootsteps.gameObject.SetActive(false);
        sprintFootsteps.gameObject.SetActive(false);
    }

    void Update()
    {
        velocity = body.velocity.magnitude;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        lookPos = cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize < zoomMax)
        {
            cam.orthographicSize += zoomInc;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize > zoomMin)
        {
            cam.orthographicSize -= zoomInc;
        }

        if (Input.GetAxis("Walk") > 0)
        {
            currentSpeed = sprintSpeed/2;
        }
        else
        {
            currentSpeed = sprintSpeed;
        }

        if (GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            runFootsteps.gameObject.SetActive(false);
            sprintFootsteps.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Vector2 lookDir = lookPos - body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        body.rotation = angle;

        dashTimer -= Time.deltaTime;
        dashTimeTimer -= Time.deltaTime;

        if (dashTimer > 0)
        {
            dashText.text = "" + Mathf.FloorToInt(dashTimer + 1);
            dashText.color = new Color(1f, 1f, 1f, 1f);

            dashText.gameObject.SetActive(true);
            dashGrey.gameObject.SetActive(true);
            dashScalar.gameObject.SetActive(true);
            dashControlGrey.gameObject.SetActive(true);

            dashScalar.fillAmount = dashTimer / dashCooldown;
        }
        else
        {
            dashGrey.gameObject.SetActive(false);
            dashScalar.gameObject.SetActive(false);
            dashControlGrey.gameObject.SetActive(false);
            dashText.gameObject.SetActive(false);
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") == 0) || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") != 0))
        {
            body.MovePosition(body.position + movement * currentSpeed * Time.fixedDeltaTime);
            if (Input.GetButtonDown("Dash") && dashTimer < 0 && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
            {
                dashTimeTimer = dashTime;
                prevMovement = movement;
                dashSound.Play();
                dashTimer = dashCooldown;
            }
            else if (Input.GetButtonDown("Dash") && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
            {
                GameObject.Find("ErrorSound").GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            body.MovePosition(body.position + movement * currentSpeed / 1.414213562373095f * Time.fixedDeltaTime);
            if (Input.GetButtonDown("Dash") && dashTimer < 0 && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
            {
                dashTimeTimer = dashTime;
                prevMovement = movement;
                dashSound.Play();
                dashTimer = dashCooldown;
            }
            else if (Input.GetButtonDown("Dash") && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
            {
                GameObject.Find("ErrorSound").GetComponent<AudioSource>().Play();
            }
        }

        if (dashTimeTimer > 0)
        {
            body.MovePosition(body.position + prevMovement * currentSpeed * dashSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetAxis("Walk") != 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            runFootsteps.gameObject.SetActive(true);
        }
        else
        {
            runFootsteps.gameObject.SetActive(false);
        }

        if (Input.GetAxis("Walk") == 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            sprintFootsteps.gameObject.SetActive(true);
        }
        else
        {
            sprintFootsteps.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Bullet"))
        {
            body.angularVelocity = 0f;
            body.velocity = Vector2.zero;
        }
    }
}
