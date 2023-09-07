using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetSound : MonoBehaviour
{
    public int hitCountMax = 10;
    int hitCount;
    AudioSource richochetPlayer;
    [SerializeField] GameObject hitParticle;
    [SerializeField] AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        richochetPlayer = GetComponent<AudioSource>();
        hitCount = hitCountMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacles"))
        {
            richochetPlayer.clip = audioClips[(int)Random.Range(0, audioClips.Length -1)];
            richochetPlayer.Play();
            Instantiate(hitParticle, transform.position, new Quaternion(collision.transform.rotation.x, collision.transform.rotation.y, collision.transform.rotation.z - 180, 1));
            hitCount--;
            if (hitCount <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.GetComponent<Collider2D>().isTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
