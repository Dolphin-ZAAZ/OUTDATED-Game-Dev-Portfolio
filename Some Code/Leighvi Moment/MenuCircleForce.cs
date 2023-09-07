using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCircleForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1f;
        this.GetComponent<Rigidbody2D>().AddForce(this.transform.up * 10, ForceMode2D.Impulse);
    }
}
