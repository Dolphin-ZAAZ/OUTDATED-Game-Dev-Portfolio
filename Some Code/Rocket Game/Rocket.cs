using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    GameObject winDoor;
    [SerializeField] int collectibleCount = 5;
    int currentCollectibleCount = 0;
    Text scoreText;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] bool cancelCollisions = false;

    enum State {Alive, Dying, Trancending}
    [SerializeField] State state = State.Alive;

    Rigidbody rigidBody;

    AudioSource audioSource;
    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip collectSound;

    [SerializeField] ParticleSystem thrustParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] ParticleSystem winParticle;

    int currentScene = 0;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (GameObject.Find("Score") != null)
        {
            scoreText = GameObject.Find("Score").GetComponent<Text>();
        }
        winDoor = GameObject.Find("Win Door");
        currentCollectibleCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        if (state == State.Alive)
        {
            if (scoreText != null)
            {
                scoreText.text = currentCollectibleCount.ToString() + "/" + collectibleCount.ToString();
            }
                Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; }
        if (cancelCollisions) { return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                WinSequence();
                break;
            default:
                DeathSequence();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectibleTally(other);
    }

    private void CollectibleTally(Collider other)
    {
        if (other.gameObject.tag == "Collectible")
        {
            if (currentCollectibleCount < collectibleCount - 1)
            {
                currentCollectibleCount++;
                GameObject.Find("Landing Pad").GetComponent<AudioSource>().PlayOneShot(collectSound);
                Destroy(other.gameObject);
            }
            else
            {
                currentCollectibleCount = collectibleCount;
                winDoor.SetActive(false);
                GameObject.Find("Landing Pad").GetComponent<AudioSource>().PlayOneShot(collectSound);
                Destroy(other.gameObject);
            }
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            cancelCollisions = !cancelCollisions;
        }
    }

    private void Thrust()
    {
        if (Input.GetAxis("Boost") > 0)
        {
            AddThrust();
        }
        else
        {
            StopThrust();
        }
    }

    private void Rotate()
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            rigidBody.angularVelocity = Vector3.zero;
            transform.Rotate(Vector3.forward * rcsThrust * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            rigidBody.angularVelocity = Vector3.zero;
            transform.Rotate(-Vector3.forward * rcsThrust * Time.deltaTime);
        }
    }

    private void AddThrust()
    {
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustSound);
        }
        if (!thrustParticle.isPlaying)
        {
            thrustParticle.Play();
        }
    }

    private void StopThrust()
    {
        thrustParticle.Stop();
        audioSource.Stop();
    }

    private void LoadNextScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
            SceneManager.LoadScene(nextScene);
    }

    private void LoadCurrentScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    private void WinSequence()
    {
        state = State.Trancending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticle.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void DeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        thrustParticle.Stop();
        deathParticle.Play();
        Invoke("LoadCurrentScene", levelLoadDelay);
    }
}
