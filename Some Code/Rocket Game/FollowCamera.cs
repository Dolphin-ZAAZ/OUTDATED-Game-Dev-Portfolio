using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject ship;
    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.Find("Rocket");
    }

    // Update is called once per frame
    void Update()
    {
        float shipYPos = ship.transform.position.y;
        float shipXPos = ship.transform.position.x;

        transform.position = new Vector3(shipXPos, shipYPos + 3, transform.position.z);
    }
}
