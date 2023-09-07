using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [SerializeField] GameObject boulder;
    [SerializeField] float fireRate = 1f;
    float countdown = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            countdown = fireRate;
            Instantiate(boulder, this.transform.position, Quaternion.identity);
        }
    }
}
