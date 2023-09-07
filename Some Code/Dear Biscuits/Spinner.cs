using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float spinSpeed = 99999999f;
    [SerializeField] float spinMax = 1.5f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {
        if(rb.angularVelocity.magnitude < spinMax)
        {
            rb.AddRelativeTorque(Vector3.up * spinSpeed * Time.deltaTime);
        }
    }
}
