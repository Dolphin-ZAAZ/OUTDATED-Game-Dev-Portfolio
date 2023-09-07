using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RuneAnimation : MonoBehaviour
{
    [SerializeField] Light2D light;
    [SerializeField] Vector3 scaleVector;
    [SerializeField] float radius;
    [SerializeField] [Range(0.75f,1f)] float scaleFactor;
    [SerializeField] float period = 2f;

    // Start is called before the first frame update
    void Start()
    {
        light = transform.GetComponentInChildren<Light2D>();
        scaleVector = this.transform.localScale;
        radius = light.pointLightOuterRadius;
    }

    // Update is called once per frame
    void Update()
    {
        const float tau = Mathf.PI * 2f;
        float cycles = Time.time / period;
        scaleFactor = ((Mathf.Sin(cycles * tau) + 1f)/8) + 0.75f;
        this.transform.localScale = scaleVector * scaleFactor;

        light.pointLightOuterRadius = radius * scaleFactor;
    }
}
