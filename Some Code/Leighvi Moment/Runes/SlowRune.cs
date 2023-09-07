using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRune : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
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
            GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().slowActive = true;
            Destroy(this.gameObject);
        }
    }
}
