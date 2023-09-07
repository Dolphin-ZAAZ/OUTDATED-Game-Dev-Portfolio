using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    GameObject player;
    GameObject[] checkpoints;
    Rigidbody playerRB;
    bool hasWon = false;
    // Start is called before the first frame update
    void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        player = GameObject.Find("Truck");
        playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        hasWon = GameObject.Find("WinTrigger").GetComponent<WinTrigger>().hasWon;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasWon && collision.gameObject.CompareTag("Player"))
        {
            player.transform.position = checkpoints[0].transform.position;
            player.transform.rotation = checkpoints[0].transform.rotation;
            playerRB.transform.Translate(0, 0.5f, 0, Space.World);
            playerRB.angularVelocity = Vector3.zero;
            playerRB.velocity = Vector3.zero;
            GameObject.Find("Truck").GetComponent<PlayerMovement>().dashTimer = 0f;
            GameObject.Find("Truck").GetComponent<PlayerMovement>().deathSoundPlayed = false;
        }
    }
}
