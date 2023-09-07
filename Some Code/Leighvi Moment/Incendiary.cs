using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incendiary : MonoBehaviour
{
    public Vector2 landSpot;
    public float speed = 50f;
    public float lingerTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        landSpot = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        this.transform.GetChild(2).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, landSpot, speed * Time.deltaTime);
        if (new Vector2 (this.transform.position.x, this.transform.position.y) == landSpot)
        {
            this.transform.GetChild(2).gameObject.SetActive(true);
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(3).gameObject.SetActive(false);
            Destroy(this.gameObject, lingerTime);
        }
    }
}
