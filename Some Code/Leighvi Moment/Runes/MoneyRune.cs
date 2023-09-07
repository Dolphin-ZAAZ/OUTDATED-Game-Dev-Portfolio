using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyRune : MonoBehaviour
{
    AudioSource pickUp;
    AudioSource cashVoiceSound;
    public float cashRuneCountdown = 20f;
    // Start is called before the first frame update
    void Start()
    {
        pickUp = GameObject.Find("CashSound").GetComponent<AudioSource>();
        cashVoiceSound = GameObject.Find("CashVoiceSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("SpawningSystem").GetComponent<Spawning>().downtimeTimer > 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            pickUp.Play();
            cashVoiceSound.Play();
            GameObject.Find("SpawningSystem").GetComponent<Spawning>().cashRuneTimer = cashRuneCountdown;
            Destroy(this.gameObject);
        }
    }
}
