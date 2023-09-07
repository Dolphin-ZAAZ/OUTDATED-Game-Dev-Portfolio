using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] bool aiming = false;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float bulletForce = 10000f;
    float countdown = 0;
    GameObject barrel;
    Transform bulletSpawn;
    // Start is called before the first frame update
    void Start()
    {
        bulletSpawn = this.transform.GetChild(0).GetChild(0).GetChild(0);
        barrel = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (aiming)
        {
            Vector3 pos = (GameObject.Find("Truck").transform.position - barrel.transform.position);
            barrel.transform.LookAt(GameObject.Find("Truck").transform.position);
            if (countdown <= 0)
            {
                countdown = fireRate;
                GameObject pellet = Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
                pellet.GetComponent<Rigidbody>().AddRelativeForce(barrel.transform.forward * bulletForce, ForceMode.Impulse);
            }
        }
        else
        {
            if (countdown <= 0)
            {
                countdown = fireRate;
                GameObject pellet = Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
                pellet.GetComponent<Rigidbody>().AddRelativeForce(this.transform.forward * bulletForce, ForceMode.Impulse);
            }
        }
    }
}
