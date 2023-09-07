using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    Vector3 startingPos;
    [SerializeField] bool automated = false;
    [SerializeField] bool roundedMovement = false;
    [SerializeField] Vector3 movementVector;
    [SerializeField] [Range(0f, 1f)] float movementFactor;
    [SerializeField] float period = 2f;
    bool increasing;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (automated)
        {
            if (roundedMovement)
            {
                const float tau = Mathf.PI * 2f;
                float cycles = Time.time / period;

                movementFactor = (Mathf.Sin(tau * cycles) + 1) / 2;
            }
            else
            {
                if (movementFactor <= 0)
                {
                    increasing = true;
                }
                else if (movementFactor >= 1)
                {
                    increasing = false;
                }
                if (increasing)
                {
                    movementFactor += Time.deltaTime / period * 2;
                }
                else
                {
                    movementFactor -= Time.deltaTime / period * 2;
                }    
            }
        }
        transform.position = startingPos + movementVector * movementFactor;
    }
}
