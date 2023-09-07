using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPointEffector : MonoBehaviour
{
    [SerializeField]
    float period = 2f;
    [SerializeField] float forceMag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        const float tau = Mathf.PI * 2f;
        float cycles = Time.time / period;
        this.gameObject.GetComponent<PointEffector2D>().forceMagnitude = (Mathf.Sin(cycles * tau) * forceMag);
    }
}
