using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSelf : MonoBehaviour
{
    [SerializeField] float timeDelay = 2f;
    float timer;
    private void OnEnable()
    {
        timer = timeDelay;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
