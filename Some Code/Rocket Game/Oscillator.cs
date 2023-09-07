using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10, 0, 0);

    [Range(0f, 1f)] [SerializeField] float movementFactor;

    [SerializeField] float period = 2f;
    float test;

    Vector3 startingPosition;
    void Start()
    {
        startingPosition = transform.position;
        test = period * 1.75f;
    }

    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        test += Time.deltaTime;
        float cycles = test / period;
        const float tau = Mathf.PI * 2f;
        float rawSinOutput = Mathf.Sin(cycles * tau);

        movementFactor = rawSinOutput / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
