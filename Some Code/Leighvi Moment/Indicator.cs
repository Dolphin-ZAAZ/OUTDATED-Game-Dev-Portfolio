using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] GameObject indicator;

    GameObject player;

    Camera cam;

    Renderer rd;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerCharacter");
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        rd = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rd.IsVisibleFrom(cam) == false)
        {
            if (indicator.activeSelf == false)
            {
                indicator.SetActive(true);
            }
            LayerMask layerMask = 1 << 9;

            Vector2 direction = player.transform.position - transform.position;

            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, layerMask);

            if (ray.collider != null)
            {
                indicator.transform.position = ray.point;
            }    
        }
        else
        {
            if (indicator.activeSelf == true)
            {
                indicator.SetActive(false);
            }
        }
    }
}
